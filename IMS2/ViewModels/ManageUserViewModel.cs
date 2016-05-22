using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IMS2.Models;
using System.ComponentModel.DataAnnotations;

namespace IMS2.ViewModels
{
    public class ManageUserViewModel
    {
        public string UserID { get; set; }
        [ScaffoldColumn(false)]
        public Guid UserInfoID { get; set; }
        [Required]
        [Display(Name = "用户名")]

        public string UserName { get; set; }
        [Display(Name = "工号")]

        public string EmployeeNo { get; set; }
        [Display(Name = "性别")]

        public string Sex { get; set; }
        [Display(Name = "工作电话")]

        public string WorkPhone { get; set; }
        [Display(Name = "家庭电话")]

        public string HomePhone { get; set; }
        public List<UserDepartment>  UserDepartments { get; set; }
        public List<RoleView> RoleViews { get; set; }
    }
}