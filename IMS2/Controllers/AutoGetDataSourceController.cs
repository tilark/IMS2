using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IMS2.Models;
using IMS2.ViewModels;
using IMS2.DAL;
using PagedList;
using System.Data.Entity.Infrastructure;
using ImsAutoLib;
namespace IMS2.Controllers
{
    [Authorize(Roles = "修改全院指标值, Administrators")]
    [RoutePrefix("AutoGetDataSource")]
    [Route("{action = index}/{id?}")]
    public class AutoGetDataSourceController : Controller
    {
        private ImsDbContext db = new ImsDbContext();

        // GET: AutoGetDataSource
        [Route("Index/{searchTime}/{dataSourceSystemID}")]

        public async Task<ActionResult> Index(DateTime? searchTime, Guid? dataSourceSystemID)
        {
            ViewBag.dataSourceSystemID = new SelectList(db.DataSourceSystems.Distinct().OrderBy(d => d.Priority), "DataSourceSystemId", "DataSourceSystemName");

            if (searchTime != null && dataSourceSystemID != null)
            {
                DataSourceSystemIndicatorView viewModel = new DataSourceSystemIndicatorView();
                var dataSourceSystem = await db.DataSourceSystems.FindAsync(dataSourceSystemID);
                if (dataSourceSystem != null)
                {
                    ViewBag.sourceSystemID = dataSourceSystem.DataSourceSystemId;
                    viewModel.dataSourceSystem = dataSourceSystem;
                    viewModel.searchTime = searchTime.Value;
                    viewModel.Indicators = new List<Indicator>();
                    viewModel.DepartmentIndicatorCountViews = new List<DepartmentIndicatorCountView>();
                    foreach (var indicator in dataSourceSystem.Indicators)
                    {
                        //数据来源系统负责的指标，根据指标与指定时间是否在科室指标值表（简称值表）中，如果不在，依次选择指标，追寻到科室类别项目组中的各个科室，再将该指标、科室、时间写入值表中。
                        viewModel.Indicators.Add(indicator);
                        var departmentCollection = indicator.IndicatorGroupMapIndicators.Select(i => i.IndicatorGroup)
                            .SelectMany(i => i.DepartmentCategoryMapIndicatorGroups)
                            .Select(d => d.DepartmentCategory)
                            .SelectMany(d => d.Departments).Distinct();
                        foreach (var department in departmentCollection)
                        {
                            //需查看这个department是否在viewModel.DepartmentIndicatorCountViews中
                            var query = viewModel.DepartmentIndicatorCountViews.Where(d => d.Department.DepartmentId == department.DepartmentId).FirstOrDefault();
                            if (query != null)
                            {
                                continue;
                            }
                            DepartmentIndicatorCountView view = new DepartmentIndicatorCountView();
                            view.Department = department;
                            //只显示
                            view.IndicatorCount = await db.DepartmentIndicatorValues.Where(d => d.Indicator.DataSourceSystemId == dataSourceSystem.DataSourceSystemId
                                                                   && d.DepartmentId == department.DepartmentId
                                                                   && d.Time.Year == searchTime.Value.Year
                                                                  && d.Time.Month == searchTime.Value.Month).CountAsync(); ;
                            view.SearchTime = searchTime;
                            viewModel.DepartmentIndicatorCountViews.Add(view);
                        }
                    }
                    return View(viewModel);

                }

            }
            return View();

        }
        private async Task<DepartmentIndicatorValue> CreateDepartmentIndicatorList(DateTime? searchTime, Indicator indicator, Department department, decimal? value)
        {
            if (searchTime == null || indicator == null || department == null)
            {
                return null;
            }
            bool canAdd = false;
            switch (indicator.Duration.DurationId.ToString().ToLower())
            {
                case ("d48aa438-ad71-4419-a2a2-a1c390f6c097")://月
                    canAdd = true;
                    break;
                case ("bd18c4f4-6552-4986-ab4e-ba2dffded2b3")://季
                    if (searchTime.Value.Month == 3 || searchTime.Value.Month == 6 || searchTime.Value.Month == 9 || searchTime.Value.Month == 12)
                    {
                        canAdd = true;
                    }
                    break;
                case ("24847114-90e4-483d-b290-97781c3fa0c2")://半年
                    if (searchTime.Value.Month == 6 || searchTime.Value.Month == 12)//12月也有“半年”——“下半年”
                    {
                        canAdd = true;
                    }
                    break;
                case ("ba74e352-0ad5-424b-bf31-738ba5666649")://年
                    if (searchTime.Value.Month == 12)
                    {
                        canAdd = true;
                    }
                    break;
                default:
                    //canAdd = true;
                    //break;
                    throw new Exception("跨度ID不存在。");
            }
            if (canAdd)
            {

                DepartmentIndicatorValue departmentIndicatorValue = new DepartmentIndicatorValue();
                departmentIndicatorValue.DepartmentId = department.DepartmentId;
                departmentIndicatorValue.IndicatorId = indicator.IndicatorId;
                departmentIndicatorValue.DepartmentIndicatorValueId = System.Guid.NewGuid();
                //departmentIndicatorValue.Time = searchTime.Value;
                //时间获取方式要修改——时间应为对应跨度时段的首月：
                switch (indicator.Duration.DurationId.ToString().ToLower())
                {
                    case ("d48aa438-ad71-4419-a2a2-a1c390f6c097")://月
                        departmentIndicatorValue.Time = searchTime.Value;
                        break;
                    case ("bd18c4f4-6552-4986-ab4e-ba2dffded2b3")://季
                        departmentIndicatorValue.Time = new DateTime(searchTime.Value.Year, searchTime.Value.Month - 2, 1);
                        break;
                    case ("24847114-90e4-483d-b290-97781c3fa0c2")://半年
                        departmentIndicatorValue.Time = new DateTime(searchTime.Value.Year, searchTime.Value.Month - 5, 1);
                        break;
                    case ("ba74e352-0ad5-424b-bf31-738ba5666649")://年
                        departmentIndicatorValue.Time = new DateTime(searchTime.Value.Year, searchTime.Value.Month - 11, 1);
                        break;
                    default:
                        throw new Exception("跨度ID不存在。");
                }
                //需找到最新的版本号
                var standardValue = db.DepartmentIndicatorStandards.Where(d => d.DepartmentId == department.DepartmentId && d.IndicatorId == indicator.IndicatorId
                        && d.Version == db.DepartmentIndicatorStandards.Where(i => i.DepartmentId == department.DepartmentId && i.IndicatorId == indicator.IndicatorId).Max(v => v.Version))
                        .FirstOrDefault();
                departmentIndicatorValue.IndicatorStandardId = standardValue?.DepartmentIndicatorStandardId;
                departmentIndicatorValue.IsLocked = true;
                departmentIndicatorValue.UpdateTime = DateTime.Now;
                //值需从其他系统中获取 
                departmentIndicatorValue.Value = value;
                //将科室、项目、时间添加到科室值表中
                return await UpdateDepartmentIndicatorValue(departmentIndicatorValue);
            }
            return null;
        }

