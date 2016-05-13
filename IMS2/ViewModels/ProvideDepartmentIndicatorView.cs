using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IMS2.Models;
namespace IMS2.ViewModels
{
    public class ProvideDepartmentIndicatorView
    {
        public Department provideDepartment { get; set; }
        public List<Indicator> Indicators { get; set; }
        public DateTime searchTime { get; set; }
        public List<DepartmentIndicatorCountView> DepartmentIndicatorCountViews { get; set; }
    }
}