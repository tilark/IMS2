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
    public class AutoGetDataSourceController : Controller
    {
        private ImsDbContext db = new ImsDbContext();

        // GET: AutoGetDataSource
        [Route("Index/{searchTime}/{dataSourceSystemID}")]

        public async Task<ActionResult> Index(DateTime? searchTime, Guid? dataSourceSystemID)
        {
            ViewBag.dataSourceSystemID = new SelectList(db.DataSourceSystems.Distinct().OrderBy(d=>d.Priority), "DataSourceSystemId", "DataSourceSystemName");

            if(searchTime != null && dataSourceSystemID != null)
            {
                DataSourceSystemIndicatorView viewModel = new DataSourceSystemIndicatorView();
                var dataSourceSystem = await db.DataSourceSystems.FindAsync(dataSourceSystemID);
                if(dataSourceSystem != null)
                {
                    ViewBag.sourceSystemID = dataSourceSystem.DataSourceSystemId;
                    viewModel.dataSourceSystem = dataSourceSystem;
                    viewModel.searchTime = searchTime.Value;
                    viewModel.Indicators = new List<Indicator>();
                    viewModel.DepartmentIndicatorCountViews = new List<DepartmentIndicatorCountView>();
                    foreach(var indicator in dataSourceSystem.Indicators)
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
        private async Task<DepartmentIndicatorValue> CreateDepartmentIndicatorList(DateTime? searchTime, Indicator indicator, Department department, decimal value)
        {
            if (searchTime == null || indicator == null || department == null)
            {
                return null;
            }
            bool canAdd = false;
            switch (indicator.Duration.DurationName)
            {
                case "季":
                    if (searchTime.Value.Month == 3 || searchTime.Value.Month == 6 || searchTime.Value.Month == 9 || searchTime.Value.Month == 12)
                    {
                        canAdd = true;
                    }
                    break;
                case "半年":
                    if (searchTime.Value.Month == 6)
                    {
                        canAdd = true;
                    }
                    break;
                case "全年":
                    if (searchTime.Value.Month == 12)
                    {
                        canAdd = true;
                    }
                    break;
                default:
                    canAdd = true;
                    break;
            }
            if (canAdd)
            {

                DepartmentIndicatorValue departmentIndicatorValue = new DepartmentIndicatorValue();
                departmentIndicatorValue.DepartmentId = department.DepartmentId;
                departmentIndicatorValue.IndicatorId = indicator.IndicatorId;
                departmentIndicatorValue.DepartmentIndicatorValueId = System.Guid.NewGuid();
                departmentIndicatorValue.Time = searchTime.Value;
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
        [Route("Details/{id}/{time}/{dataSourceSystem}")]

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

        // GET: AutoGetDataSource/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName");
            ViewBag.IndicatorStandardId = new SelectList(db.DepartmentIndicatorStandards, "DepartmentIndicatorStandardId", "Remarks");
            ViewBag.IndicatorId = new SelectList(db.Indicators, "IndicatorId", "IndicatorName");
            return View();
        }

        // POST: AutoGetDataSource/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DateTime? searchTime, Guid? dataSourceSystemID)
        {
            if(searchTime != null && dataSourceSystemID != null)
            {
                var sourceSystem = await db.DataSourceSystems.FindAsync(dataSourceSystemID.Value);
                if (sourceSystem != null)
                {
                    Decimal value = 0;
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
                                    value = indicatorValue.GetDepartmentIndicatorValueByCalculate(department.DepartmentId, indicator.IndicatorId, searchTime.Value);

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
        [Route("Details/{id}/{time}/{dataSourceSystem}")]

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

        // GET: AutoGetDataSource/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DepartmentIndicatorValue departmentIndicatorValue = await db.DepartmentIndicatorValues.FindAsync(id);
            if (departmentIndicatorValue == null)
            {
                return HttpNotFound();
            }
            return View(departmentIndicatorValue);
        }

        // POST: AutoGetDataSource/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            DepartmentIndicatorValue departmentIndicatorValue = await db.DepartmentIndicatorValues.FindAsync(id);
            db.DepartmentIndicatorValues.Remove(departmentIndicatorValue);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
