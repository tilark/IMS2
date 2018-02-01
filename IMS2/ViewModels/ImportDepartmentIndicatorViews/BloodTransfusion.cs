using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using FromExcelResourceFile;

namespace IMS2.ViewModels.ImportDepartmentIndicatorViews
{
    /// <summary>
    /// 输血科
    /// </summary>
    public class BloodTransfusion
    {
        //[Display(Name = "[SX-1-1]红细胞用量")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "BloodTransfusion1")]
        [Required]
        public virtual string Data1 { get; set; }

        //[Display(Name = "[SX-2-1]血浆用量")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "BloodTransfusion2")]
        [Required]

        public virtual string Data2 { get; set; }

        //[Display(Name = "[SX-3-1]冷沉淀用量")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "BloodTransfusion3")]
        [Required]

        public virtual string Data3 { get; set; }

        //[Display(Name = "[SX-4-1]血小板用量")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "BloodTransfusion4")]
        [Required]
        public virtual string Data4 { get; set; }
    }
}