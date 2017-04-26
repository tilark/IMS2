using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using FromExcelResourceFile;

namespace IMS2.ViewModels.ImportDepartmentIndicatorViews
{
    /// <summary>
    /// 从Excel表中读取病案统计室数据
    /// </summary>
    public class MedicalRecordStatisticalFromExcel 
    {
        //[Display(Name = "病历回收数")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "MedicalRecordStatisticalData1")]
        [Required]
        public virtual string Data1 { get; set; }

        //[Display(Name = "手术冰冻与石蜡诊断符合例数")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "MedicalRecordStatisticalData2")]
        [Required]

        public virtual string Data2 { get; set; }

        //[Display(Name = "手术冰冻与石蜡诊报告总例数")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "MedicalRecordStatisticalData3")]
        [Required]

        public virtual string Data3 { get; set; }

      
    }
}