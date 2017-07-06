using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMS2.ViewModels.StatisticsDepartmentIndicatorValueViews
{
    /// <summary>
    /// 显示科室指标时段虚拟值表的内容
    /// </summary>
    public class DepartmentIndicatorDurationVirtualValueView
    {
        public Guid DepartmentIndicatorDurationVirtualValueID { get; set; }

        [Display(Name = "科室")]
        public string DepartmentName { get; set; }

        [Display(Name = "指标")]
        public string IndicatorName { get; set; }

        [Display(Name = "跨度")]
        public string DurationName { get; set; }

        [DisplayFormat(DataFormatString = "{0:D}")]
        [Display(Name = "记录时间")]
        public DateTime Time { get; set; }

        [Display(Name = "值")]
        public decimal? Value { get; set; }

        [DisplayFormat(DataFormatString = "{0:D}")]
        [Display(Name = "创建时间")]
        public DateTime CreateTime { get; set; }
        [DisplayFormat(DataFormatString = "{0:D}")]
        [Display(Name = "更新时间")]
        public DateTime UpdateTime { get; set; }
    }
}