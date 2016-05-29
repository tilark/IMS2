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
    [Authorize(Roles = "修改全院人员信息, Administrators")]

    public class ManageUserController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ManageUser
        public async Task<ActionResult> Index(IMSMessageIdEnum? message)
        {
            ViewBag.StatusMessage =
             message == IMSMessageIdEnum.CreateSuccess ? "已创建新项。"
             : message == IMSMessageIdEnum.EditdSuccess ? "已更新完成。"
             : message == IMSMessageIdEnum.DeleteSuccess ? "已删除成功。"
             : message == IMSMessageIdEnum.CreateError ? "创建项目出现错误。"
             : message == IMSMessageIdEnum.EditError ? "有重名，无法更新相关信息。"
             : message == IMSMessageIdEnum.DeleteError ? "不允许删除该项。"
             : "";
            var viewModel = new List<ManageUserViewModel>();
            var userInfos = await db.UserInfos.Include(u => u.UserDepartments).ToListAsync();
            var users = await db.Users.Include(c => c.UserInfo).Where(u => u.UserName != "Administrators").ToListAsync();

            foreach (var user in users)
            {
                var manageUser = new ManageUserViewModel
                {
                    UserID = user.Id,
                    UserInfoID = user.UserInfo.UserInfoID,
                    UserName = user.UserInfo.UserName,
                    EmployeeNo = user.UserInfo.EmployeeNo,
                    Sex = user.UserInfo.Sex,
                    WorkPhone = user.UserInfo.WorkPhone,
                    HomePhone = user.UserInfo.HomePhone
                };
                manageUser.UserDepartments = user.UserInfo.UserDepartments.ToList();
                manageUser.RoleViews = new List<RoleView>();
                using (UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db)))
                {
                    foreach (var role in userManager.GetRoles(user.Id))
                    {
                        RoleView roleView = new RoleView();
                        roleView.RoleName = role;
                        manageUser.RoleViews.Add(roleView);
                    }
                }
                viewModel.Add(manageUser);
            }
            return View(viewModel);
        }

        // GET: ManageUser/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //var userInfo = await db.UserInfos.FindAsync(id.Value);
            var user = db.Users.Find(id);

            var viewModel = new ManageUserViewModel
            {
                UserInfoID = user.UserInfo.UserInfoID,
                UserName = user.UserInfo.UserName,
                EmployeeNo = user.UserInfo.EmployeeNo,
                Sex = user.UserInfo.Sex,
                WorkPhone = user.UserInfo.WorkPhone,
                HomePhone = user.UserInfo.HomePhone
            };
            viewModel.UserDepartments = user.UserInfo.UserDepartments.ToList();
            viewModel.RoleViews = new List<RoleView>();
            using (UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db)))
            {
                foreach (var role in await userManager.GetRolesAsync(user.Id))
                {
                    RoleView roleView = new RoleView();
                    roleView.RoleName = role;
                    viewModel.RoleViews.Add(roleView);
                }
            }
            return View(viewModel);
        }

        public async Task<ActionResult> ChangeRole(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = db.Users.Find(id);
            var viewModel = new ManageUserViewModel
            {
                UserID = user.Id,
                UserInfoID = user.UserInfo.UserInfoID,
                UserName = user.UserInfo.UserName,
                EmployeeNo = user.UserInfo.EmployeeNo,
                Sex = user.UserInfo.Sex,
                WorkPhone = user.UserInfo.WorkPhone,
                HomePhone = user.UserInfo.HomePhone
            };
            viewModel.UserDepartments = user.UserInfo.UserDepartments.ToList();

            await PopulateAssignedRoleView(user);

            return View(viewModel);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeRole(string id, string[] selectedRoles)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = db.Users.Find(id);
            await UpdateUserRoles(selectedRoles, user);
            return RedirectToAction("Index", new { message = IMSMessageIdEnum.EditdSuccess });
        }

        public ActionResult ResetPassword(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = db.Users.Find(id);
            var viewModel = new ResetPasswordView
            {
                UserID = user.Id,
                EmployeeNo = user.UserName,
            };
            return View(viewModel);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordView model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                using (UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context)))
                {
                    var user = await userManager.FindByIdAsync(model.UserID);
                    if (user == null)
                    {
                        // 请不要显示该用户不存在
                        return RedirectToAction("Index", new { message = IMSMessageIdEnum.EditError });
                    }
                    var result = userManager.RemovePassword(user.Id);
                    if (result.Succeeded)
                    {
                        result = userManager.AddPassword(user.Id, model.NewPassword);
                    }
                }
            }
            return RedirectToAction("Index", new { message = IMSMessageIdEnum.EditdSuccess });
        }
        // GET: ManageUser/Create
        public async Task<ActionResult> Create()
        {
            await InitialRoleViewData();
            await InitialAssignedDepartmentData();
            return View();
        }

        // POST: ManageUser/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        //// 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel model, string[] selectedDepartment, string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                using (UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db)))
                {
                    var email = model.EmployeeNo + "@hims.com";
                    var user = new ApplicationUser { UserName = model.EmployeeNo, Email = email };
                    var userInfo = new UserInfo { UserInfoID = System.Guid.NewGuid(), UserName = model.UserName, WorkPhone = model.WorkPhone, EmployeeNo = model.EmployeeNo };
                    userInfo.UserDepartments = new List<UserDepartment>();
                    await AddUserDepartments(selectedDepartment, userInfo);
                    db.UserInfos.Add(userInfo);
                    int num = await db.SaveChangesAsync();
                    if (num <= 0)
                    {
                        await InitialAssignedDepartmentData();
                        return View(model);
                    }
                    user.UserInfoID = userInfo.UserInfoID;
                    var result = await userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        await AddUserRoles(selectedRoles, user);
                        return RedirectToAction("Index", new { message = IMSMessageIdEnum.CreateSuccess });
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
            await InitialRoleViewData();
            await InitialAssignedDepartmentData();
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
                return RedirectToAction("Index", new { message = IMSMessageIdEnum.EditdSuccess });
            }
            PopulateAssignedDepartmentData(userInfo);

            return View(manageUserViewModel);
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
                else
                {
                    return RedirectToAction("Index", new { message = IMSMessageIdEnum.DeleteError });

                }
            }
            return RedirectToAction("Index", new { message = IMSMessageIdEnum.DeleteSuccess });
        }
        #region UserDepartments
        private async Task InitialAssignedDepartmentData()
        {
            var allDepartments = new List<UserDepartment>();
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                allDepartments = await context.UserDepartments.OrderBy(d => d.UserDepartmentName).ToListAsync();
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
        #endregion

        #region UserRoles
        private async Task InitialRoleViewData()
        {
            var allRoleView = new List<IdentityRole>();
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                using (RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context)))
                {
                    allRoleView = await roleManager.Roles.ToListAsync();
                }
            }

            var assignedRoleViews = new List<RoleView>();
            foreach (var role in allRoleView)
            {
                assignedRoleViews.Add(new RoleView
                {
                    RoleName = role.Name,
                    Assigned = false
                });
            }
            ViewBag.RoleViews = assignedRoleViews;
        }
        private async Task PopulateAssignedRoleView(ApplicationUser user)
        {
            var viewModel = new List<RoleView>();

            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                using (RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context)))
                {
                    using (UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context)))
                    {
                        var allRoleView = await roleManager.Roles.ToListAsync();
                        foreach (var role in allRoleView)
                        {
                            viewModel.Add(new RoleView
                            {
                                RoleName = role.Name,
                                Assigned = userManager.IsInRole(user.Id, role.Name)
                            });
                        }
                    }
                }
            }
            ViewBag.RoleViews = viewModel;

        }
        private async Task AddUserRoles(string[] selectedRoles, ApplicationUser userToUpdate)
        {
            if (selectedRoles == null)
            {
                return;
            }
            using (UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db)))
            {
                using (RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db)))
                {

                    var selectedRolesHS = new HashSet<string>(selectedRoles);
                    var allRoles = await roleManager.Roles.ToListAsync();
                    foreach (var role in allRoles)
                    {
                        if (selectedRolesHS.Contains(role.Name))
                        {
                            if (!await userManager.IsInRoleAsync(userToUpdate.Id, role.Name))
                            {
                                await userManager.AddToRoleAsync(userToUpdate.Id, role.Name);
                            }
                        }
                    }
                }
            }
        }
        private async Task UpdateUserRoles(string[] selectedRoles, ApplicationUser userToUpdate)
        {
            using (UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db)))
            {
                using (RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db)))
                {
                    if (selectedRoles == null)
                    {
                        //移除权限
                        foreach (var role in await userManager.GetRolesAsync(userToUpdate.Id))
                        {
                            await userManager.RemoveFromRoleAsync(userToUpdate.Id, role);
                        }
                        return;
                    }
                    var selectedRolesHS = new HashSet<string>(selectedRoles);
                    var userRoles = await userManager.GetRolesAsync(userToUpdate.Id);

                    var allRoles = await roleManager.Roles.ToListAsync();
                    foreach (var role in allRoles)
                    {
                        if (selectedRolesHS.Contains(role.Name))
                        {
                            if (!await userManager.IsInRoleAsync(userToUpdate.Id, role.Name))
                            {
                                await userManager.AddToRoleAsync(userToUpdate.Id, role.Name);
                            }
                        }
                        else
                        {
                            if (await userManager.IsInRoleAsync(userToUpdate.Id, role.Name))
                            {
                                await userManager.RemoveFromRolesAsync(userToUpdate.Id, role.Name);
                            }
                        }
                    }
                }

            }

        }

        #endregion
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
