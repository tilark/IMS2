using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace IMS2.ViewModels
{
    public class RoleView
    {
        [Required]
        [Display(Name ="权限名")]
        public string RoleName { get; set; }
        public bool Assigned { get; set; }

    }
}