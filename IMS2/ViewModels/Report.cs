using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.Entity;

namespace IMS2.Models
{
    /// <summary>
    /// 报表。
    /// </summary>
    /// <remarks>表示整个报表。</remarks>
    public partial class Reprot
    {
        public Reprot(IEnumerable<DepartmentCategory> departmentCategories,IEnumerable<IndicatorGroup> IndicatorGroups,DateTime startTime,DateTime endTime)
        {

        }
    }





    /// <summary>
    /// 报表行。
    /// </summary>
    /// <remarks>表示报表中的一行。</remarks>
    public partial class ReportRow
    {
        public ReportRow()
        {
            ReportIndicators = new HashSet<ReportRowIndicator>();
        }

        public DateTime startTime;
        public DateTime endTime;

        public string departmentName;
        public string departmentCategoryName;

        public virtual ICollection<ReportRowIndicator> ReportIndicators { get; set; }
    }





    /// <summary>
    /// 报表行。
    /// </summary>
    public partial class ReportRow
    {
        /// <summary>
        /// 时段文本。
        /// </summary>
        /// <returns>表示时段的文本，如“2016-02”、“合计”等。</returns>
        /// <remarks>当表示一个月时，返回“年-月”字符串，其他情况即表示合计行，返回“合计”。</remarks>
        public string ToStringDate()
        {
            if(startTime == endTime)
            {
                return startTime.ToString("yyyy-MM");
            }
            else
            {
                return "合计";
            }
        }
    }





    /// <summary>
    /// 报表行中的指标项。
    /// </summary>
    public partial class ReportRowIndicator
    {
        public string IndicatorGroupName;
        public decimal IndicatorGroupPriority;
        public string IndicatorName;
        public decimal IndicatorPriority;
        public decimal Value;
        public bool OutOfStandard;
    }
}