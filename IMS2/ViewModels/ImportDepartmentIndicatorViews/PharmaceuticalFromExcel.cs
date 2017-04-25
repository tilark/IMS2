using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace IMS2.ViewModels.ImportDepartmentIndicatorViews
{
    /// <summary>
    /// 药学部
    /// </summary>
    public class PharmaceuticalFromExcel
    {
        [Display(Name = "住院病人抗菌药物使用人数")]
        [Required]
        public virtual string Data1 { get; set; }

        [Display(Name = "抗菌药物消耗量")]
        [Required]
        public virtual string Data2 { get; set; }
        [Display(Name = "基药总费用")]
        [Required]
        public virtual string Data3 { get; set; }

        [Display(Name = "药品总费用【基药】")]
        [Required]
        public virtual string Data4 { get; set; }
        [Display(Name = "I类切口手术抗菌药物预防使用例数")]
        [Required]
        public virtual string Data5 { get; set; }

        [Display(Name = "除外治疗性用药的Ⅰ类切口手术例数")]
        [Required]
        public virtual string Data6 { get; set; }
        [Display(Name = "门诊使用抗菌药物人数")]
        [Required]
        public virtual string Data7 { get; set; }

        [Display(Name = "急诊使用抗菌药物人数")]
        [Required]
        public virtual string Data8 { get; set; }
    }
}