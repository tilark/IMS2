using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace IMS2.ViewModels.ImportDepartmentIndicatorViews
{
    /// <summary>
    /// 病理科
    /// </summary>
    public class PathologyFromExcel
    {
        [Display(Name = "常规诊断报告准确例数【抽查】")]
        [Required]
        public virtual string Data1 { get; set; }

        [Display(Name = "常规诊断报告总例数【抽查】")]
        [Required]
        public virtual string Data2 { get; set; }

        [Display(Name = "病理诊断报告在5个工作日内发出例数")]
        [Required]
        public virtual string Data3 { get; set; }

        [Display(Name = "病理诊断报告总例数")]
        [Required]
        public virtual string Data4 { get; set; }

        [Display(Name = "病理报告书内容与格式书写合格例数【抽查】")]
        [Required]
        public virtual string Data5 { get; set; }

        [Display(Name = "病理报告总例数【抽查】")]
        [Required]
        public virtual string Data6 { get; set; }
    }
}