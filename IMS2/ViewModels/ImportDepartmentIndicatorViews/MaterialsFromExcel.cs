using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using FromExcelResourceFile;

namespace IMS2.ViewModels.ImportDepartmentIndicatorViews
{
    /// <summary>
    /// 物资办
    /// </summary>
    public class MaterialsFromExcel
    {
        //[Display(Name = "总卫生材料收入")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "MaterialsStatisticalData1")]
        [Required]
        public virtual string Data1 { get; set; }

        //[Display(Name = "【物资】总收入")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "MaterialsStatisticalData2")]
        [Required]

        public virtual string Data2 { get; set; }

        //[Display(Name = "卫材总领用")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "MaterialsStatisticalData3")]
        [Required]

        public virtual string Data3 { get; set; }

        //[Display(Name = "【物资】不含药品收入")]
        [Display(ResourceType = typeof(FromExcelResource), Name = "MaterialsStatisticalData4")]
        [Required]
        public virtual string Data4 { get; set; }
    }
}