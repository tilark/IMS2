using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using IMS2.RepositoryAsync;
using IMS2.BusinessModel.SatisticsValueModel;
using IMS2.BusinessModel.AlgorithmModel;
using IMS2.Models;

namespace IMS2.BusinessModel.ObserverMode.Dad
{
    /// <summary>
    /// 新值表观察者。
    /// </summary>
    /// <remarks>在“基于值表变动动态更新虚拟值表机制”中，实现具体观察者功能。对值表的变动进行虚拟值表的改动，包括新增、更新与删除。</remarks>
    /// <see cref="基于值表变动动态更新虚拟值表机制"/>
    /// <example>
    /// 初始化后，将被动调用Update执行虚拟值的批量修改。
    /// </example>
    public class VirtualValueObserver : IObserver
    {
        /// <summary>
        /// 初始化。
        /// </summary>
        public VirtualValueObserver(RepositoryAsync.IDomainUnitOfWork unitOfWork, ISatisticsValue satisticsValue)
        {
            this.unitOfWork = unitOfWork;
            this.satisticsValue = satisticsValue;
            //this.satisticsValue = new SatisticsValue(new AlgorithmOperationImpl(), unitOfWork);
            //this.repo = new DepartmentIndicatorDurationVirtualValueRepositoryAsync(unitOfWork);
        }

        /// <summary>
        /// 初始化。
        /// </summary>
        public VirtualValueObserver(ISatisticsValue satisticsValue)
        {
            this.satisticsValue = satisticsValue;
        }





        private ISatisticsValue satisticsValue;
        private IDomainUnitOfWork unitOfWork;





        /// <summary>
        /// 变动对象。
        /// </summary>
        /// <remarks>应赋值为“值表变动对象”。</remarks>
        public ISubject Subject { get; set; }





        /// <summary>
        /// 更新。
        /// </summary>
        /// <remarks>受触发后而执行的更新操作，批量改动与“变动对象”相关联的所有“虚拟值表”记录。</remarks>
        /// <see cref="基于值表变动动态更新虚拟值表机制"/>
        public void Update()
        {
            //var db = new Models.ImsDbContext();

            var originIsLocked = (this.Subject as DepartmentIndicatorValueSubject).IsLocked;

            var originDepartmentId = (this.Subject as DepartmentIndicatorValueSubject).DepartmentId;
            var originIndicatorId = (this.Subject as DepartmentIndicatorValueSubject).IndicatorId;
            var originTime = (this.Subject as DepartmentIndicatorValueSubject).Time;

            var indicatorRelativeIndicatorAlgorithmSearchingAlgorithm = new BusinessModel.IndicatorRelativeIndicatorAlgorithmSearchingAlgorithm.IndicatorRelativeIndicatorAlgorithmSearchingAlgorithm();
            var resultIds = indicatorRelativeIndicatorAlgorithmSearchingAlgorithm.Find(originIndicatorId);
            var indicatorIds = resultIds.ToList();
            indicatorIds.Add(originIndicatorId);

            var indicator = new Indicator();
            using (var context = new ImsDbContext())
            {
                indicator = context.Indicators.Find(originIndicatorId);
            }
            if (indicator == null)
            {
                return;
            }
            var durationTimeSolver = new BusinessModel.DurationTime.DurationTimeSolver();
            var durationTimeList = durationTimeSolver.Solve(indicator.DurationId.Value, originTime);

            if (originIsLocked)
            {
                foreach (var indicatorId in indicatorIds)
                    foreach (var durationTimeItem in durationTimeList)
                    {
                        this.UpdateOrCreateVirturlValue(indicatorId, originDepartmentId, durationTimeItem.DurationId, durationTimeItem.Time);
                    }
            }
            else
            {
                foreach (var indicatorId in indicatorIds)
                    foreach (var durationTimeItem in durationTimeList)
                    {
                        this.RemoveVirturlValue(indicatorId, originDepartmentId, durationTimeItem.DurationId, durationTimeItem.Time);
                    }
            }
        }

        /// <summary>
        /// 更新或新增虚拟值。
        /// </summary>
        /// <param name="indicatorId">指标ID。</param>
        /// <param name="departmentId">科室ID。</param>
        /// <param name="durationId">时段ID。</param>
        /// <param name="time">时间。</param>
        /// <remarks>若对应虚拟值已存在，则更新，否则进行新增。</remarks>
        private async void UpdateOrCreateVirturlValue(Guid indicatorId, Guid departmentId, Guid durationId, DateTime time)
        {
            try
            {
                //var value = this.satisticsValue.GetSatisticsValue(indicatorId, durationId, departmentId, time).Result;

                var value = await this.satisticsValue.GetSatisticsValue(indicatorId, durationId, departmentId, time);

                if (value != null)
                {
                    var item = new DepartmentIndicatorDurationTimeVirtualValueView
                    {
                        IndicatorId = indicatorId,
                        DepartmentId = departmentId,
                        DurationId = durationId,
                        Time = time,
                        Value = value.Value
                    };
                    try
                    {
                        item.UpdateOrCreateIfNotExistDepartmentIndicatorDurationVirtualValue();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 删除虚拟值。
        /// </summary>
        /// <param name="indicatorId">指标ID。</param>
        /// <param name="departmentId">科室ID。</param>
        /// <param name="durationId">时段ID。</param>
        /// <param name="time">时间。</param>
        /// <remarks>若对应虚拟值已存在，则删除，否则不出错。</remarks>
        private void RemoveVirturlValue(Guid indicatorId, Guid departmentId, Guid durationId, DateTime time)
        {
            var item = new DepartmentIndicatorDurationTimeVirtualValueView
            {
                IndicatorId = indicatorId,
                DepartmentId = departmentId,
                DurationId = durationId,
                Time = time
            };
            try
            {
                item.RemoveDepartmentIndicatorDurationVirtualValue();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}