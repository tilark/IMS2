using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMS2.Models
{
    /// <summary>
    /// 科室指标时段值，包括聚合指标及基础指标
    /// </summary>
    public class DepartmentIndicatorDurationVirtualValue
    {
        public Guid DepartmentIndicatorDurationVirtualValueID { get; set; }

        [Display(Name = "科室")]
        public Guid DepartmentId { get; set; }

        [Display(Name = "指标")]
        public Guid IndicatorId { get; set; }

        [Display(Name = "时段")]
        public Guid DurationId { get; set; }


        [Display(Name = "记录时间")]
        public DateTime Time { get; set; }

        [Display(Name = "值")]
        public decimal? Value { get; set; }


        [Display(Name = "创建时间")]
        public DateTime CreateTime { get; set; }

        [Display(Name = "更新时间")]
        public DateTime UpdateTime { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [ScaffoldColumn(false)]
        [Timestamp]
        public byte[] TimeStamp { get; set; }

        public virtual Department Department { get; set; }

        public virtual Indicator Indicator { get; set; }

        public virtual Duration Duration { get; set; }
    }
}