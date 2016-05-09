using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
namespace IMS2.Models
{
    public partial class Department
    {
        public Department()
        {
            DepartmentIndicatorStandards = new HashSet<DepartmentIndicatorStandard>();
            DepartmentIndicatorValues = new HashSet<DepartmentIndicatorValue>();
            Indicators = new HashSet<Indicator>();
            Indicators1 = new HashSet<Indicator>();
        }

        public Guid DepartmentId { get; set; }

        public Guid DepartmentCategoryId { get; set; }

        [Required]
        [Display(Name = "科室")]

        public string DepartmentName { get; set; }
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

        public virtual ICollection<DepartmentIndicatorStandard> DepartmentIndicatorStandards { get; set; }

        public virtual ICollection<DepartmentIndicatorValue> DepartmentIndicatorValues { get; set; }

        public virtual ICollection<Indicator> Indicators { get; set; }

        public virtual ICollection<Indicator> ProvidingIndicators { get; set; }
    }
}