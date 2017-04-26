using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using FromExcelResourceFile;

namespace IMS2.ViewModels.ImportDepartmentIndicatorViews
{
    /// <summary>
    /// 检验科
    /// </summary>
    public class ExamineFromExcel
    {
        //[Display(Name = "POCT项目对比例数")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "ExamineData1")]
        [Required]
        public virtual string Data1 { get; set; }

        //[Display(Name = "POCT项目总数")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "ExamineData2")]
        [Required]
        public virtual string Data2 { get; set; }

        //[Display(Name = "危急值报告及时例数")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "ExamineData3")]
        [Required]
        public virtual string Data3 { get; set; }

        //[Display(Name = "危急值总例数")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "ExamineData4")]
        [Required]
        public virtual string Data4 { get; set; }
    }
}