using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMS2.ViewModels.ImportDepartmentIndicatorViews
{
    /// <summary>
    /// 从Excel表中读取病案统计室数据
    /// </summary>
    public class MedicalRecordStatisticalFromExcel 
    {
        [Display(Name = "病历回收数")]
        [Required]
        public virtual string MedicalRecordRecovery { get; set; }

        [Display(Name = "手术冰冻与石蜡诊断符合例数")]
        [Required]

        public virtual string DiagnoseNumber { get; set; }
        [Display(Name = "手术冰冻与石蜡诊报告总例数")]
        [Required]

        public virtual string ReportNumber { get; set; }

      
    }
}