using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IMS2.ViewModels;
using System.Data.Entity.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using IMS2.Models;

namespace IMS2.Controllers
{
    [Authorize(Roles = "修改全院人员信息, Administrators")]

    public class ManageRoleController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ManageRole
        public ActionResult Index(IMSMessageIdEnum? message)
        {
            ViewBag.StatusMessage =
             message == IMSMessageIdEnum.CreateSuccess ? "已创建新权限。"
             : message == IMSMessageIdEnum.EditdSuccess ? "已更新完成。"
             : message == IMSMessageIdEnum.DeleteSuccess ? "已删除成功。"
             : message == IMSMessageIdEnum.CreateError ? "重复权限。"
             : message == IMSMessageIdEnum.EditError ? "有重名，无法更新相关信息。"
             : message == IMSMessageIdEnum.DeleteError ? "不允许删除该项。"
             : "";
            var viewModel = new List<RoleView>();
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                using (RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context)))
                {
                    foreach(var role in roleManager.Roles.ToList())
                    {
                        RoleView roleView = new RoleView();
                        roleView.RoleName = role.Name;
                        viewModel.Add(roleView);
                    }
                    return View(viewModel);
                }
            }
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RoleView model)
        {
            if (ModelState.IsValid)
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    using (UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context)))
                    {
                        using (RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context)))
                        {
                            
                            if (!await roleManager.RoleExistsAsync(model.RoleName))
                            {
                                await roleManager.CreateAsync(new IdentityRole(model.RoleName));
                                return RedirectToAction("Index", new { message = IMSMessageIdEnum.CreateSuccess });

                            }
                        }
                    }
                }
            }
            return RedirectToAction("Index", new { message = IMSMessageIdEnum.CreateError });
        }

        public ActionResult Delete(string id)
        {
            RoleView viewModel = new RoleView();
            viewModel.RoleName = id;
            return View(viewModel);
        }
        // POST: Indicators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                using (RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context)))
                {
                    var role = await roleManager.FindByNameAsync(id);
                    //如果role在user中存在，不能删除
                    var query = context.Users.SelectMany(r => r.Roles).Where(r => r.RoleId == role.Id).FirstOrDefault();
                    if(query == null)
                    {
                        await roleManager.DeleteAsync(role);
                        return RedirectToAction("Index", new { message = IMSMessageIdEnum.DeleteSuccess });

                    }
                }
            }
            return RedirectToAction("Index", new { message = IMSMessageIdEnum.DeleteError });
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