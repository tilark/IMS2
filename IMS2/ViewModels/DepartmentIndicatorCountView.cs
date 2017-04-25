using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IMS2.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace IMS2.ViewModels
{
    public class DepartmentIndicatorCountView
    {
        //public List<Department> Departments{ get; set; }
       public  Department Department { get; set; }
        public List<Indicator> Indicators { get; set; }
        public int IndicatorCount { get; set; }
        public int HasValueCount { get; set; }

        [Display(Name = "时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM}", ApplyFormatInEditMode = true)]
        public DateTime? SearchTime { get; set; }
        public DateTime Time { get; set; }
        public List<DepartmentIndicatorValue> DepartmentIndicatorValues { get; set; }

    }
}