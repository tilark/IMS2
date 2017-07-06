using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMS2.ViewModels.StatisticsDepartmentIndicatorValueViews
{
    public class DepartmentIndicatorDurationVirtualValueEdit
    {
        public Guid DepartmentIndicatorDurationVirtualValueID { get; set; }      

       

        [Display(Name = "跨度")]
        public Guid DurationId { get; set; }


        [Display(Name = "记录时间")]
        public DateTime Time { get; set; }

        [Display(Name = "值")]
        public decimal? Value { get; set; }
      
    }
}