using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMS2.ViewModels.ImportDepartmentIndicatorViews
{
    /// <summary>
    /// 财务部
    /// </summary>
    public class FinanceFromExcel
    {
        [Display(Name = "药品总费用")]
        [Required]
        public virtual string Data1 { get; set; }

        [Display(Name = "总费用")]
        [Required]
        public virtual string Data2 { get; set; }
    }
}