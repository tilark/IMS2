using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
namespace IMS2.Models
{
    public partial class Indicator
    {
        public Indicator()
        {
            DepartmentIndicatorStandards = new HashSet<DepartmentIndicatorStandard>();
            DepartmentIndicatorValues = new HashSet<DepartmentIndicatorValue>();
            IndicatorGroupMapIndicators = new HashSet<IndicatorGroupMapIndicator>();
        }

        public Guid IndicatorId { get; set; }

        [Required]
        [Display(Name = "指标")]

        public string IndicatorName { get; set; }

        [Required]
        [Display(Name = "单位")]

        public string Unit { get; set; }
        [Display(Name = "自动获取")]

        public bool IsAutoGetData { get; set; }

        public Guid? ProvidingDepartmentId { get; set; }

        public Guid? DataSourceSystemId { get; set; }

        public Guid? DutyDepartmentId { get; set; }

        public Guid? DurationId { get; set; }
        [Display(Name = "优先级")]

        public decimal Priority { get; set; }
        [Display(Name = "备注")]

        public string Remarks { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [ScaffoldColumn(false)]
        [Timestamp]
        public byte[] TimeStamp { get; set; }

        public virtual DataSourceSystem DataSourceSystem { get; set; }

        public virtual ICollection<DepartmentIndicatorStandard> DepartmentIndicatorStandards { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DepartmentIndicatorValue> DepartmentIndicatorValues { get; set; }
        //责任科室
        public virtual Department Department { get; set; }
        //数据来源科室
        public virtual Department ProvidingDepartment { get; set; }

        public virtual Duration Duration { get; set; }

        public virtual ICollection<IndicatorGroupMapIndicator> IndicatorGroupMapIndicators { get; set; }
    }
}