using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMS2.ViewModels.UploadFileViews
{
    public class UploadFileView
    {
       
        [Display(Name = "报表日期")]
        [Required]
        public DateTime ReporterDate { get; set; }
    }
}