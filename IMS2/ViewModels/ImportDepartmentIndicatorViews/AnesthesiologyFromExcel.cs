using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace IMS2.ViewModels.ImportDepartmentIndicatorViews
{
    /// <summary>
    /// 麻醉科
    /// </summary>
    public class AnesthesiologyFromExcel
    {
        [Display(Name = "麻醉相关死亡人数")]
        [Required]
        public virtual string Data1 { get; set; }

        [Display(Name = "硬膜外阻滞成功例数")]
        [Required]
        public virtual string Data2 { get; set; }
        [Display(Name = "硬膜外阻滞总例数")]
        [Required]
        public virtual string Data3 { get; set; }

        [Display(Name = "神经阻滞麻醉成功例数")]
        [Required]
        public virtual string Data4 { get; set; }
        [Display(Name = "神经阻滞麻醉总例数")]
        [Required]
        public virtual string Data5 { get; set; }

        [Display(Name = "硬膜外麻醉硬脊膜穿破发生例数")]
        [Required]
        public virtual string Data6 { get; set; }
        [Display(Name = "硬膜外麻醉总例数")]
        [Required]
        public virtual string Data7 { get; set; }

        [Display(Name = "椎管内麻醉神经并发症发生例数")]
        [Required]
        public virtual string Data8 { get; set; }
        [Display(Name = "椎管内麻醉总例数")]
        [Required]
        public virtual string Data9 { get; set; }

        [Display(Name = "椎管内麻醉神经并发症截瘫发生例数")]
        [Required]
        public virtual string Data10 { get; set; }
    }
}