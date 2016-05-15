using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMS2.Models
{
    public class UserInfo
    {
        [Key]
        public Guid UserInfoID { get; set; }
        [Required]
        [Display(Name = "用户名")]

        public string UserName { get; set; }
        [Required]
        [Display(Name = "工号")]

        public string EmployeeNo { get; set; }
        [Display(Name = "性别")]

        public string Sex { get; set; }
        public string WorkPhone { get; set; }
        public string HomePhone { get; set; }
        public virtual ICollection<UserDepartment> UserDepartments { get; set; }
    }
}