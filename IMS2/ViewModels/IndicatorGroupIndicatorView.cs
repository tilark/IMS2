using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IMS2.Models;
using System.ComponentModel.DataAnnotations;

namespace IMS2.ViewModels
{
    public class IndicatorGroupIndicatorView
    {
        [Display(Name = "指标组ID")]
        public Guid IndicatorGroupId { get; set; }
        [Display(Name = "指标组")]
        public string IndicatorGroupName { get; set; }
        [Display(Name = "优先级")]

        public decimal Priority { get; set; }
        [Display(Name = "备注")]

        public string Remarks { get; set; }

        public List<Indicator> Indicators { get; set; }

    }
}