using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace IMS2.Models
{
    public partial class DataSourceSystem
    {
        public DataSourceSystem()
        {
            Indicators = new HashSet<Indicator>();
        }

        public Guid DataSourceSystemId { get; set; }

        [Required]
        [Display(Name = "数据来源系统")]

        public string DataSourceSystemName { get; set; }
        [Display(Name = "优先级")]

        public decimal Priority { get; set; }
        [Display(Name = "备注")]

        public string Remarks { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [ScaffoldColumn(false)]
        [Timestamp]
        public byte[] TimeStamp { get; set; }

        public virtual ICollection<Indicator> Indicators { get; set; }
    }
}