using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMS2.Models
{
    public class UserDepartment
    {
        [Key]
        [ScaffoldColumn(false)]

        public Guid UserDepartmentId { get; set; }
        [Required]
        [Display(Name = "科室")]

        public string UserDepartmentName { get; set; }
        [Display(Name = "备注")]
        public string Remarks { get; set; }
        public virtual ICollection<UserInfo> UserInfos { get; set; }

    }
}