using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using IMS2.Models;
namespace IMS2.ViewModels
{
    public class DepartmentCategoryIndicatorGroupView
    {
        [Display(Name = "科室类别ID")]

        public Guid DepartmentCategoryId { get; set; }

        [StringLength(128)]
        [Display(Name = "科室类别")]
        public string DepartmentCategoryName { get; set; }
        [Display(Name = "优先级")]

        public decimal Priority { get; set; }
        [Display(Name = "备注")]
        public string Remarks { get; set; }

        public List<IndicatorGroup> IndicatorGroups { get; set; }
    }
}