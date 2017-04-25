using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace IMS2.ViewModels.VerifyDepartmentIndicatorView
{
    public class VerifySearchCondition
    {
        [Display(Name = "开始时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM}", ApplyFormatInEditMode = true)]
        public DateTime SearchStartTime { get; set; }

        [Display(Name = "截止时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM}", ApplyFormatInEditMode = true)]
        public DateTime SearchEndTime { get; set; }
        [Display(Name = "科室")]

        public Guid? DepartmentId { get; set; }

        [Display(Name = "审核状态")]
        public LockStatus LockStatus { get; set; }
    }

    public enum LockStatus
    {
        [Display(Name = "全选")]
        All,
        [Display(Name = "已审核")]
        Locked,
        [Display(Name = "未审核")]
        UnLock
    }
}