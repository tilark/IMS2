using IMS2.BusinessModel.AlgorithmModel;
using IMS2.Models;
using IMS2.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IMS2.BusinessModel.SatisticsValueModel
{
    public class SatisticsValueUseDbContext : ISatisticsValue
    {
        private IAlgorithmOperation algorithmOperation;
        public SatisticsValueUseDbContext(IAlgorithmOperation algorithmOperation)
        {
            this.algorithmOperation = algorithmOperation;          
        }
        public async Task<decimal?> GetSatisticsValue(Guid indicatorID, Guid durationId, Guid departmentId, DateTime time)
        {

            decimal? result = null;
            //查找算法
            IndicatorAlgorithm algorithm = await GetAlgorithm(indicatorID);
            if (algorithm != null)
            {
                //如果找到
                //判断是否是除法运算
                if ((OperationMethod)Enum.Parse(typeof(OperationMethod), algorithm.OperationMethod) == OperationMethod.division)
                {
                    //如果是除法，通过算法计算获得指定指标的值
                    result = await GetDepartmentIndicatorTimeValueFromAlgorithm(indicatorID, durationId, departmentId, time, algorithm);
                    return result;
                }
                else
                {
                    //如果是非除运算，如乘、加、减等

                    result = await GetBaseValueOrAggregationValueFromAlgorithm(indicatorID, durationId, departmentId, time, algorithm);
                    return result;
                }
            }
            else
            {
                //未找到算法，说明该指标是基础值
                result = await GetBaseValueOrAggregationValueWithoutAlgorithm(indicatorID, durationId, departmentId, time);
                return result;
            }
        }

        #region Private Method
        /// <summary>
        /// 获得指定Indicator的相关算法
        /// </summary>
        /// <param name="departmentIndicatorDurationTime"></param>
        /// <returns>如果在算法表中找到算法，则返回该算法操作，否则返回null</returns>
        private async Task<IndicatorAlgorithm> GetAlgorithm(Guid indicatorID)
        {
            try
            {
                IndicatorAlgorithm algorithm = null;
                using (var context = new ImsDbContext())
                {
                    algorithm = await context.IndicatorAlgorithms.Where(a => a.ResultId == indicatorID).FirstOrDefaultAsync();

                }

                return algorithm;

            }
            catch (Exception)
            {

                return null;
            }
        }

        /// <summary>
        /// 通过算法计算出值 
        /// </summary>
        /// <param name="departmentIndicatorDurationTime">指定指标</param>
        /// <param name="algorithm">指定算法</param>
        /// <returns></returns>       

        private async Task<decimal?> GetDepartmentIndicatorTimeValueFromAlgorithm(Guid indicatorID, Guid durationId, Guid departmentId, DateTime time, IndicatorAlgorithm algorithm)
        {
            decimal? result = null;
            var operand1Value = await GetSatisticsValue(algorithm.FirstOperandID, durationId, departmentId, time);
            var operand2Value = await GetSatisticsValue(algorithm.SecondOperandID, durationId, departmentId, time);
            if (operand1Value != null && operand2Value != null)
            {
                try
                {
                    result = this.algorithmOperation.GetAlgorithmOperationValue(operand1Value.Value, operand2Value.Value, algorithm.OperationMethod);
                }
                catch (Exception)
                {

                    result = null;
                }
            }
            return result;
        }

        /// <summary>
        /// 级别分 有三种情况，一种是低级别 小于 0，直接返回null，一种是同级别 = 0，通过算法计算出值 大于 0，另一种是高级别，是聚合标标。
        /// </summary>
        /// <param name="indicatorID"></param>
        /// <param name="durationId"></param>
        /// <param name="departmentId"></param>
        /// <param name="time"></param>
        /// <param name="algorithm"></param>
        /// <returns></returns>
        private async Task<decimal?> GetBaseValueOrAggregationValueFromAlgorithm(Guid indicatorID, Guid durationId, Guid departmentId, DateTime time, IndicatorAlgorithm algorithm)
        {
            decimal? result = null;
            int level = await CompareIndicatorDuration(indicatorID, durationId);
            if (level == 0)
            {
                if (algorithm != null)
                {
                    //是最初级别，且有算法,通过算法计算出值
                    result = await GetDepartmentIndicatorTimeValueFromAlgorithm(indicatorID, durationId, departmentId, time, algorithm);
                }
                else
                {
                    //无算法，直接从基础数据表中获得
                    result = await GetDepartmentIndicatorTimeValueFromBaseData(indicatorID, durationId, departmentId, time);
                }
                return result;
            }
            else if (level > 0)
            {

                //说明是聚合指标，需获得该跨度的下一级别，如年需获得半年，半年下一级为季
                result = await CalculateAggregationDepartmentIndicatorTimeValue(indicatorID, durationId, departmentId, time);
                return result;
            }
            else
            {
                ////不是最初级别，也有可能是低级别
                return null;
            }
        }

        /// <summary>
        /// 输入指标为聚合统计类指标，迭代到下一级跨度，直到找到最初的跨度，再进行累加
        /// </summary>
        /// <param name="departmentIndicatorDurationTime"></param>
        /// <returns></returns>

        private async Task<decimal?> CalculateAggregationDepartmentIndicatorTimeValue(Guid indicatorID, Guid durationId, Guid departmentId, DateTime time)
        {
            decimal? result = 0M;
            //获得下一级别的跨度，如年的下一级跨度为半年，半年的下一级跨度为季
            //需要根据跨度，重新调整时间

            //newDepartmentIndicatorDurationTime.Update(departmentIndicatorDurationTime);
            var times = new DateTime[3];
            var lowerLevelDurationId =  GetLowerLevelDurationID(durationId, time, out times);
            if (times == null || times.All(a => a.Equals(DateTime.MinValue)))
            {
                //如果是其他月份，直接返回Null
                return null;
            }
            for (int i = 0; i < times.Length; i++)
            {
                if (times[i] != DateTime.MinValue)
                {
                    var currentTime = times[i];
                    try
                    {
                        var resultTemp = await GetSatisticsValue(indicatorID, lowerLevelDurationId, departmentId, currentTime);
                        if (resultTemp != null)
                        {
                            result += resultTemp;
                        }
                        else
                        {
                            result = null;
                            break;
                        }
                    }
                    catch (Exception)
                    {
                        result = null;
                        break;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 获得跨度的下一级别的跨度ID，如年的下一级跨度为半年，半年的下一级跨度为季
        /// </summary>
        /// <param name="durationId"></param>
        /// <returns>如果找到下一级别的跨度ID，则返回，否则返回Guid.Empty</returns>
        private Guid GetLowerLevelDurationID(Guid durationId, DateTime time, out DateTime[] times)
        {
            Guid result = Guid.Empty;
            times = new DateTime[3];
            Duration duration = null;
            using(var context = new ImsDbContext())
            {
                duration =  context.Durations.Find(durationId);

            }
            if (duration != null)
            {
                if (duration.IsYearDuration())
                {
                    if (time.Month == 1)
                    {
                        //全年
                        times[0] = new DateTime(time.Year, 1, 1);
                        times[1] = new DateTime(time.Year, 7, 1);
                    }
                }
                else if (duration.HalfYearDuration())
                {
                    //上半年
                    if (time.Month == 1)
                    {
                        times[0] = new DateTime(time.Year, 1, 1);
                        times[1] = new DateTime(time.Year, 4, 1);
                    }
                    else if (time.Month == 7)
                    {
                        //下半年               
                        times[0] = new DateTime(time.Year, 7, 1);
                        times[1] = new DateTime(time.Year, 10, 1);
                    }
                }
                else if (duration.SeasonDuration())
                {
                    if (time.Month == 1)
                    {
                        //第一季度
                        times[0] = new DateTime(time.Year, 1, 1);
                        times[1] = new DateTime(time.Year, 2, 1);
                        times[2] = new DateTime(time.Year, 3, 1);
                    }
                    else if (time.Month == 4)
                    {
                        //第二季度
                        times[0] = new DateTime(time.Year, 4, 1);
                        times[1] = new DateTime(time.Year, 5, 1);
                        times[2] = new DateTime(time.Year, 6, 1);
                    }
                    else if (time.Month == 7)
                    {
                        //第三季度
                        times[0] = new DateTime(time.Year, 7, 1);
                        times[1] = new DateTime(time.Year, 8, 1);
                        times[2] = new DateTime(time.Year, 9, 1);
                    }
                    else if (time.Month == 10)
                    {
                        //第四季度
                        times[0] = new DateTime(time.Year, 10, 1);
                        times[1] = new DateTime(time.Year, 11, 1);
                        times[2] = new DateTime(time.Year, 12, 1);
                    }
                }
                else if (duration.MonthDuration())
                {
                    times[0] = new DateTime(time.Year, time.Month, 1);
                }
                else
                {
                    //如果是其他月份

                }
                if(duration.NextLevel() != 0)
                {
                    using (var context = new ImsDbContext())
                    {
                        var nextLevelDuration = context.Durations.ToList().Where(a => a.Level == duration.NextLevel()).FirstOrDefault();
                        result = nextLevelDuration != null ? nextLevelDuration.DurationId : Guid.Empty;
                    }
                }                           
            }
            return result;
        }
        /// <summary>
        /// 从基本值表中获得规定的值，并且是已审核的值
        /// </summary>
        /// <param name="indicatorID"></param>
        /// <param name="durationId"></param>
        /// <param name="departmentId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private async Task<decimal?> GetDepartmentIndicatorTimeValueFromBaseData(Guid indicatorID, Guid durationId, Guid departmentId, DateTime time)
        {
            decimal? result = null;
            DepartmentIndicatorValue query = null;
            using (var context = new ImsDbContext())
            {
                query = context.DepartmentIndicatorValues.Where(a => a.IndicatorId == indicatorID && a.DepartmentId == departmentId && a.IsLocked == true && a.Time == time).FirstOrDefault();
            }        

            if (query != null)
            {
                result = query.Value.HasValue ? query.Value : null;
            }
            else
            {
                result = null;
            }
            return result;
        }

        private async Task<decimal?> GetBaseValueOrAggregationValueWithoutAlgorithm(Guid indicatorID, Guid durationId, Guid departmentId, DateTime time)
        {
            return await GetBaseValueOrAggregationValueFromAlgorithm(indicatorID, durationId, departmentId, time, null);
        }
        /// <summary>
        /// 判断指标的时段与目标的时段的级别，是低、同等、高级
        /// </summary>
        /// <param name="indicatorID"></param>
        /// <param name="durationId"></param>
        /// <returns>低级别小于0，同级别等于0，高级别大于0</returns>
        private async Task<int> CompareIndicatorDuration(Guid indicatorID, Guid durationId)
        {
            //默认是低级别
            int result = -1;
            Duration durationOfIndicator = null;
            //找到指标所属的时段
            using (var context = new ImsDbContext())
            {
                durationOfIndicator = context.Indicators.Include("Duration").Where(a => a.IndicatorId == indicatorID).Select(a => a.Duration).FirstOrDefault();
                if (durationOfIndicator != null)
                {
                    //找到时段，判断是否是同级别。

                    //var durationRepo = new DurationRepositoryAsync(this.unitOfWork);
                    //var duration = await durationRepo.GetAll(a => a.DurationId == durationId).FirstOrDefaultAsync();
                    var duration = await context.Durations.FindAsync(durationId);

                    if (duration != null)
                    {
                        result = duration.Level - durationOfIndicator.Level;
                    }
                }
            }
           
            return result;
        }

        #endregion
    }
}