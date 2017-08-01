using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace IMS2.ViewModels.StatisticsDepartmentIndicatorValueViews
{
    public class SatisticsSearchCondition
    {
        [Required]
        [Display(Name = "时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM}", ApplyFormatInEditMode = true)]
        public DateTime SearchTime { get; set; }

        [Display(Name = "科室")]
        public Guid? DepartmentID { get; set; }

        [Display(Name = "跨度")]
        public Guid? DurationID { get; set; }

        [Display(Name ="指标")]
        public Guid? IndicatorID { get; set; }
    }
}