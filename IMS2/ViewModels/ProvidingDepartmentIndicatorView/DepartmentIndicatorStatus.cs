using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMS2.ViewModels.ProvidingDepartmentIndicatorView
{
    public class DepartmentIndicatorStatus
    {

        [Display(Name = "科室")]

        public string DepartmentName { get; set; }
        public Guid DepartmentID { get; set; }
        [Display(Name = "需填写项")]
        public int IndicatorCount { get; set; }

        [Display(Name = "已填写项")]
        public int HasValueCount { get; set; }

        [Display(Name = "登记时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM}", ApplyFormatInEditMode = true)]
        public DateTime SearchTime { get; set; }
    }
}