using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace IMS2.ViewModels.StatisticsDepartmentIndicatorValueViews
{
    /// <summary>
    /// 通过指标来源科室获得所有的指标，再找到相对应的科室，组合成
    /// </summary>
    public class DepartmentIndicatorDurationVirtualValueCreate
    {

        [Display(Name = "跨度")]
        public Guid? DurationId { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM}", ApplyFormatInEditMode = true)]
        [Display(Name = "记录时间")]
        public DateTime Time { get; set; }

        [Display(Name = "指标来源科室")]
        public Guid? ProvidingDepartmentId { get; set; }
    }
}