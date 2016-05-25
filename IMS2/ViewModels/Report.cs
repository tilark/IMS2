using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS2.ViewModels
{
    /// <summary>
    /// 报表。
    /// </summary>
    /// <remarks>表示整个报表。遍历以获取“报表行”。
    /// 初始化后，执行GetData即可获取数据。
    /// </remarks>
    public partial class Report
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
        /// 初始化报表<see cref="Report"/>。
        /// </summary>
        /// <param name="departmentCategories">“科室分类”ID。</param>
        /// <param name="IndicatorGroups">“指标组”ID。</param>
        /// <param name="startTime">期初时段。</param>
        /// <param name="endTime">期末时段。</param>
        public Report(IEnumerable<Guid> departmentCategoryIds, IEnumerable<Guid> IndicatorGroupIds, DateTime startTime, DateTime endTime)
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
        public void GetData()
        {
            //标准化“期初时段”“期末时段”时间点。
            startTime = new DateTime(startTime.Year, startTime.Month, 1);
            endTime = new DateTime(endTime.Year, endTime.Month, 1);

            using (var context = new Models.ImsDbContext())
            {
                //遍历每个“科室分类ID”。
                foreach (var departmentCategoryId in departmentCategoryIds)
                {
                    //获取“科室分类”。
                    var departmentCategory = context.DepartmentCategories.Where(i => i.DepartmentCategoryId == departmentCategoryId).FirstOrDefault();

                    //处理“科室分类”不存在。
                    if (departmentCategory == null)
                    {
                        throw new Exception("科室分类ID不存在：" + departmentCategoryId.ToString());
                    }

                    //获取“科室分类”对应的“科室”。
                    var departments = context.DepartmentCategories.Where(i => i.DepartmentCategoryId == departmentCategoryId).SelectMany(c => c.Departments);

                    //遍历每个“科室”。
                    foreach (var department in departments)
                    {
                        //用于遍历时间段的时间点。
                        DateTime tempTime = startTime;

                        while (tempTime <= endTime)
                        {
                            //月份。
                            {
                                //新增“报表行”。
                                var newReportRow = new ReportRow();
                                reportRows.Add(newReportRow);

                                newReportRow.startTime = tempTime;
                                newReportRow.endTime = tempTime;
                                newReportRow.departmentName = department.DepartmentName;
                                newReportRow.departmentPriority = department.Priority;
                                newReportRow.departmentCategoryName = departmentCategory.DepartmentCategoryName;
                                newReportRow.departmentCatetoryPriority = departmentCategory.Priority;

                                //遍历“指标组ID”
                                foreach (var indicatorGourpId in IndicatorGroupIds)
                                {
                                    //获取“指标组”。
                                    var indicatorGroup = context.IndicatorGroups.Where(i => i.IndicatorGroupId == indicatorGourpId).FirstOrDefault();

                                    //处理“指标组”不存在。
                                    if (indicatorGroup == null)
                                    {
                                        throw new Exception("不存在指标组ID：" + indicatorGourpId.ToString());
                                    }

                                    //获取“指标组”对应的“指标”。
                                    var queryIndicators = context.IndicatorGroups.Where(i => i.IndicatorGroupId == indicatorGourpId).SelectMany(c => c.IndicatorGroupMapIndicators).Select(c => c.Indicator);

                                    //遍历“指标”。
                                    foreach (var indicator in queryIndicators)
                                    {
                                        //新增“指标列”。
                                        var newReportRowIndicator = new ReportRowIndicator();
                                        newReportRow.ReportRowIndicators.Add(newReportRowIndicator);

                                        var departmentIndicatorValue = context.DepartmentIndicatorValues.Where(i => i.DepartmentId == department.DepartmentId && i.IndicatorId == indicator.IndicatorId && i.Time == tempTime).FirstOrDefault();

                                        //newReportRowIndicator.Value = GetDepartmentIndicatorValueValue(context, department.DepartmentId, indicator.IndicatorId, tempTime, tempTime);
                                        newReportRowIndicator.IndicatorGroupName = indicatorGroup.IndicatorGroupName;
                                        newReportRowIndicator.IndicatorGroupPriority = indicatorGroup.Priority;
                                        newReportRowIndicator.IndicatorName = indicator.IndicatorName;
                                        newReportRowIndicator.IndicatorPriority = indicator.Priority;
                                    }
                                }
                            }

                            //季度
                            if (tempTime.Month == 3 || tempTime.Month == 6 || tempTime.Month == 9 || tempTime.Month == 12)
                            {

                            }

                            tempTime = tempTime.AddMonths(1);
                        };
                    }
                }
            }
        }

        private static decimal? GetDepartmentIndicatorValueValue(Models.ImsDbContext context, Guid departmentId, Guid indicatorId, DateTime startTime, DateTime endTime)
        {
            var indicatorAlgorithm = context.IndicatorAlgorithms.Where(i => i.ResultId == indicatorId).FirstOrDefault();

            if (indicatorAlgorithm != null)
            {
                if (string.IsNullOrEmpty(indicatorAlgorithm.OperationMethod))
                    throw new Exception("操作符为空。");

                decimal? operand1 = GetDepartmentIndicatorValueValue(context, departmentId, indicatorAlgorithm.FirstOperandID, startTime, endTime);
                decimal? operand2 = GetDepartmentIndicatorValueValue(context, departmentId, indicatorAlgorithm.SecondOperandID, startTime, endTime);

                switch (indicatorAlgorithm.OperationMethod)
                {
                    case ("addition"):
                        return operand1 + operand2;
                    case ("subtraction"):
                        return operand1 - operand2;
                    case ("multiplication"):
                        return operand1 * operand2;
                    case ("division"):
                        return (operand2 == decimal.Zero) ? null : operand1 / operand2; //除数为0时，返回null。
                    default:
                        return null;
                }
            }
            else
            {
                var queryDepartmentIndicatorValue = context.DepartmentIndicatorValues.Where(i => i.IndicatorId == indicatorId && i.DepartmentId == departmentId && i.Time <= endTime && i.Time >= startTime);

                if (queryDepartmentIndicatorValue.Any())
                {
                    decimal? returnedValue = queryDepartmentIndicatorValue.Sum(i => i.Value);
                    return returnedValue;
                }
                else
                {
                    return null;
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
        public decimal? Value;

        /// <summary>
        /// 是否超“标准值”。
        /// </summary>
        public bool? OutOfStandard;
    }






}

namespace IMS2.Models
{
    public partial class DepartmentIndicatorValue
    {
        public bool OutOfStardard()
        {
            if (DepartmentIndicatorStandard != null)
            {
                if (DepartmentIndicatorStandard.LowerBoundIncluded == true)
                {
                    if (Value < DepartmentIndicatorStandard.LowerBound)
                        return true;
                }
                else if (DepartmentIndicatorStandard.LowerBoundIncluded == false)
                {
                    if (Value <= DepartmentIndicatorStandard.LowerBound)
                        return true;
                }

                if (DepartmentIndicatorStandard.UpperBoundIncluded == true)
                {
                    if (Value > DepartmentIndicatorStandard.UpperBound)
                        return true;
                }
                else if (DepartmentIndicatorStandard.UpperBoundIncluded == false)
                {
                    if (Value >= DepartmentIndicatorStandard.UpperBound)
                        return true;
                }

                return false;
            }
            else
            {
                return false;
            }
        }
    }
}