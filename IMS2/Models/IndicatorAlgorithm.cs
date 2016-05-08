using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
namespace IMS2.Models
{
    public partial class IndicatorAlgorithm
    {
        [Key]
        public Guid IndicatorAlgorithmsId { get; set; }
        [Display(Name = "结果项")]

        public Guid ResultId { get; set; }
        [Display(Name = "首操作项")]

        public Guid FirstOperandID { get; set; }
        [Display(Name = "尾操作项")]

        public Guid SecondOperandID { get; set; }

        [Required]
        [Display(Name = "操作方法")]

        public string OperationMethod { get; set; }
        [Display(Name = "备注")]

        public string Remarks { get; set; }
    }
}