using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using FromExcelResourceFile;

namespace IMS2.ViewModels.ImportDepartmentIndicatorViews
{
    /// <summary>
    /// 门诊部
    /// </summary>
    public class ClinicFromExcel
    {
        //[Display(Name = "预约诊疗总人次")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "ClinicData1")]
        [Required]
        public virtual string Data1 { get; set; }

        //[Display(Name = "预约号源数")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "ClinicData2")]
        [Required]
        public virtual string Data2 { get; set; }
    }
}