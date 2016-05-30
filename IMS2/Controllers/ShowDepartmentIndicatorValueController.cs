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
using Newtonsoft.Json;
namespace IMS2.Controllers
{
    [Authorize(Roles = "查看科室指标值,查看全院指标值, Administrators")]

    public class ShowDepartmentIndicatorValueController : Controller
    {
        private ImsDbContext db = new ImsDbContext();

        public async Task<ActionResult> IndexAjax()
        {
            await InitialDepartment();
            return View();
        }

        private async Task InitialDepartment()
        {
            if (User.IsInRole("查看全院指标值") || User.IsInRole("Administrators"))
            {
                ViewBag.department = new SelectList(db.Departments.OrderBy(d => d.Priority), "DepartmentId", "DepartmentName");

            }
            else
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    using (UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context)))
                    {
                        var user = await userManager.FindByIdAsync(User.Identity.GetUserId());
                        ViewBag.department = new SelectList(user.UserInfo.UserDepartments, "UserDepartmentId", "UserDepartmentName");
                    }

                }
            }
        }
        public async Task<ActionResult> IndexJson()
        {
            await InitialDepartment();
            return View();
        }
        public async Task<ActionResult> ValueSearchJson(DateTime? searchTime, Guid? department)
        {
            //var view =  JsonConvert.DeserializeObject<DepartmentIndicatorValueView>(json);
            if (searchTime != null && department != null)
            {
                var departments = await db.Departments.FindAsync(department.Value);
                if (departments != null)
                {
                    DepartmentIndicatorCountView viewModel = new DepartmentIndicatorCountView();
                    viewModel.Department = departments;
                    viewModel.SearchTime = searchTime;
                    var departmentIndicatorValues = await db.DepartmentIndicatorValues.Where(d => d.DepartmentId == departments.DepartmentId
                                                    && d.Time.Year == searchTime.Value.Year
                                                    && d.Time.Month == searchTime.Value.Month).OrderBy(d => d.Indicator.Priority).ToListAsync();
                    viewModel.DepartmentIndicatorValues = departmentIndicatorValues;
                    //return View(viewModel);
                    return PartialView("_valueView", viewModel);
                }
            }
            await InitialDepartment();

            return View();
        }
        public async Task<ActionResult> ValueSearch(DateTime? searchTime, Guid? department)
        {
            //应该选择提供科室名列表，根据成员角色中的科室选择，如果权限为“创建指标值”，可获取全部科室信息
            if (searchTime != null && department != null)
            {
                var departments = await db.Departments.FindAsync(department.Value);
                if (departments != null)
                {
                    DepartmentIndicatorCountView viewModel = new DepartmentIndicatorCountView();
                    viewModel.Department = departments;
                    viewModel.SearchTime = searchTime;
                    var departmentIndicatorValues = await db.DepartmentIndicatorValues.Where(d => d.DepartmentId == departments.DepartmentId
                                                    && d.Time.Year == searchTime.Value.Year
                                                    && d.Time.Month == searchTime.Value.Month).OrderBy(d => d.Indicator.Priority).ToListAsync();
                    viewModel.DepartmentIndicatorValues = departmentIndicatorValues;
                    //return View(viewModel);
                    return PartialView("_valueView", viewModel);
                }
            }
            await InitialDepartment();
            return View();
        }
        // GET: DepartmentUpdateIndicatorValue
        [Route("Index/{searchTime}/{providingDepartment}")]

        public async Task<ActionResult> Index(DateTime? searchTime, Guid? department)
        {
            if(User.IsInRole("查看全院指标值") || User.IsInRole("Administrators"))
            {
                ViewBag.department = new SelectList(db.Departments.OrderBy(d => d.Priority), "DepartmentId", "DepartmentName");

            }
            else
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    using (UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context)))
                    {
                        var user = await userManager.FindByIdAsync(User.Identity.GetUserId());
                        ViewBag.department = new SelectList(user.UserInfo.UserDepartments, "UserDepartmentId", "UserDepartmentName");
                    }

                }
            }
            //应该选择提供科室名列表，根据成员角色中的科室选择，如果权限为“创建指标值”，可获取全部科室信息

            if (searchTime != null && department != null)
            {
                var departments = await db.Departments.FindAsync(department.Value);
                if(departments != null)
                {
                    DepartmentIndicatorCountView viewModel = new DepartmentIndicatorCountView();
                    viewModel.Department = departments;
                    viewModel.SearchTime = searchTime;
                    //viewModel.DepartmentIndicatorValues = new List<DepartmentIndicatorValue>();
                    var departmentIndicatorValues = await db.DepartmentIndicatorValues.Where(d => d.DepartmentId == departments.DepartmentId
                                                    && d.Time.Year == searchTime.Value.Year
                                                    && d.Time.Month == searchTime.Value.Month).OrderBy(d => d.Indicator.Priority).ToListAsync() ;
                    viewModel.DepartmentIndicatorValues = departmentIndicatorValues;
                    return View(viewModel);
                }
            }
            return View();
        }

        // POST: DepartmentUpdateIndicatorValue/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit(Department department, IEnumerable<DepartmentIndicatorValue> departmentIndicatorValues, DateTime? searchTime)
        //{
        //    var viewModel = new DepartmentIndicatorCountView();
        //    if (department != null)
        //    {
        //        viewModel.Department = await db.Departments.FindAsync(department.DepartmentId);
        //    }
        //    viewModel.SearchTime = searchTime;
        //    viewModel.DepartmentIndicatorValues = new List<DepartmentIndicatorValue>();
        //    foreach (var departmentIdicatorValueQuery in departmentIndicatorValues)
        //    {
        //        //保存值
        //        var departmentIdicatorValue = await db.DepartmentIndicatorValues
        //                                     .FindAsync(departmentIdicatorValueQuery.DepartmentIndicatorValueId);
        //        try
        //        {
        //            if (departmentIdicatorValueQuery.Value != null &&
        //                departmentIdicatorValue.Value != departmentIdicatorValueQuery.Value)
        //            {
        //                departmentIdicatorValue.Value = departmentIdicatorValueQuery.Value;
        //                //database win
        //                bool saveFailed;
        //                do
        //                {
        //                    saveFailed = false;
        //                    try
        //                    {
        //                        await db.SaveChangesAsync();
        //                    }
        //                    catch (DbUpdateConcurrencyException ex)
        //                    {
        //                        saveFailed = true;
        //                        // Update the values of the entity that failed to save from the store 
        //                        ex.Entries.Single().Reload();
        //                    }
        //                } while (saveFailed);
        //            }
        //        }
        //        catch (Exception)
        //        {

        //            ModelState.AddModelError("", String.Format("无法更新指标{0}的值！", departmentIdicatorValue.Indicator.IndicatorName));
        //        }
        //        //}
        //        viewModel.DepartmentIndicatorValues.Add(departmentIdicatorValue);
        //    }
        //    return RedirectToAction("Index", new { searchTime = searchTime, departmentID = department.DepartmentId });
        //}

       

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
