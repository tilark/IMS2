using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace IMS2.ViewModels.ImportDepartmentIndicatorViews
{
    /// <summary>
    /// 康复医学科
    /// </summary>
    public class RehabilitationFromExcel
    {
        [Display(Name = "康复治疗有效患者数")]
        [Required]
        public virtual string Data1 { get; set; }

        [Display(Name = "康复治疗患者数")]
        [Required]
        public virtual string Data2 { get; set; }
        [Display(Name = "住院患者康复功能评定例数")]
        [Required]
        public virtual string Data3 { get; set; }

        [Display(Name = "完好设备数")]
        [Required]
        public virtual string Data4 { get; set; }

        [Display(Name = "设备总数")]
        [Required]
        public virtual string Data5 { get; set; }
    }
}