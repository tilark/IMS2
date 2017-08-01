using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMS2.BusinessModel.SatisticsValueModel
{
    public class DepartmentIndicatorDurationTime
    {
        [Display(Name = "指标")]
        public Guid IndicatorID { get; set; }

        [Display(Name = "科室")]
        public Guid DepartmentId { get; set; }

        [Display(Name = "跨度")]
        public Guid DurationId { get; set; }

        [Display(Name = "记录时间")]
        public DateTime Time { get; set; }

        [Display(Name = "值")]
        public decimal? Value { get; set; }

        public void Update(DepartmentIndicatorDurationTime newValue)
        {
            this.DepartmentId = newValue.DepartmentId;
            this.DurationId = newValue.DurationId;
            this.IndicatorID = newValue.IndicatorID;
            this.Time = newValue.Time;
            this.Value = newValue.Value;
        }

    }
}