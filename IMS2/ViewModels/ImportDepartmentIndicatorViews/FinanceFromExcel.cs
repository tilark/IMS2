using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using FromExcelResourceFile;

namespace IMS2.ViewModels.ImportDepartmentIndicatorViews
{
    /// <summary>
    /// 财务部
    /// </summary>
    public class FinanceFromExcel
    {
        //[Display(Name = "药品总费用")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "FinanceData1")]
        [Required]
        public virtual string Data1 { get; set; }

        //[Display(Name = "总费用")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "FinanceData2")]
        [Required]
        public virtual string Data2 { get; set; }

        //[Display(Name = "专科及门急诊总费用")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "FinanceData3")]
        [Required]
        public virtual string Data3 { get; set; }

        //[Display(Name = "专科及门急诊药品费用")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "FinanceData4")]
        [Required]
        public virtual string Data4 { get; set; }
    }
}