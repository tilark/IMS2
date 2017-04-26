using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using FromExcelResourceFile;

namespace IMS2.ViewModels.ImportDepartmentIndicatorViews
{
    /// <summary>
    /// 超声科
    /// </summary>
    public class UltrasoundFromExcel
    {
        //[Display(Name = "医学影像诊断与手术后符合例数【超声】【抽查】")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "UltrasoundData1")]
        [Required]
        public virtual string Data1 { get; set; }

        //[Display(Name = "医学影像诊断例数【超声】【抽查】")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "UltrasoundData2")]
        [Required]
        public virtual string Data2 { get; set; }
    }
}