using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
namespace IMS2.Models
{
    public partial class DepartmentCategoryMapIndicatorGroup
    {
        public Guid DepartmentCategoryMapIndicatorGroupId { get; set; }

        public Guid DepartmentCategoryId { get; set; }

        public Guid IndicatorGroupId { get; set; }
        [Display(Name = "优先级")]

        public decimal Priority { get; set; }
        [Display(Name = "备注")]

        public string Remarks { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [ScaffoldColumn(false)]
        [Timestamp]
        public byte[] TimeStamp { get; set; }

        public virtual DepartmentCategory DepartmentCategory { get; set; }

        public virtual IndicatorGroup IndicatorGroup { get; set; }
    }
}