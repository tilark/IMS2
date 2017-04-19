using IMS2.Models;
using IMS2.ViewModels;
using IMS2.ViewModels.ProvidingDepartmentIndicatorView;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS2.Controllers
{
    public class ProvidingDepartmentIndicatorAjaxController : Controller
    {
        private ImsDbContext db = new ImsDbContext();
        // GET: ProvidingDepartmentIndicatorAjax
        public ActionResult Index()
        {
            PoPulateDepartmentList();
            var viewModel = new SearchCondition();
            viewModel.SearchTime = System.DateTime.Now.AddMonths(-1);
            return View(viewModel);
        }


        public async Task<ActionResult> _List(SearchCondition searchCondition)
        {
            ViewBag.SearchTime = searchCondition.SearchTime;
            ViewBag.DepartmentID = searchCondition.DepartmentId;
            if (TryUpdateModel(searchCondition))
            {
                var viewModel = new List<DepartmentIndicatorStatus>();
                var provideDepartment = await db.Departments.Where(a => a.DepartmentId == searchCondition.DepartmentId).Include(a => a.ProvidingIndicators).FirstOrDefaultAsync();
                if (provideDepartment != null)
                {
                    foreach (var indicator in provideDepartment.ProvidingIndicators)
                    {
                        var departmentCollection = indicator.IndicatorGroupMapIndicators.Select(i => i.IndicatorGroup)
                            .SelectMany(i => i.DepartmentCategoryMapIndicatorGroups)
                            .Select(d => d.DepartmentCategory)
                            .SelectMany(d => d.Departments).Distinct();

                        foreach (var department in departmentCollection)
                        {
                            var query = viewModel.Where(d => d.DepartmentName == department.DepartmentName).FirstOrDefault();
                            if (query != null)
                            {
                                continue;
                            }
                            var view = new DepartmentIndicatorStatus();
                            //view.Department = department;
                            //只显示
                            view.DepartmentID = department.DepartmentId;
                            view.DepartmentName = department.DepartmentName;
                            view.IndicatorCount = await db.DepartmentIndicatorValues.Where(d => d.Indicator.ProvidingDepartmentId == provideDepartment.DepartmentId
                                                                   && d.DepartmentId == department.DepartmentId
                                                                   && d.Time.Year == searchCondition.SearchTime.Year
                                                                  && d.Time.Month == searchCondition.SearchTime.Month).CountAsync();
                            view.HasValueCount = await db.DepartmentIndicatorValues.Where(d => d.Indicator.ProvidingDepartmentId == provideDepartment.DepartmentId
                                                                    && d.DepartmentId == department.DepartmentId
                                                                    && d.Time.Year == searchCondition.SearchTime.Year
                                                                   && d.Time.Month == searchCondition.SearchTime.Month
                                                                   && d.Value.HasValue).CountAsync();
                            view.SearchTime = searchCondition.SearchTime;
                            //viewModel.DepartmentIndicatorCountViews.Add(view);
                            viewModel.Add(view);
                        }
                    }
                }
                return PartialView("_List", viewModel);
            }
            else
            {
                return new EmptyResult();
            }
        }

        #region 创建
        // POST: ProvidingDepartmentIndicator/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [Authorize(Roles = "创建指标值, Administrators")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> _Create(DateTime searchTime, Guid departmentID)
        {
            var provideDepartment = await db.Departments.FindAsync(departmentID);
            if (provideDepartment != null)
            {
                //数据来源科室负责的指标，根据指标与指定时间是否在科室指标值表（简称值表）中，如果不在，依次选择指标，追寻到科室类别项目组中的各个科室，再将该指标、科室、时间写入值表中。
                foreach (var indicator in provideDepartment.ProvidingIndicators)
                {

                    var departmentCollection = indicator.IndicatorGroupMapIndicators.Select(i => i.IndicatorGroup)
                        .SelectMany(i => i.DepartmentCategoryMapIndicatorGroups)
                        .Select(d => d.DepartmentCategory)
                        .SelectMany(d => d.Departments).Distinct();
                    foreach (var department in departmentCollection)
                    {
                        //添加
                        await CreateDepartmentIndicatorList(searchTime, indicator, department);
                    }
                }
            }
            var searchCondition = new SearchCondition { DepartmentId = departmentID, SearchTime = searchTime };
            return await _List(searchCondition);
        }
        private async Task<DepartmentIndicatorValue> CreateDepartmentIndicatorList(DateTime? searchTime, Indicator indicator, Department department)
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
                departmentIndicatorValue.IsLocked = false;
                departmentIndicatorValue.UpdateTime = DateTime.Now;
                //将科室、项目、时间添加到科室值表中
                return await AddDepartmentIndicatorValue(departmentIndicatorValue);
            }
            return null;
        }
        private async Task<DepartmentIndicatorValue> AddDepartmentIndicatorValue(DepartmentIndicatorValue departmentIndicatorValue)
        {
            DepartmentIndicatorValue item = null;
            if (departmentIndicatorValue == null)
            {
                return null;
            }
            //查重
            item = db.DepartmentIndicatorValues.Where(d => d.DepartmentId == departmentIndicatorValue.DepartmentId
                            && d.IndicatorId == departmentIndicatorValue.IndicatorId
                            && d.Time.Year == departmentIndicatorValue.Time.Year && d.Time.Month == departmentIndicatorValue.Time.Month)
                            .FirstOrDefault();
            if (item == null)
            {
                item = departmentIndicatorValue;
                db.DepartmentIndicatorValues.Add(item);
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
            }
            return item;
        }

        #endregion
        #region 编辑
        public async Task<ActionResult> _Edit(Guid? id, DateTime? time, Guid? provideDepartment)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.provideDepartment = provideDepartment;
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
                                                && d.Indicator.ProvidingDepartmentId == provideDepartment).OrderBy(d => d.Indicator.Priority).ToListAsync();
            foreach (var departmentIndicatorValue in departmentIndicatorValues)
            {
                viewModel.DepartmentIndicatorValues.Add(departmentIndicatorValue);
            }
            return PartialView("_Edit", viewModel);
        }

        // POST: ProvidingDepartmentIndicator/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> _Edit(Department department, IEnumerable<DepartmentIndicatorValue> departmentIndicatorValues, DateTime? searchTime, Guid? provideDepartment)
        {
            ViewBag.provideDepartment = provideDepartment;

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
                if (TryUpdateModel(departmentIdicatorValue))
                {
                    try
                    {
                        if (!departmentIdicatorValue.IsLocked)
                        {

                            if (departmentIdicatorValue.Value != departmentIdicatorValuequery.Value)
                            {
                                departmentIdicatorValue.Value = departmentIdicatorValuequery.Value;
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
                        viewModel.DepartmentIndicatorValues.Add(departmentIdicatorValue);

                    }
                    catch (Exception)
                    {

                        ModelState.AddModelError("", String.Format("无法更新指标{0}的值！", departmentIdicatorValue.Indicator.IndicatorName));
                        Response.StatusCode = (int)HttpStatusCode.NonAuthoritativeInformation;
                    }
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NonAuthoritativeInformation;
                    return RedirectToAction("_Edit", new { id = department.DepartmentId, time = searchTime, provideDepartment = provideDepartment });
                    //break;
                }
            }
            return PartialView("_Edit", viewModel);
        }
        #endregion


        private void PoPulateDepartmentList()
        {
            //db.Indicators.Select(i => i.ProvidingDepartment).Distinct().OrderBy(d => d.Priority), "DepartmentId", "DepartmentName"
            ViewBag.DepartmentSelect = new SelectList(this.db.Indicators.Select(i => i.ProvidingDepartment).Distinct().OrderBy(d => d.Priority).Select(a => new SelectListItem { Text = a.DepartmentName, Value = a.DepartmentId.ToString() }), "Value", "Text");
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