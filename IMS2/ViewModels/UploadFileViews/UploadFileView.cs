using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMS2.ViewModels.UploadFileViews
{
    public class UploadFileView
    {
        private DateTime reporterDate { get;  set; }
        [Display(Name = "报表日期")]
        [Required]
        public DateTime ReporterDate {
            get
            {
                return new DateTime(this.reporterDate.Year, this.reporterDate.Month, 1);
            }
            set { this.reporterDate = value; } }

        
    }
}