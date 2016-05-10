using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
namespace IMS2.Models
{
    public partial class DepartmentIndicatorStandard
    {
        public DepartmentIndicatorStandard()
        {
            DepartmentIndicatorValues = new HashSet<DepartmentIndicatorValue>();
        }

        public Guid DepartmentIndicatorStandardId { get; set; }

        public Guid DepartmentId { get; set; }

        public Guid IndicatorId { get; set; }
        [Display(Name = "上限值")]

        public decimal? UpperBound { get; set; }
        [Display(Name = "包含上限值")]

        public bool? UpperBoundIncluded { get; set; }
        [Display(Name = "下限值")]

        public decimal? LowerBound { get; set; }
        [Display(Name = "包含下限值")]

        public bool? LowerBoundIncluded { get; set; }
        [Display(Name = "更新时间")]

        public DateTime UpdateTime { get; set; }
        [Display(Name = "标准值版本")]

        public int Version { get; set; }
        [Display(Name = "备注")]

        public string Remarks { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [ScaffoldColumn(false)]
        [Timestamp]
        public byte[] TimeStamp { get; set; }

        public virtual Department Department { get; set; }

        public virtual Indicator Indicator { get; set; }

        public virtual ICollection<DepartmentIndicatorValue> DepartmentIndicatorValues { get; set; }
        public string Range
        {
            get
            {
                return "[" + LowerBound.ToString() + "-" + UpperBound.ToString() + "]";
            }
        }
    }
}