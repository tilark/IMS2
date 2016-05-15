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
    public class ManageUserController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ManageUser
        public async Task<ActionResult> Index()
        {
            var viewModel = new List<ManageUserViewModel>();
            var userInfos = await db.UserInfos.Include(u => u.UserDepartments).ToListAsync();
            foreach (var userInfo in userInfos)
            {
                var manageUser = new ManageUserViewModel
                {
                    UserInfoID = userInfo.UserInfoID,
                    UserName = userInfo.UserName,
                    EmployeeNo = userInfo.EmployeeNo,
                    Sex = userInfo.Sex,
                    WorkPhone = userInfo.WorkPhone,
                    HomePhone = userInfo.HomePhone
                };
                manageUser.UserDepartments = userInfo.UserDepartments.ToList();
                viewModel.Add(manageUser);
            }
            return View(viewModel);
        }

        // GET: ManageUser/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userInfo = await db.UserInfos.FindAsync(id.Value);
            var viewModel = new ManageUserViewModel
            {
                UserInfoID = userInfo.UserInfoID,
                UserName = userInfo.UserName,
                EmployeeNo = userInfo.EmployeeNo,
                Sex = userInfo.Sex,
                WorkPhone = userInfo.WorkPhone,
                HomePhone = userInfo.HomePhone
            };
            viewModel.UserDepartments = userInfo.UserDepartments.ToList();
            PopulateAssignedDepartmentData(userInfo);

            return View(viewModel);
        }
        private void InitialAssignedDepartmentData()
        {
            var allDepartments = new List<UserDepartment>();
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                allDepartments = context.UserDepartments.OrderBy(d => d.UserDepartmentName).ToList();
            }
            var assignedUserDepartmentData = new List<AssignedUserDepartmentData>();
            foreach (var department in allDepartments)
            {
                assignedUserDepartmentData.Add(new AssignedUserDepartmentData
                {
                    UserDepartmentId = department.UserDepartmentId,
                    UserDepartmentName = department.UserDepartmentName,
                    Assigned = false
                });
            }

            ViewBag.UserDepartments = assignedUserDepartmentData;
        }
        private void PopulateAssignedDepartmentData(UserInfo userInfo)
        {
            var allDepartments = new List<UserDepartment>();
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                allDepartments = context.UserDepartments.OrderBy(d => d.UserDepartmentName).ToList();
            }
            var userInfoDepartments = new HashSet<Guid>(userInfo.UserDepartments.Select(u => u.UserDepartmentId));
            var viewModel = new List<AssignedUserDepartmentData>();
            foreach (var department in allDepartments)
            {
                viewModel.Add(new AssignedUserDepartmentData
                {
                    UserDepartmentId = department.UserDepartmentId,
                    UserDepartmentName = department.UserDepartmentName,
                    Assigned = userInfoDepartments.Contains(department.UserDepartmentId)
                });
            }

            ViewBag.UserDepartments = viewModel;
        }
        // GET: ManageUser/Create
        public ActionResult Create()
        {

            InitialAssignedDepartmentData();
            return View();
        }

        // POST: ManageUser/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        //// 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel model, string[] selectedDepartment)
        {
            if (ModelState.IsValid)
            {
                using (UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db)))
                {
                    var user = new ApplicationUser { UserName = model.EmployeeNo, Email = model.Email };
                    var userInfo = new UserInfo { UserInfoID = System.Guid.NewGuid(), UserName = model.UserName, WorkPhone = model.WorkPhone, EmployeeNo = model.EmployeeNo };
                    userInfo.UserDepartments = new List<UserDepartment>();
                    await AddUserDepartments(selectedDepartment, userInfo);

                    db.UserInfos.Add(userInfo);
                    int num = await db.SaveChangesAsync();
                    if (num <= 0)
                    {
                        InitialAssignedDepartmentData();
                        return View(model);
                    }
                    user.UserInfoID = userInfo.UserInfoID;
                    var result = await userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        //没成功，需要删除userInfo
                        var userInfoDel = await db.UserInfos.FindAsync(userInfo.UserInfoID);
                        db.UserInfos.Remove(userInfoDel);
                        await db.SaveChangesAsync();

                    }
                }
            }
            InitialAssignedDepartmentData();
            return View(model);
        }



        // GET: ManageUser/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userInfo = await db.UserInfos.FindAsync(id.Value);
            var viewModel = new ManageUserViewModel
            {
                UserInfoID = userInfo.UserInfoID,
                UserName = userInfo.UserName,
                EmployeeNo = userInfo.EmployeeNo,
                Sex = userInfo.Sex,
                WorkPhone = userInfo.WorkPhone,
                HomePhone = userInfo.HomePhone
            };
            PopulateAssignedDepartmentData(userInfo);

            return View(viewModel);
        }

        // POST: ManageUser/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        //// 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ManageUserViewModel manageUserViewModel, string[] selectedDepartment)
        {
            var userInfo = await db.UserInfos.FindAsync(manageUserViewModel.UserInfoID);

            if (ModelState.IsValid)
            {
                userInfo.UserName = manageUserViewModel.UserName;
                userInfo.WorkPhone = manageUserViewModel.WorkPhone;
                userInfo.HomePhone = manageUserViewModel.HomePhone;
                userInfo.Sex = manageUserViewModel.Sex;
                await UpdateUserDepartments(selectedDepartment, userInfo);

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
            PopulateAssignedDepartmentData(userInfo);

            return View(manageUserViewModel);
        }
        private async Task AddUserDepartments(string[] selectedDepartment, UserInfo userInfoToAdd)
        {
            if (selectedDepartment == null)
            {
                userInfoToAdd.UserDepartments = new List<UserDepartment>();
                return;
            }
            var selectedDepartmentsHS = new HashSet<string>(selectedDepartment);
            var allDepartments = await db.UserDepartments.ToListAsync();
            foreach (var department in allDepartments)
            {
                if (selectedDepartmentsHS.Contains(department.UserDepartmentId.ToString()))
                {
                    userInfoToAdd.UserDepartments.Add(department);
                }

            }
        }

        private async Task UpdateUserDepartments(string[] selectedDepartment, UserInfo userInfoToUpdate)
        {
            if (selectedDepartment == null)
            {
                userInfoToUpdate.UserDepartments = new List<UserDepartment>();
                return;
            }
            var selectedDepartmentsHS = new HashSet<string>(selectedDepartment);
            var userDepartments = new HashSet<Guid>
                (userInfoToUpdate.UserDepartments.Select(u => u.UserDepartmentId));
            var allDepartments = await db.UserDepartments.ToListAsync();
            foreach (var department in allDepartments)
            {
                if (selectedDepartmentsHS.Contains(department.UserDepartmentId.ToString()))
                {
                    if (!userDepartments.Contains(department.UserDepartmentId))
                    {
                        userInfoToUpdate.UserDepartments.Add(department);
                    }
                }
                else
                {
                    if (userDepartments.Contains(department.UserDepartmentId))
                    {
                        userInfoToUpdate.UserDepartments.Remove(department);
                    }
                }
            }
        }

        // GET: ManageUser/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userInfo = await db.UserInfos.FindAsync(id.Value);
            var viewModel = new ManageUserViewModel
            {
                UserInfoID = userInfo.UserInfoID,
                UserName = userInfo.UserName,
                EmployeeNo = userInfo.EmployeeNo,
                Sex = userInfo.Sex,
                WorkPhone = userInfo.WorkPhone,
                HomePhone = userInfo.HomePhone
            };
            viewModel.UserDepartments = userInfo.UserDepartments.ToList();

            return View(viewModel);
        }

        //// POST: ManageUser/Delete/5        
        /// <summary>
        /// 删除用户信息时需同时删除用户登陆信息，需用到UserManage.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;ActionResult&gt;.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            using (UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db)))
            {
                var userInfo = await db.UserInfos.FindAsync(id);
                var user = await db.Users.Where(u => u.UserInfoID == id).FirstAsync();

                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    db.UserInfos.Remove(userInfo);
                    await db.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
