using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMS2.ViewModels.UploadFileViews
{
    public class ReadExcelMessage
    {
        public ReadExcelMessage()
        {
            this.TotalCount = 0;
            this.ReadSuccessCount = 0;
            this.ReadFailedCount = 0;
            this.DataTotalCount = 0;
        }
        [Display(Name = "Excel总行数")]

        public long TotalCount { get; set; }

        [Display(Name = "数据项总数")]
        public long DataTotalCount { get; set; }
        [Display(Name = "成功条目")]
        public long ReadSuccessCount { get; set; }

        [Display(Name = "失败条目")]
        public long ReadFailedCount { get; set; }

        [Display(Name = "错误信息")]

        public string ErrorMessage { get; set; }
        //将错误内容存放在此处
        [Display(Name = "信息")]

        public string Message
        {
            get
            {
                return String.Format("读取Excel总行数：{0}，数据总项数量有{1}项，成功读取数据{2}项，失败读取{3}项", this.TotalCount,this.DataTotalCount, this.ReadSuccessCount, this.ReadFailedCount);
            }
        }
    }
}