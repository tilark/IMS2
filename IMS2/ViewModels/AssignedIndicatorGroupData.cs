using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS2.ViewModels
{
    public class AssignedIndicatorGroupData
    {
        public Guid IndicatorGroupId { get; set; }
        public string IndicatorGroupName { get; set; }
        public bool Assigned { get; set; }

        public string Color { get; set; }
    }
}