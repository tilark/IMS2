using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
namespace IMS2.Models
{
    public partial class Duration
    {
        public Duration()
        {
            Indicators = new HashSet<Indicator>();
        }

        public Guid DurationId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "跨度")]

        public string DurationName { get; set; }

        [Display(Name = "级别")]
        public int? Level { get; set; }
        [Display(Name = "备注")]

        public string Remarks { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Indicator> Indicators { get; set; }


        #region Method
        public bool IsYearDuration()
        {
            return this.DurationName == "年";
        }

        public bool HalfYearDuration()
        {
            return this.DurationName == "半年";
        }
        public bool SeasonDuration()
        {
            return this.DurationName == "季";
        }
        public bool MonthDuration()
        {
            return this.DurationName == "月";
        }
        public int NextLevel()
        {
            return this.Level != null && (int)this.Level >= 2 ? (int)this.Level - 1 : 0;
        }
        #endregion
    }
}