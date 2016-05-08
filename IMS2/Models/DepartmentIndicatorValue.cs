using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
namespace IMS2.Models
{
    public partial class DepartmentIndicatorValue
    {
        public Guid DepartmentIndicatorValueId { get; set; }

        public Guid DepartmentId { get; set; }

        public Guid IndicatorId { get; set; }
        [Display(Name = "记录时间")]

        public DateTime Time { get; set; }
        [Display(Name = "数据")]

        public decimal? Value { get; set; }

        public Guid? IndicatorStandardId { get; set; }
        [Display(Name = "审核")]

        public bool IsLocked { get; set; }
        [Display(Name = "更新时间")]

        public DateTime UpdateTime { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [ScaffoldColumn(false)]
        [Timestamp]
        public byte[] TimeStamp { get; set; }

        public virtual DepartmentIndicatorStandard DepartmentIndicatorStandard { get; set; }

        public virtual Department Department { get; set; }

        public virtual Indicator Indicator { get; set; }
    }
}