using IMS2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMS2.ViewModels
{
    public class DepartmentIndicatorStandardView
    {
        public Guid DepartmentIndicatorStandardId { get; set; }
        [Display(Name = "科室")]

        public Guid DepartmentId { get; set; }
        [Display(Name = "指标")]

        public Guid IndicatorId { get; set; }
        [Display(Name = "上限值")]

        public decimal? UpperBound { get; set; }
        [Display(Name = "包含上限值")]

        public bool UpperBoundIncluded { get; set; }
        [Display(Name = "下限值")]

        public decimal? LowerBound { get; set; }
        [Display(Name = "包含下限值")]

        public bool LowerBoundIncluded { get; set; }
        [Display(Name = "范围")]

        public string Range { get; set; }
        [Display(Name = "更新时间")]

        public DateTime UpdateTime { get; set; }
        [Display(Name = "标准值版本")]

        public int Version { get; set; }
        [Display(Name = "备注")]

        public string Remarks { get; set; }
        [Display(Name = "科室")]

        public string DepartmentName { get; set; }
        [Display(Name = "指标")]

        public string IndicatorName { get; set; }
    }
}