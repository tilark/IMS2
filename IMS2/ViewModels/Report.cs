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
    /// <remarks>
    /// 表示整个报表。
    /// 初始化后，执行GetData()即可获取数据。
    /// 可以直接遍历实例，获取“报表行”。
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
        /// “科室分类ID”集合。保存传入参数。
        /// </summary>
        private List<Guid> departmentCategoryIds;

        /// <summary>
        /// “指标组ID”集合。保存传入参数。
        /// </summary>
        private List<Guid> IndicatorGroupIds;

        /// <summary>
        /// “报表行”集合。
        /// </summary>
        public List<ReportRow> reportRows;





        /// <summary>
        /// 初始化报表<see cref="Report"/>。
        /// </summary>
        /// <param name="departmentCategoryIds">“科室分类ID”集合。</param>
        /// <param name="IndicatorGroupIds">“指标组ID”集合。</param>
        /// <param name="startTime">期初时段。</param>
        /// <param name="endTime">期末时段。</param>
        /// <exception cref="ArgumentNullException">“departmentCategoryIds”或“IndicatorGroupIds”为null。</exception>
        public Report(IEnumerable<Guid> departmentCategoryIds, IEnumerable<Guid> IndicatorGroupIds, DateTime startTime, DateTime endTime)
        {
            //判断实参合法性。
            if (departmentCategoryIds == null)
            {
                throw new ArgumentNullException("departmentCategoryIds");
            }
            if (IndicatorGroupIds == null)
            {
                throw new ArgumentNullException("IndicatorGroupIds");
            }

            //保存实参。
            this.departmentCategoryIds = new List<Guid>(departmentCategoryIds);
            this.IndicatorGroupIds = new List<Guid>(IndicatorGroupIds);
            this.startTime = startTime;
            this.endTime = endTime;

            //初始化其他字段。
            Title = "自定义报表";
            reportRows = new List<ReportRow>();
        }

        /// <summary>
        /// 获取数据。
        /// </summary>
        /// <exception cref="Exception">查看内部描述。</exception>
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
                    var departments = context.DepartmentCategories.Where(i => i.DepartmentCategoryId == departmentCategoryId).SelectMany(c => c.Departments).ToList();

                    //遍历每个“科室”。
                    foreach (var department in departments)
                    {
                        //遍历时间段，在循环中逐月递增。
                        for (DateTime tempTime = startTime; tempTime <= endTime; tempTime = tempTime.AddMonths(1))
                        {
                            //“月”行。“月”行是一定有的，不需要if进行判断，直接进入。
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
                                    var Indicators = context.IndicatorGroups.Where(i => i.IndicatorGroupId == indicatorGourpId).SelectMany(c => c.IndicatorGroupMapIndicators).Select(c => c.Indicator).ToList();

                                    //遍历“指标”。
                                    foreach (var indicator in Indicators)
                                    {
                                        //新增“指标列”。
                                        var newReportRowIndicator = new ReportRowIndicator();
                                        newReportRow.ReportRowIndicators.Add(newReportRowIndicator);

                                        newReportRowIndicator.IndicatorGroupName = indicatorGroup.IndicatorGroupName;
                                        newReportRowIndicator.IndicatorGroupPriority = indicatorGroup.Priority;
                                        newReportRowIndicator.IndicatorName = indicator.IndicatorName;
                                        newReportRowIndicator.IndicatorPriority = indicator.Priority;

                                        //判断当前“指标”的“跨度”。分析出是“独立项”还是“整合项”。
                                        if (indicator.DurationId == new Guid("d48aa438-ad71-4419-a2a2-a1c390f6c097"))
                                        //如果为“月”，则为“独立项”。
                                        {
                                            var departmentIndicatorValue = context.DepartmentIndicatorValues.Include("DepartmentIndicatorStandard").Where(i => i.DepartmentId == department.DepartmentId && i.IndicatorId == indicator.IndicatorId && i.Time == newReportRow.startTime && i.IsLocked == true).FirstOrDefault();

                                            newReportRowIndicator.Value = departmentIndicatorValue?.Value;
                                            newReportRowIndicator.OutOfStandard = departmentIndicatorValue?.OutOfStardard();
                                        }
                                        else
                                        //如果不为“月”，应留空。因为处理“月”行的时候，不应涉及其他非“月”的“跨度”。
                                        {
                                            newReportRowIndicator.Value = null;
                                            newReportRowIndicator.OutOfStandard = null;
                                        }
                                    }
                                }
                            }

                            //“季”行。“季”行存在“独立项”，也存在“整合项”。
                            if ((tempTime.Month == 3 || tempTime.Month == 6 || tempTime.Month == 9 || tempTime.Month == 12) && startTime <= tempTime.AddMonths(-2))
                            {
                                //新增“报表行”。
                                var newReportRow = new ReportRow();
                                reportRows.Add(newReportRow);

                                newReportRow.startTime = tempTime.AddMonths(-2);
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
                                    var Indicators = context.IndicatorGroups.Where(i => i.IndicatorGroupId == indicatorGourpId).SelectMany(c => c.IndicatorGroupMapIndicators).Select(c => c.Indicator).ToList();

                                    //遍历“指标”。
                                    foreach (var indicator in Indicators)
                                    {
                                        //新增“指标列”。
                                        var newReportRowIndicator = new ReportRowIndicator();
                                        newReportRow.ReportRowIndicators.Add(newReportRowIndicator);

                                        newReportRowIndicator.IndicatorGroupName = indicatorGroup.IndicatorGroupName;
                                        newReportRowIndicator.IndicatorGroupPriority = indicatorGroup.Priority;
                                        newReportRowIndicator.IndicatorName = indicator.IndicatorName;
                                        newReportRowIndicator.IndicatorPriority = indicator.Priority;

                                        //判断当前“指标”的“跨度”。
                                        if (indicator.DurationId == new Guid("d48aa438-ad71-4419-a2a2-a1c390f6c097"))
                                        //如果为“月”，则为“整合项”。
                                        {
                                            newReportRowIndicator.Value = AggregateDepartmentIndicatorValueValue(context, department.DepartmentId, indicator.IndicatorId, newReportRow.startTime, newReportRow.endTime,false);
                                            newReportRowIndicator.OutOfStandard = null;
                                        }
                                        else if (indicator.DurationId == new Guid("bd18c4f4-6552-4986-ab4e-ba2dffded2b3"))
                                        //如果为“季”，则为“独立项”。
                                        {
                                            var departmentIndicatorValue = context.DepartmentIndicatorValues.Include("DepartmentIndicatorStandard").Where(i => i.DepartmentId == department.DepartmentId && i.IndicatorId == indicator.IndicatorId && i.Time == newReportRow.startTime && i.IsLocked == true).FirstOrDefault();

                                            newReportRowIndicator.Value = departmentIndicatorValue?.Value;
                                            newReportRowIndicator.OutOfStandard = departmentIndicatorValue?.OutOfStardard();
                                        }
                                        else//如果不为“月”、“季”，应留空。
                                        {
                                            newReportRowIndicator.Value = null;
                                            newReportRowIndicator.OutOfStandard = null;
                                        }

                                    }
                                }
                            }

                            //“半年”行。“半年”行存在“独立项”，也存在“整合项”。    
                            if ((tempTime.Month == 6 || tempTime.Month == 12) && startTime <= tempTime.AddMonths(-5))
                            {
                                //新增“报表行”。
                                var newReportRow = new ReportRow();
                                reportRows.Add(newReportRow);

                                newReportRow.startTime = tempTime.AddMonths(-5);
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
                                    var Indicators = context.IndicatorGroups.Where(i => i.IndicatorGroupId == indicatorGourpId).SelectMany(c => c.IndicatorGroupMapIndicators).Select(c => c.Indicator).ToList();

                                    //遍历“指标”。
                                    foreach (var indicator in Indicators)
                                    {
                                        //新增“指标列”。
                                        var newReportRowIndicator = new ReportRowIndicator();
                                        newReportRow.ReportRowIndicators.Add(newReportRowIndicator);

                                        newReportRowIndicator.IndicatorGroupName = indicatorGroup.IndicatorGroupName;
                                        newReportRowIndicator.IndicatorGroupPriority = indicatorGroup.Priority;
                                        newReportRowIndicator.IndicatorName = indicator.IndicatorName;
                                        newReportRowIndicator.IndicatorPriority = indicator.Priority;

                                        //判断当前“指标”的“跨度”。
                                        if (indicator.DurationId == new Guid("d48aa438-ad71-4419-a2a2-a1c390f6c097") || indicator.DurationId == new Guid("bd18c4f4-6552-4986-ab4e-ba2dffded2b3"))
                                        //如果为“月”、“季”，则为“整合项”。
                                        {
                                            newReportRowIndicator.Value = AggregateDepartmentIndicatorValueValue(context, department.DepartmentId, indicator.IndicatorId, newReportRow.startTime, newReportRow.endTime,false);
                                            newReportRowIndicator.OutOfStandard = null;
                                        }
                                        else if (indicator.DurationId == new Guid("24847114-90e4-483d-b290-97781c3fa0c2"))
                                        //如果为“半年”，则为“独立项”。
                                        {
                                            var departmentIndicatorValue = context.DepartmentIndicatorValues.Include("DepartmentIndicatorStandard").Where(i => i.DepartmentId == department.DepartmentId && i.IndicatorId == indicator.IndicatorId && i.Time == newReportRow.startTime && i.IsLocked == true).FirstOrDefault();

                                            newReportRowIndicator.Value = departmentIndicatorValue?.Value;
                                            newReportRowIndicator.OutOfStandard = departmentIndicatorValue?.OutOfStardard();
                                        }
                                        else
                                        //如果不为“月”、“季”、“半年”，应留空。
                                        {
                                            newReportRowIndicator.Value = null;
                                            newReportRowIndicator.OutOfStandard = null;
                                        }
                                    }
                                }
                            }

                            //“年”行。“年”行存在“独立项”，也存在“整合项”。 
                            if ((tempTime.Month == 12) && startTime <= tempTime.AddMonths(-11))
                            {
                                //新增“报表行”。
                                var newReportRow = new ReportRow();
                                reportRows.Add(newReportRow);

                                newReportRow.startTime = tempTime.AddMonths(-11);
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
                                    var Indicators = context.IndicatorGroups.Where(i => i.IndicatorGroupId == indicatorGourpId).SelectMany(c => c.IndicatorGroupMapIndicators).Select(c => c.Indicator).ToList();

                                    //遍历“指标”。
                                    foreach (var indicator in Indicators)
                                    {
                                        //新增“指标列”。
                                        var newReportRowIndicator = new ReportRowIndicator();
                                        newReportRow.ReportRowIndicators.Add(newReportRowIndicator);

                                        newReportRowIndicator.IndicatorGroupName = indicatorGroup.IndicatorGroupName;
                                        newReportRowIndicator.IndicatorGroupPriority = indicatorGroup.Priority;
                                        newReportRowIndicator.IndicatorName = indicator.IndicatorName;
                                        newReportRowIndicator.IndicatorPriority = indicator.Priority;

                                        //判断当前“指标”的“跨度”。
                                        if (indicator.DurationId == new Guid("d48aa438-ad71-4419-a2a2-a1c390f6c097") || indicator.DurationId == new Guid("bd18c4f4-6552-4986-ab4e-ba2dffded2b3") || indicator.DurationId == new Guid("24847114-90e4-483d-b290-97781c3fa0c2"))
                                        //如果为“月”、“季”、“半年”，则为“整合项”。
                                        {
                                            newReportRowIndicator.Value = AggregateDepartmentIndicatorValueValue(context, department.DepartmentId, indicator.IndicatorId, newReportRow.startTime, newReportRow.endTime,false);
                                            newReportRowIndicator.OutOfStandard = null;
                                        }
                                        else if (indicator.DurationId == new Guid("ba74e352-0ad5-424b-bf31-738ba5666649"))
                                        //如果为“年”，则为“独立项”。
                                        {
                                            var departmentIndicatorValue = context.DepartmentIndicatorValues.Include("DepartmentIndicatorStandard").Where(i => i.DepartmentId == department.DepartmentId && i.IndicatorId == indicator.IndicatorId && i.Time == newReportRow.startTime && i.IsLocked == true).FirstOrDefault();

                                            newReportRowIndicator.Value = departmentIndicatorValue?.Value;
                                            newReportRowIndicator.OutOfStandard = departmentIndicatorValue?.OutOfStardard();
                                        }
                                        else
                                        //如果不为“月”、“季”、“半年”、“年”，应留空。
                                        {
                                            newReportRowIndicator.Value = null;
                                            newReportRowIndicator.OutOfStandard = null;
                                        }
                                    }
                                }
                            }

                            //“合计”行。所有项均作为“整合项”。只有一个月份时不出“合计”行。
                            if ((tempTime == endTime) && (startTime != endTime))
                            {
                                //新增“报表行”。
                                var newReportRow = new ReportRow();
                                reportRows.Add(newReportRow);

                                newReportRow.startTime = startTime;
                                newReportRow.endTime = endTime;
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
                                    var Indicators = context.IndicatorGroups.Where(i => i.IndicatorGroupId == indicatorGourpId).SelectMany(c => c.IndicatorGroupMapIndicators).Select(c => c.Indicator).ToList();

                                    //遍历“指标”。
                                    foreach (var indicator in Indicators)
                                    {
                                        //新增“指标列”。
                                        var newReportRowIndicator = new ReportRowIndicator();
                                        newReportRow.ReportRowIndicators.Add(newReportRowIndicator);

                                        newReportRowIndicator.IndicatorGroupName = indicatorGroup.IndicatorGroupName;
                                        newReportRowIndicator.IndicatorGroupPriority = indicatorGroup.Priority;
                                        newReportRowIndicator.IndicatorName = indicator.IndicatorName;
                                        newReportRowIndicator.IndicatorPriority = indicator.Priority;

                                        //不需要判断当前“指标”的“跨度”。均按“整合项”处理。
                                        newReportRowIndicator.Value = AggregateDepartmentIndicatorValueValue(context, department.DepartmentId, indicator.IndicatorId, newReportRow.startTime, newReportRow.endTime,false);
                                        newReportRowIndicator.OutOfStandard = null;
                                    }
                                }
                            }
                        };
                    }
                }
            }
        }

        /// <summary>
        /// 整合“科室指标值”的值。
        /// </summary>
        /// <param name="context">数据上下文。</param>
        /// <param name="departmentId">“科室ID”。</param>
        /// <param name="indicatorId">“指标ID”。</param>
        /// <param name="startTime">期初时段。</param>
        /// <param name="endTime">期末时段。</param>
        /// <param name="onlyCalculation">限定仅返回“计算”项的“值”。</param>
        /// <returns>指点时段区间内的指定“科室”的指定“指标”的整合值。</returns>
        /// <exception cref="ArgumentNullException">实参为null。</exception>
        /// <exception cref="Exception">查看内部描述。</exception>
        /// <remarks>不会判断指标的“跨度”。</remarks>
        internal static decimal? AggregateDepartmentIndicatorValueValue(Models.ImsDbContext context, Guid departmentId, Guid indicatorId, DateTime startTime, DateTime endTime, bool onlyCalculation = true)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            //在“算法”表中查找“结果ID”与“指标ID”相同的记录。
            var indicatorAlgorithm = context.IndicatorAlgorithms.Where(i => i.ResultId == indicatorId).FirstOrDefault();

            if (indicatorAlgorithm != null)
            //如果存在，则调用算法。
            {
                if (string.IsNullOrEmpty(indicatorAlgorithm.OperationMethod))
                    throw new Exception("操作符为空。");

                decimal? operand1 = AggregateDepartmentIndicatorValueValue(context, departmentId, indicatorAlgorithm.FirstOperandID, startTime, endTime, false);
                decimal? operand2 = AggregateDepartmentIndicatorValueValue(context, departmentId, indicatorAlgorithm.SecondOperandID, startTime, endTime, false);

                decimal? result;

                switch (indicatorAlgorithm.OperationMethod)
                {
                    case ("addition"):
                        result = operand1 + operand2;
                        break;
                    case ("subtraction"):
                        result = operand1 - operand2;
                        break;
                    case ("multiplication"):
                        result = operand1 * operand2;
                        break;
                    case ("division"):
                        result = (operand2 == decimal.Zero) ? null : operand1 / operand2; //除数为0时，返回null。
                        break;
                    default:
                        return null;
                }

                var indicatorUnit = context.Indicators.Where(i => i.IndicatorId == indicatorId).FirstOrDefault().Unit;

                if (result == null)
                    return null;

                if (indicatorUnit == "百分比")
                {
                    return Math.Round(result.Value, 2) * 100;
                }
                else//待补充：根据其他“单位”作相应优化小数位
                {
                    return Math.Round(result.Value, 2);
                }
            }
            else
            //如果不存在，则查找“科室指标表”。
            {
                if (onlyCalculation == true)
                {
                    return null;
                }

                var queryDepartmentIndicatorValue = context.DepartmentIndicatorValues.Where(i => i.IndicatorId == indicatorId && i.DepartmentId == departmentId && i.Time <= endTime && i.Time >= startTime);

                if (queryDepartmentIndicatorValue.Any())
                {
                    if (queryDepartmentIndicatorValue.Any(c => c.IsLocked == false))
                    {
                        return null;
                    }

                    var returnedValue = queryDepartmentIndicatorValue.Sum(i => i.Value);
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
            else if ((startTime.Month == 1 || startTime.Month == 4 || startTime.Month == 7 || startTime.Month == 10) && (endTime.Year == startTime.Year && startTime.Month + 2 == endTime.Month))
            {
                return startTime.Year + "年第" + ((startTime.Month - 1) / 3 + 1) + "季度";
            }//表示一个半年
            else if ((startTime.Month == 1 || startTime.Month == 7) && (endTime.Year == startTime.Year && startTime.Month + 5 == endTime.Month))
            {
                if (startTime.Month == 1)
                    return startTime.Year + "年上半年";
                else
                    return startTime.Year + "年下半年";
            }
            else if ((startTime.Month == 1) && (endTime.Year == startTime.Year && startTime.Month + 11 == endTime.Month))
            {
                return startTime.Year + "年全年";
            }
            else
            {
                return "合计";
            }
        }





        public IEnumerator<ReportRowIndicator> GetEnumerator()
        {
            return ((IEnumerable<ReportRowIndicator>)ReportRowIndicators).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
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
        public bool? OutOfStardard()
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
                return null;
            }
        }
    }
}