        private async Task<DepartmentIndicatorValue> UpdateDepartmentIndicatorValue(DepartmentIndicatorValue departmentIndicatorValue)
        {
            DepartmentIndicatorValue item = null;
            if (departmentIndicatorValue == null)
            {
                return null;
            }
            //找到该值，如果存在，更改Value，不存在，添加
            item = db.DepartmentIndicatorValues.Where(d => d.DepartmentId == departmentIndicatorValue.DepartmentId
                            && d.IndicatorId == departmentIndicatorValue.IndicatorId
                            && d.Time.Year == departmentIndicatorValue.Time.Year && d.Time.Month == departmentIndicatorValue.Time.Month)
                            .FirstOrDefault();
            if (item == null)
            {
                item = departmentIndicatorValue;
                db.DepartmentIndicatorValues.Add(item);

            }
            else
            {
                item.Value = departmentIndicatorValue.Value;
            }
            //client win

            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    await db.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    // Update original values from the database 
                    var entry = ex.Entries.Single();
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                }

            } while (saveFailed);
            return item;
        }
        // GET: AutoGetDataSource/Details/5
        [Route("Details/{id}/{time}/{dataSourceSystemID}")]

        public async Task<ActionResult> Details(Guid? id, DateTime? time, Guid? dataSourceSystemID)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.dataSourceSystem = dataSourceSystemID;
            //查找到该department       
            var department = await db.Departments.FindAsync(id.Value);
            DepartmentIndicatorCountView viewModel = new DepartmentIndicatorCountView();
            viewModel.DepartmentIndicatorValues = new List<DepartmentIndicatorValue>();
            viewModel.Department = department;
            //从DepartmentIndicatorValue找值
            viewModel.SearchTime = time;
            var departmentIndicatorValues = await db.Departments.SelectMany(c => c.DepartmentIndicatorValues).Include(d => d.Indicator.Duration)
                                                .Where(d => d.DepartmentId == department.DepartmentId
                                                && d.Time.Year == time.Value.Year && d.Time.Month == time.Value.Month
                                                && d.Indicator.DataSourceSystemId == dataSourceSystemID).OrderBy(d => d.Indicator.Priority).ToListAsync();
            foreach (var departmentIndicatorValue in departmentIndicatorValues)
            {
                viewModel.DepartmentIndicatorValues.Add(departmentIndicatorValue);
            }
            return View(viewModel);
        }


        // POST: AutoGetDataSource/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DateTime? searchTime, Guid? dataSourceSystemID)
        {
            if (searchTime != null && dataSourceSystemID != null)
            {
                var sourceSystem = await db.DataSourceSystems.FindAsync(dataSourceSystemID.Value);
                if (sourceSystem != null)
                {
                    decimal? value = 0;//value可空，表示相关数据不存在，或除数为0等情况。
                    IndicatorValue indicatorValue = new IndicatorValue();
                    //数据来源科室负责的指标，根据指标与指定时间是否在科室指标值表（简称值表）中，如果不在，依次选择指标，追寻到科室类别项目组中的各个科室，再将该指标、科室、时间写入值表中。
                    foreach (var indicator in sourceSystem.Indicators)
                    {

                        var departmentCollection = indicator.IndicatorGroupMapIndicators.Select(i => i.IndicatorGroup)
                            .SelectMany(i => i.DepartmentCategoryMapIndicatorGroups)
                            .Select(d => d.DepartmentCategory)
                            .SelectMany(d => d.Departments).Distinct();
                        foreach (var department in departmentCollection)
                        {
                            //根据下拉列表的选项，根据自动获取的系统名称，获取相关的值，再添加到数据库中
                            switch (sourceSystem.DataSourceSystemName)
                            {
                                case "病案管理系统":
                                    //从病案管理系统中获取值
                                    var bagl = new ImsAutoLib.Bagl.Bagl();
                                    value = bagl.GetIndicatorValue(department.DepartmentId, indicator.IndicatorId, searchTime.Value);
                                    break;
                                case "计算":
                                    //value = indicatorValue.GetDepartmentIndicatorValueByCalculate(department.DepartmentId, indicator.IndicatorId, searchTime.Value);

                                    //换新的“算法”函数
                                    value = ViewModels.Report.AggregateDepartmentIndicatorValueValue(new Models.ImsDbContext(), department.DepartmentId, indicator.IndicatorId, searchTime.Value, searchTime.Value);

                                    break;
                                case "超声影像系统":
                                    break;
                            }
                            //添加
                            await CreateDepartmentIndicatorList(searchTime, indicator, department, value);
                        }
                    }
                }
            }
            return RedirectToAction("Index", new { searchTime = searchTime, dataSourceSystemID = dataSourceSystemID });
        }

        // GET: AutoGetDataSource/Edit/5
        [Route("Edit/{id}/{time}/{dataSourceSystemIDs}")]

        public async Task<ActionResult> Edit(Guid? id, DateTime? time, Guid? dataSourceSystemID)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.dataSourceSystemID = dataSourceSystemID;
            //查找到该department       
            var department = await db.Departments.FindAsync(id.Value);
            DepartmentIndicatorCountView viewModel = new DepartmentIndicatorCountView();
            viewModel.DepartmentIndicatorValues = new List<DepartmentIndicatorValue>();
            viewModel.Department = department;
            //从DepartmentIndicatorValue找值
            viewModel.SearchTime = time;
            var departmentIndicatorValues = await db.Departments.SelectMany(c => c.DepartmentIndicatorValues).Include(d => d.Indicator.Duration)
                                                .Where(d => d.DepartmentId == department.DepartmentId
                                                && d.Time.Year == time.Value.Year && d.Time.Month == time.Value.Month
                                                && d.Indicator.DataSourceSystemId == dataSourceSystemID).OrderBy(d => d.Indicator.Priority).ToListAsync();
            foreach (var departmentIndicatorValue in departmentIndicatorValues)
            {
                viewModel.DepartmentIndicatorValues.Add(departmentIndicatorValue);
            }
            return View(viewModel);
        }

        // POST: AutoGetDataSource/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Department department, IEnumerable<DepartmentIndicatorValue> departmentIndicatorValues, DateTime? searchTime, Guid? dataSourceSystemID)
        {
            ViewBag.dataSourceSystemID = dataSourceSystemID;

            var viewModel = new DepartmentIndicatorCountView();
            if (department != null)
            {
                viewModel.Department = await db.Departments.FindAsync(department.DepartmentId);
            }
            viewModel.SearchTime = searchTime;
            viewModel.DepartmentIndicatorValues = new List<DepartmentIndicatorValue>();
            foreach (var departmentIdicatorValuequery in departmentIndicatorValues)
            {
                //保存值
                var departmentIdicatorValue = await db.DepartmentIndicatorValues
                                             .FindAsync(departmentIdicatorValuequery.DepartmentIndicatorValueId);
                try
                {
                    if (departmentIdicatorValuequery.Value != null &&
                        departmentIdicatorValue.Value != departmentIdicatorValuequery.Value
                        || departmentIdicatorValuequery.IsLocked != departmentIdicatorValue.IsLocked)
                    {
                        departmentIdicatorValue.Value = departmentIdicatorValuequery.Value;
                        departmentIdicatorValue.IsLocked = departmentIdicatorValuequery.IsLocked;
                        //database win
                        bool saveFailed;
                        do
                        {
                            saveFailed = false;
                            try
                            {
                                await db.SaveChangesAsync();
                            }
                            catch (DbUpdateConcurrencyException ex)
                            {
                                saveFailed = true;
                                // Update the values of the entity that failed to save from the store 
                                ex.Entries.Single().Reload();
                            }
                        } while (saveFailed);
                    }
                }
                catch (Exception)
                {

                    ModelState.AddModelError("", String.Format("无法更新指标{0}的值！", departmentIdicatorValue.Indicator.IndicatorName));
                }
                //}
                viewModel.DepartmentIndicatorValues.Add(departmentIdicatorValue);
            }
            return View(viewModel);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
