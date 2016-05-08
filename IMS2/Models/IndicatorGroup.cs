using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
namespace IMS2.Models
{
    public partial class IndicatorGroup
    {
        public IndicatorGroup()
        {
            DepartmentCategoryMapIndicatorGroups = new HashSet<DepartmentCategoryMapIndicatorGroup>();
            IndicatorGroupMapIndicators = new HashSet<IndicatorGroupMapIndicator>();
        }

        public Guid IndicatorGroupId { get; set; }

        [Required]
        [Display(Name = "指标组")]

        public string IndicatorGroupName { get; set; }
        [Display(Name = "优先级")]

        public decimal Priority { get; set; }
        [Display(Name = "备注")]

        public string Remarks { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [ScaffoldColumn(false)]
        [Timestamp]
        public byte[] TimeStamp { get; set; }

        public virtual ICollection<DepartmentCategoryMapIndicatorGroup> DepartmentCategoryMapIndicatorGroups { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IndicatorGroupMapIndicator> IndicatorGroupMapIndicators { get; set; }
    }
}