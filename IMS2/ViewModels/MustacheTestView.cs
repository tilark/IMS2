using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS2.ViewModels
{
    public class MustacheTestView
    {
        public string IndicatorName { get; set; }
        public string DurationName { get; set; }
        public Decimal? Value { get; set; }
        public bool IsLocked { get; set; }
        public DateTime Time { get; set; }

    }
}