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
using System.Data.Entity.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace IMS2.Controllers
{
    [Authorize(Roles = "修改全院指标值,修改科室指标值, Administrators")]
    [RoutePrefix("providingdepartmentindicator")]
    public class ProvidingDepartmentIndicatorController : Controller
    {
        private ImsDbContext db = new ImsDbContext();
        [Route("")]
        [Route("indexajax")]
        public async Task<ActionResult> IndexAjax()
        {
            if (User.IsInRole("修改全院指标值") || User.IsInRole("Administrators"))
            {
                ViewBag.providingDepartmentID = new SelectList(db.Indicators.Select(i => i.ProvidingDepartment).Distinct().OrderBy(d => d.Priority), "DepartmentId", "DepartmentName");
            }
            else
            {
                //应该选择提供科室名列表，根据成员角色中的科室选择，如果权限为“创建指标值”，可获取全部科室信息
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    using (UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context)))
                    {
                        var user = await userManager.FindByIdAsync(User.Identity.GetUserId());
                        ViewBag.providingDepartmentID = new SelectList(user.UserInfo.UserDepartments, "UserDepartmentId", "UserDepartmentName");
                    }

                }
            }
            return View();

        }
        [Route("IndexAjax/{searchTime:datetime}/{providingDepartmentID:guid}")]
        public async Task<ActionResult> IndexAjax(DateTime searchTime, Guid providingDepartmentID)
        {
            var provideDepartment = await db.Departments.FindAsync(providingDepartmentID);
            var providingDepartmentView = new ProvidingDepartmentView();

            if (provideDepartment != null)
            {
                providingDepartmentView.SearchTime = searchTime;
                providingDepartmentView.ProvidingDepartmentID = providingDepartmentID;
                providingDepartmentView.ProvidingDepartmentName = provideDepartment.DepartmentName;
                providingDepartmentView.SearchDepartmentIndicatorViews = await GetSearchDepartmentIndicatorViewList(searchTime, providingDepartmentID);
                providingDepartmentView.IndicatorDurationViews = new List<IndicatorDurationView>();
                foreach (var name in provideDepartment.ProvidingIndicators)
                {
                    var item = new IndicatorDurationView
                    {
                        IndicatorName = name.IndicatorName,
                        DurationName = name.Duration.DurationName
                    };
                    providingDepartmentView.IndicatorDurationViews.Add(item);
                }
            }
            return View(providingDepartmentView);

        }


        public async Task<ActionResult> SearchIndicator(DateTime searchTime, Guid providingDepartmentID)
        {
            if ( !providingDepartmentID.Equals(Guid.Empty))
            {

                ViewBag.ProvideDepartmentID = providingDepartmentID;
                ViewBag.SearchTime = searchTime;
                var viewModel = await GetSearchDepartmentIndicatorViewList(searchTime, providingDepartmentID);
                //填充ViewModel
                return PartialView("_searchIndicator", viewModel);
            }
            return PartialView("_searchIndicator", null);
        }

        private async Task<List<SearchDepartmentIndicatorView>> GetSearchDepartmentIndicatorViewList(DateTime? searchTime, Guid providingDepartmentID)
        {
            var viewModel = new List<SearchDepartmentIndicatorView>();

            var provideDepartment = await db.Departments.FindAsync(providingDepartmentID);
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
                        var view = new SearchDepartmentIndicatorView();

                        //需查看这个department是否在viewModel.DepartmentIndicatorCountViews中
                        var query = viewModel.Where(d => d.DepartmentID == department.DepartmentId).FirstOrDefault();
                        if (query != null)
                        {
                            continue;
                        }
                        view.DepartmentName = department.DepartmentName;
                        view.DepartmentID = department.DepartmentId;
                        //只显示
                        view.IndicatorCount = await db.DepartmentIndicatorValues.Where(d => d.Indicator.ProvidingDepartmentId == provideDepartment.DepartmentId
                                                               && d.DepartmentId == department.DepartmentId
                                                               && d.Time.Year == searchTime.Value.Year
                                                              && d.Time.Month == searchTime.Value.Month).CountAsync();
                        view.HasValueCount = await db.DepartmentIndicatorValues.Where(d => d.Indicator.ProvidingDepartmentId == provideDepartment.DepartmentId
                                                                && d.DepartmentId == department.DepartmentId
                                                                && d.Time.Year == searchTime.Value.Year
                                                               && d.Time.Month == searchTime.Value.Month
                                                               && d.Value.HasValue).CountAsync();
                        viewModel.Add(view);
                    }
                }
            }
            return viewModel;
        }
        public ActionResult Message()
        {
            ViewBag.Message = "This is a partial view";
            return PartialView();
        }

        public async Task<ActionResult> IndicatorDetails(string providingDepartmentID)
        {
            if (String.IsNullOrWhiteSpace(providingDepartmentID))
            {
                ViewBag.ProvidingDepartmentName = "空";
                ViewBag.IndicatorDetails = new List<string>();
                return PartialView("_indicatorDetails");

            }
            try
            {
                Guid provideDepartmentID;
                if (Guid.TryParse(providingDepartmentID, out provideDepartmentID))
                {
                    var provideDepartment = await db.Departments.FindAsync(provideDepartmentID);
                    ViewBag.ProvidingDepartmentName = provideDepartment.DepartmentName;
                    ViewBag.IndicatorDetails = provideDepartment.ProvidingIndicators.Select(i => i.IndicatorName).ToList();
                }
            }
            catch (Exception)
            {
            }
            return PartialView("_indicatorDetails");

        }
        // GET: ProvidingDepartmentIndicator
        //[Route("Index/{searchTime}/{providingDepartment}")]

        public async Task<ActionResult> Index(DateTime? searchTime, Guid? providingDepartment)
        {
            if (User.IsInRole("修改全院指标值") || User.IsInRole("Administrators"))
            {
                //ViewBag.providingDepartment = new SelectList(db.Departments.Where(d => d.ProvidingIndicators != null).OrderBy(d => d.Priority), "DepartmentId", "DepartmentName");
                ViewBag.providingDepartment = new SelectList(db.Indicators.Select(i => i.ProvidingDepartment).Distinct().OrderBy(d => d.Priority), "DepartmentId", "DepartmentName");


            }
            else
            {
                //应该选择提供科室名列表，根据成员角色中的科室选择，如果权限为“创建指标值”，可获取全部科室信息
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    using (UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context)))
                    {
                        var user = await userManager.FindByIdAsync(User.Identity.GetUserId());
                        ViewBag.providingDepartment = new SelectList(user.UserInfo.UserDepartments, "UserDepartmentId", "UserDepartmentName");
                    }

                }
            }

            if (searchTime != null && providingDepartment != null)
            {
                ProvideDepartmentIndicatorView viewModel = new ProvideDepartmentIndicatorView();

                //填充ViewModel
                var provideDepartment = await db.Departments.FindAsync(providingDepartment);
                if (provideDepartment != null)
                {
                    viewModel.provideDepartment = provideDepartment;
                    viewModel.searchTime = searchTime.Value;
                    viewModel.Indicators = new List<Indicator>();
                    viewModel.DepartmentIndicatorCountViews = new List<DepartmentIndicatorCountView>();
                    //数据来源科室负责的指标，根据指标与指定时间是否在科室指标值表（简称值表）中，如果不在，依次选择指标，追寻到科室类别项目组中的各个科室，再将该指标、科室、时间写入值表中。
                    ViewBag.departmentID = provideDepartment.DepartmentId;

                    foreach (var indicator in provideDepartment.ProvidingIndicators)
                    {
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
                            view.IndicatorCount = await db.DepartmentIndicatorValues.Where(d => d.Indicator.ProvidingDepartmentId == provideDepartment.DepartmentId
                                                                   && d.DepartmentId == department.DepartmentId
                                                                   && d.Time.Year == searchTime.Value.Year
                                                                  && d.Time.Month == searchTime.Value.Month).CountAsync();
                            view.HasValueCount = await db.DepartmentIndicatorValues.Where(d => d.Indicator.ProvidingDepartmentId == provideDepartment.DepartmentId
                                                                    && d.DepartmentId == department.DepartmentId
                                                                    && d.Time.Year == searchTime.Value.Year
                                                                   && d.Time.Month == searchTime.Value.Month
                                                                   && d.Value.HasValue).CountAsync();
                            view.SearchTime = searchTime;
                            viewModel.DepartmentIndicatorCountViews.Add(view);
                        }
                    }
                    return View(viewModel);
                }
            }
            return View();
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

        // GET: ProvidingDepartmentIndicator/Details/5
        [Route("Details/{id:guid}/{time:datetime}/{provideDepartment:guid}")]
        public async Task<ActionResult> Details(Guid id, DateTime time, Guid provideDepartment)
        {
            if (id.Equals(Guid.Empty))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.provideDepartment = provideDepartment;
            //查找到该department       
            var department = await db.Departments.FindAsync(id);
            DepartmentIndicatorCountView viewModel = new DepartmentIndicatorCountView();
            viewModel.DepartmentIndicatorValues = new List<DepartmentIndicatorValue>();
            viewModel.Department = department;
            //从DepartmentIndicatorValue找值
            viewModel.SearchTime = time;
            var departmentIndicatorValues = await db.Departments.SelectMany(c => c.DepartmentIndicatorValues).Include(d => d.Indicator.Duration)
                                                .Where(d => d.DepartmentId == department.DepartmentId
                                                && d.Time.Year == time.Year && d.Time.Month == time.Month
                                                && d.Indicator.ProvidingDepartmentId == provideDepartment).OrderBy(d => d.Indicator.Priority).ToListAsync();
            foreach (var departmentIndicatorValue in departmentIndicatorValues)
            {
                viewModel.DepartmentIndicatorValues.Add(departmentIndicatorValue);
            }
            return View(viewModel);
        }
        //public async Task<ActionResult> Details(Guid? id, DateTime? time, Guid? provideDepartment)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ViewBag.provideDepartment = provideDepartment;
        //    //查找到该department       
        //    var department = await db.Departments.FindAsync(id.Value);
        //    DepartmentIndicatorCountView viewModel = new DepartmentIndicatorCountView();
        //    viewModel.DepartmentIndicatorValues = new List<DepartmentIndicatorValue>();
        //    viewModel.Department = department;
        //    //从DepartmentIndicatorValue找值
        //    viewModel.SearchTime = time;
        //    var departmentIndicatorValues = await db.Departments.SelectMany(c => c.DepartmentIndicatorValues).Include(d => d.Indicator.Duration)
        //                                        .Where(d => d.DepartmentId == department.DepartmentId
        //                                        && d.Time.Year == time.Value.Year && d.Time.Month == time.Value.Month
        //                                        && d.Indicator.ProvidingDepartmentId == provideDepartment).OrderBy(d => d.Indicator.Priority).ToListAsync();
        //    foreach (var departmentIndicatorValue in departmentIndicatorValues)
        //    {
        //        viewModel.DepartmentIndicatorValues.Add(departmentIndicatorValue);
        //    }
        //    return View(viewModel);
        //}

        //[Authorize(Roles = "创建指标值, Administrators")]
        [HttpPost]
        //[ValidateAntiForgeryToken]

        public async Task<ActionResult> CreateIndicators(DateTime? searchTime, Guid? providingDepartmentID)
        {

            if (searchTime.HasValue && !providingDepartmentID.Value.Equals(Guid.Empty))
            {
                ViewBag.ProvideDepartmentID = providingDepartmentID.Value;
                ViewBag.SearchTime = searchTime.Value;
                var provideDepartment = await db.Departments.FindAsync(providingDepartmentID);
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
                var viewModel = await GetSearchDepartmentIndicatorViewList(searchTime, providingDepartmentID.Value);
                //return RedirectToAction("SearchIndicator", new { searchTime = searchTime, providingDepartmentID = providingDepartmentID });
                return PartialView("_searchIndicator", viewModel);
            }
            return PartialView("_searchIndicator", null);
        }
        // POST: ProvidingDepartmentIndicator/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [Authorize(Roles = "创建指标值, Administrators")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create/{searchTime}/{departmentID}")]

        public async Task<ActionResult> Create(DateTime? searchTime, Guid? departmentID)
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
            return RedirectToAction("Index", new { searchTime = searchTime, providingDepartment = departmentID });
        }
        // GET: ProvidingDepartmentIndicator/Edit/5
        [Route("Edit/{id}/{time}/{provideDepartment}")]

        public async Task<ActionResult> Edit(Guid? id, DateTime? time, Guid? provideDepartment)
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
            return View(viewModel);
        }

        // POST: ProvidingDepartmentIndicator/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Department department, IEnumerable<DepartmentIndicatorValue> departmentIndicatorValues, DateTime? searchTime, Guid? provideDepartment)
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
                //if (TryUpdateModel(departmentIdicatorValue, "", new string[] { "Value" }))
                //{
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
