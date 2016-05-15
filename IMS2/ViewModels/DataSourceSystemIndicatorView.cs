using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IMS2.Models;
namespace IMS2.ViewModels
{
    public class DataSourceSystemIndicatorView
    {
        public DataSourceSystem dataSourceSystem { get; set; }
        public List<Indicator> Indicators { get; set; }
        public DateTime searchTime { get; set; }
        public List<DepartmentIndicatorCountView> DepartmentIndicatorCountViews { get; set; }
    }
}