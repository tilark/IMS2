using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using FromExcelResourceFile;

namespace IMS2.ViewModels.ImportDepartmentIndicatorViews
{
    /// <summary>
    /// 放射科
    /// </summary>
    public class RadiologyFromExcel
    {
        //[Display(Name = "医学影像诊断与手术后符合例数【放射】【抽查】")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "RadiologyData1")]
        [Required]
        public virtual string Data1 { get; set; }

        //[Display(Name = "医学影像诊断例数【放射】【抽查】")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "RadiologyData2")]
        [Required]
        public virtual string Data2 { get; set; }
    }
}