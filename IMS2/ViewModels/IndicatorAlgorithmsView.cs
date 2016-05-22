using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMS2.ViewModels
{
    public class IndicatorAlgorithmsView
    {
        public Guid IndicatorAlgorithmsId { get; set; }
        [Display(Name = "结果项")]
        [Required]

        public string Result { get; set; }
        [Display(Name = "首操作项")]
        [Required]
        public string FirstOperand { get; set; }
        [Display(Name = "尾操作项")]
        [Required]

        public string SecondOperand { get; set; }
        [Display(Name = "操作方法")]
        [Required]

        public OperationMethod OperationMethod { get; set; }
        [Display(Name = "备注")]

        public string Remarks { get; set; }
    }
    public enum OperationMethod
    {
        addition,
        subtraction,
        multiplication,
        division
    }
}