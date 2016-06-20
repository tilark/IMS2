using IMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace IMS2.ViewModels
{
    public class SearchDepartmentIndicatorView
    {

        public Guid DepartmentID { get; set; }
        public String DepartmentName { get; set; }
        public int IndicatorCount { get; set; }
        public int HasValueCount { get; set; }
    }

    public class SearchTimeView
    {

    }
    public class ProvidingDepartmentView
    {
        public DateTime SearchTime { get; set; }

        public Guid ProvidingDepartmentID { get; set; }
        public string ProvidingDepartmentName { get; set; }
        public List<IndicatorDurationView> IndicatorDurationViews { get; set; }
        public List<SearchDepartmentIndicatorView> SearchDepartmentIndicatorViews { get; set; }
    }
    public class IndicatorDurationView
    {
        public string IndicatorName { get; set; }
        public string DurationName { get; set; }
    }


}