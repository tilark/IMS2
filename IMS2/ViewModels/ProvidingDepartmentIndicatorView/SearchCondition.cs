using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMS2.ViewModels.ProvidingDepartmentIndicatorView
{
    public class SearchCondition
    {
        [Display(Name = "时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM}", ApplyFormatInEditMode = true)]
        public DateTime SearchTime { get; set; }

        [Display(Name = "科室")]

        public Guid DepartmentId { get; set; }
    }
}