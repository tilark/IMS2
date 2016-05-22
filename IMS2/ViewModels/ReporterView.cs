using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IMS2.Models;
namespace IMS2.ViewModels
{
    public class ReporterView
    {
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public List<DepartmentCategory> DepartmentCategories { get; set; }
        public List<IndicatorGroup> IndicatorGroups { get; set; }

    }
}