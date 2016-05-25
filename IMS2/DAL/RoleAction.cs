using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OperateExcel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using IMS2.Models;

namespace IMS2.DAL
{
    public class RoleAction
    {
        internal void InitialRoleName()
        {
            //从权限Excel表中读取权限名称，加入到权限表中
            var fileName = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager
               .AppSettings["RoleName"]);
            ReadFromExcel readFromExcel = new ReadFromExcel();
            //按Row获取当前行的所有数据
            int rowCount = readFromExcel.GetRowCount(fileName);
            for (int i = 2; i <= rowCount; i++)
            {
                var columnData = readFromExcel.ReadRowFromExcel((uint)i, fileName);
                //数据必须大于等1才有效
                //第1列为权限名称
                if (columnData.Count >= 1)
                {
                    using (ApplicationDbContext context = new ApplicationDbContext())
                    {
                        using (RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context)))
                        {
                            var roleName = columnData.ElementAt(0);
                            if (!roleManager.RoleExists(roleName))
                            {
                                roleManager.Create(new IdentityRole(roleName));
                            }
                        }
                    }
                }
            }
        }
        internal void CreateAdmin()
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                using (UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context)))
                {
                    using (RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context)))
                    {
                        //Create Role Administrator if it does not exist
                        if (!roleManager.RoleExists("Administrators"))
                        {
                            var roleResult = roleManager.Create(new IdentityRole("Administrators"));
                        }

                        //创建Administrator用户
                        var userInfo = new UserInfo { UserInfoID = System.Guid.NewGuid(), UserName = "Administrators", EmployeeNo="0000" };
                        userInfo.UserDepartments = new List<UserDepartment>();
                        context.UserInfos.Add(userInfo);
                        int num = context.SaveChanges();

                        string adminName = "Administrator@qq.com";
                        string password = "52166057";
                        var user = new ApplicationUser();
                        user.UserName = adminName;
                        user.Email = "Administrator@qq.com";
                        user.UserInfoID = userInfo.UserInfoID;

                        var adminResult = userManager.Create(user, password);
                        //Add User Admin to Role Administrator
                        if (adminResult.Succeeded)
                        {
                            var result = userManager.AddToRole(user.Id, "Administrators");
                        }
                        else
                        {
                            //没成功，需要删除userInfo
                            var userInfoDel = context.UserInfos.Find(userInfo.UserInfoID);
                            context.UserInfos.Remove(userInfoDel);
                            context.SaveChanges();

                        }
                    }
                }
            }
        }
    }
}