using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS2.Models
{
    /// <summary>
    /// 报表。
    /// </summary>
    /// <remarks>表示整个报表。遍历以获取“报表行”。
    /// 初始化后，执行GetData即可获取数据。
    /// </remarks>
    public partial class Reprot
        : IEnumerable<ReportRow>
    {
        /// <summary>
        /// 报表标题。
        /// </summary>
        public string Title;

        /// <summary>
        /// 期初时段。
        /// </summary>
        public DateTime startTime;

        /// <summary>
        /// 期末时段。
        /// </summary>
        public DateTime endTime;

        /// <summary>
        /// “科室分类”ID。
        /// </summary>
        public List<Guid> departmentCategoryIds;

        /// <summary>
        /// “指标组”ID。
        /// </summary>
        public List<Guid> IndicatorGroupIds;

        /// <summary>
        /// “报表行”。
        /// </summary>
        public List<ReportRow> reportRows;





        /// <summary>
        /// 初始化报表<see cref="Reprot"/>。
        /// </summary>
        /// <param name="departmentCategories">“科室分类”ID。</param>
        /// <param name="IndicatorGroups">“指标组”ID。</param>
        /// <param name="startTime">期初时段。</param>
        /// <param name="endTime">期末时段。</param>
        public Reprot(IEnumerable<Guid> departmentCategoryIds, IEnumerable<Guid> IndicatorGroupIds, DateTime startTime, DateTime endTime)
        {
            this.departmentCategoryIds = new List<Guid>(departmentCategoryIds);
            this.IndicatorGroupIds = new List<Guid>(IndicatorGroupIds);
            this.startTime = startTime;
            this.endTime = endTime;

            reportRows = new List<ReportRow>();
        }

        /// <summary>
        /// 获取数据。
        /// </summary>
        private void GetData()
        {
            using (var context = new ImsDbContext())
            {
                foreach (var departmentCategoryId in departmentCategoryIds)
                {
                    var newReportRow = new ReportRow();

                    //var query = from record in context.DepartmentIndicatorValues
                    //            where record.DepartmentId = Department
                    //            select record;
                }
            }
        }





        public IEnumerator<ReportRow> GetEnumerator()
        {
            return ((IEnumerable<ReportRow>)reportRows).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ReportRow>)reportRows).GetEnumerator();
        }
    }





    /// <summary>
    /// “报表行”。
    /// </summary>
    /// <remarks>表示“报表”中的一行。遍历以获取每个“指标列”。</remarks>
    public partial class ReportRow
        : IEnumerable<ReportRowIndicator>
    {
        /// <summary>
        /// 期初时段。
        /// </summary>
        public DateTime startTime;

        /// <summary>
        /// 期末时段。
        /// </summary>
        public DateTime endTime;

        /// <summary>
        /// “科室”名称。
        /// </summary>
        public string departmentName;

        /// <summary>
        /// “科室分类”名称。
        /// </summary>
        public string departmentCategoryName;

        /// <summary>
        /// “科室”优先级。
        /// </summary>
        public decimal departmentPriority;

        /// <summary>
        /// “科室分类”优先级。
        /// </summary>
        public decimal departmentCatetoryPriority;

        /// <summary>
        /// 该“报表行”所包含的“指标列”。
        /// </summary>
        public List<ReportRowIndicator> ReportRowIndicators;





        public ReportRow()
        {
            ReportRowIndicators = new List<ReportRowIndicator>();
        }

        /// <summary>
        /// 获取“时段”文本。
        /// </summary>
        /// <returns>表示时段的文本，如“2016年2月”、“合计”等。</returns>
        /// <remarks>当表示一个月时，返回如“2016年2月”，表示一个季度时，返回如“2016年第1季度”，表示合计时，返回“合计”。</remarks>
        public string ToStringDateInterval()
        {
            //表示1个月
            if (startTime == endTime)
            {
                return startTime.ToString("yyyy年M月");
            }
            //表示1个季度
            else if ((startTime.Month == 1 || startTime.Month == 4 || startTime.Month == 7 || startTime.Month == 10) && (endTime.Year == startTime.Year && endTime.Month + 2 == startTime.Month))
            {
                return startTime.Year + "年第" + ((startTime.Month - 1) / 3 + 1) + "季度";
            }//待补充“半年”“年”
            else
            {
                return "合计";
            }
        }





        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ReportRowIndicator>)ReportRowIndicators).GetEnumerator();
        }

        public IEnumerator<ReportRowIndicator> GetEnumerator()
        {
            return ((IEnumerable<ReportRowIndicator>)ReportRowIndicators).GetEnumerator();
        }
    }





    /// <summary>
    /// “指标列”。
    /// </summary>
    /// <remarks>表示“报表行”中的一个“指标列”。</remarks>
    public partial class ReportRowIndicator
    {
        /// <summary>
        /// “指标组”名称。
        /// </summary>
        public string IndicatorGroupName;

        /// <summary>
        /// “指标组”优先级。
        /// </summary>
        public decimal IndicatorGroupPriority;

        /// <summary>
        /// “指标”名称。
        /// </summary>
        public string IndicatorName;

        /// <summary>
        /// “指标”优先级。
        /// </summary>
        public decimal IndicatorPriority;

        /// <summary>
        /// “指标值”。
        /// </summary>
        public decimal Value;

        /// <summary>
        /// 是否超“标准值”。
        /// </summary>
        public bool OutOfStandard;
    }
}