using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS2.ViewModels
{
    public class AssignedIndicatorData
    {
        public Guid IndicatorId { get; set; }
        public string IndicatorName { get; set; }
        public bool Assigned { get; set; }

    }
}