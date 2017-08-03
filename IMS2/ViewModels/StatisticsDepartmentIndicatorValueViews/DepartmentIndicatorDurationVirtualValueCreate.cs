using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using IMS2.RepositoryAsync;
using IMS2.BusinessModel.SatisticsValueModel;
using IMS2.Models;
using PagedList;
using System.Data.Entity;

namespace IMS2.ViewModels.StatisticsDepartmentIndicatorValueViews
{
    /// <summary>
    /// 通过指标来源科室获得所有的指标，再找到相对应的科室，组合成
    /// </summary>
    public class DepartmentIndicatorDurationVirtualValueCreate
    {
        private IDomainUnitOfWork unitOfWork;
        [Display(Name = "时段")]
        public Guid? DurationId { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM}", ApplyFormatInEditMode = true)]
        [Display(Name = "时间点")]
        public DateTime Time { get; set; }

        [Display(Name = "指标来源科室")]
        public Guid? DataSourceId { get; set; }


        #region PublicMethod
        /// <summary>
        /// 更新新值表
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="satisticsValue"></param>
        /// <param name="datasource">指标数据来源</param>
        /// <returns></returns>
        public async Task UpdateDepartmentIndicatorDurationVirtualTable(IDomainUnitOfWork unitOfWork, ISatisticsValue satisticsValue, DataSourceEnum datasource)
        {
            this.unitOfWork = unitOfWork;
            //获得来源科室对应的指标，再获得该指标对应的科室，构成指标、科室集合
            var indicatorDepartmentList = await GetIndictorDepartmentList(DataSourceId, datasource);

            //获得指定的Duration集合
            var durationList = await GetDurationSelect(DurationId);

            //将指标科室集合、Duration集合和时间组建成科室、指标、跨度、时间的集合
            var indicatorDepartmentDurationTimeList = TransferToIndicatorDepartmentDurationTimeList(indicatorDepartmentList, durationList, Time);

            //从Satistics中获得值
            if (indicatorDepartmentDurationTimeList != null && indicatorDepartmentDurationTimeList.Count > 0)
            {
                //此处需优化
                foreach (var item in indicatorDepartmentDurationTimeList)
                {
                    try
                    {
                        item.Value = await satisticsValue.GetSatisticsValue(item.IndicatorID, item.DurationId, item.DepartmentId, item.Time);
                    }
                    catch (NullReferenceException)
                    {
                        continue;
                    }
                    catch (Exception)
                    {

                        item.Value = null;
                    }

                }
                //同时需计算出该指标所对应的结果指标的值，再写入到数据库
                //写入数据库的新值表中保存
                try
                {
                    await SaveDepartmentIndicatorDurationVirtualValueToDatabase(indicatorDepartmentDurationTimeList);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        #endregion

        #region PrivateMethod


        /// <summary>
        /// 将计算出来的DepartmentIndicatorDurationTime的值写入到数据库
        /// </summary>
        /// <param name="departmentDurationValue"></param>
        /// <returns></returns>
        private async Task SaveDepartmentIndicatorDurationVirtualValueToDatabase(List<DepartmentIndicatorDurationTime> departmentDurationValue)
        {
            var repo = new DepartmentIndicatorDurationVirtualValueRepositoryAsync(unitOfWork);
            foreach (var item in departmentDurationValue.AsParallel().Where(a => a.Value != null).ToList())
            {
                ///先判断在新值表中
                var query = await repo.GetAll(a => a.IndicatorId == item.IndicatorID && a.DepartmentId == item.DepartmentId && a.DurationId == item.DurationId && a.Time == item.Time).FirstOrDefaultAsync();
                if (query != null)
                {
                    ///如果存在，则需更新
                    query.Value = item.Value;
                    query.UpdateTime = DateTime.Now;
                    repo.Update(query);
                }
                else
                {
                    ///不存在，直接添加
                    var temp = new DepartmentIndicatorDurationVirtualValue
                    {
                        DepartmentIndicatorDurationVirtualValueID = Guid.NewGuid(),
                        DepartmentId = item.DepartmentId,
                        IndicatorId = item.IndicatorID,
                        DurationId = item.DurationId,
                        Time = item.Time,
                        Value = item.Value,
                        CreateTime = DateTime.Now,
                        UpdateTime = System.DateTime.Now
                    };
                    repo.Add(temp);
                }
                try
                {
                    await this.unitOfWork.SaveChangesClientWinAsync();
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }
        /// <summary>
        /// 通过指标科室集合，跨度集合和时间，组建成指标科室跨度时间集合
        /// </summary>
        /// <param name="indicatorDepartmentList"></param>
        /// <param name="durationList"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private List<DepartmentIndicatorDurationTime> TransferToIndicatorDepartmentDurationTimeList(List<IndicatorDepartmentViewModel> indicatorDepartmentList, List<Duration> durationList, DateTime time)
        {
            var result = (from i in indicatorDepartmentList.AsParallel()
                          from d in durationList.AsParallel()
                          select new DepartmentIndicatorDurationTime
                          {
                              IndicatorID = i.IndicatorId,
                              DepartmentId = i.DepartmentId,
                              DurationId = d.DurationId,
                              Time = time
                          }).WithMergeOptions(ParallelMergeOptions.NotBuffered).ToList();

            return result;
        }

        /// <summary>
        /// 获得指定的跨度集合，如果指定的DurationId不存在，则获得所有的跨度集合
        /// </summary>
        /// <param name="durationId"></param>
        /// <returns></returns>
        private async Task<List<Duration>> GetDurationSelect(Guid? durationId)
        {
            var durationRepo = new DurationRepositoryAsync(this.unitOfWork);
            var durationList = durationRepo.GetAll();
            if (durationId.HasValue)
            {

                durationList = durationList.Where(a => a.DurationId == durationId.Value);
            }
            return await durationList.ToListAsync();
        }


        /// <summary>
        /// 根据数据来源条件获得IdicatorDepartment
        /// </summary>
        /// <param name="dataSourceId"></param>
        /// <param name="datasource"></param>
        /// <returns></returns>
        private async Task<List<IndicatorDepartmentViewModel>> GetIndictorDepartmentList(Guid? dataSourceId, DataSourceEnum datasource)
        {
            switch (datasource)
            {
                case DataSourceEnum.PROVIDEDEPARTMENT:
                    return await GetIndictorDepartmentListFromProvideDepartment(dataSourceId);
                case DataSourceEnum.DATASOURCESYSTEM:
                    return await GetIndictorDepartmentListFromDataSourceSystem(dataSourceId);

                case DataSourceEnum.DEPARTMENT:
                    return await GetIndictorDepartmentListFromDepartment(dataSourceId);
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// 从科室获得所拥有的指标及该指标对应的所有科室，组建成指标-科室集合
        /// </summary>
        /// <param name="dataSourceId">DepartmentId</param>
        /// <returns></returns>
        private async Task<List<IndicatorDepartmentViewModel>> GetIndictorDepartmentListFromDepartment(Guid? departmentId)
        {
            List<IndicatorDepartmentViewModel> result = new List<IndicatorDepartmentViewModel>();
            //如果providingDepartmentId的值是Guid.Empty，则直接返回Null
            if (departmentId.HasValue && departmentId.Value.Equals(Guid.Empty))
            {
                return null;
            }
            var departmentRepo = new DepartmentRepositoryAsync(this.unitOfWork);

            //需获得所有数据来源系统的所管指标
            var departmentCollection = departmentRepo.GetAll().Include(a => a.Indicators);
            if (departmentId.HasValue)
            {
                //如果指定了来源科室，则直接获得该科室的所管指标
                departmentCollection = departmentCollection.Where(a => a.DepartmentId == departmentId.Value);
            }
            //从数据来源科室中获得的IndicatorId集合

            var departmentIndicatorList = await departmentCollection.Select(a => a.DepartmentCategory).SelectMany(a => a.DepartmentCategoryMapIndicatorGroups).Select(a => a.IndicatorGroup).SelectMany(a => a.IndicatorGroupMapIndicators).Select(a => a.IndicatorId).ToListAsync();

            result = await GetIndicatorDepartmentViewModelList(departmentIndicatorList);
            return result;
        }

        /// <summary>
        /// 从数据来源系统获得所管的指标及该指标对应的所有科室，组建成指标-科室集合
        /// </summary>
        /// <param name="dataSourceId">DataSourceSystemId</param>
        /// <returns></returns>
        private async Task<List<IndicatorDepartmentViewModel>> GetIndictorDepartmentListFromDataSourceSystem(Guid? dataSourceSystemId)
        {
            List<IndicatorDepartmentViewModel> result = new List<IndicatorDepartmentViewModel>();
            //如果providingDepartmentId的值是Guid.Empty，则直接返回Null
            if (dataSourceSystemId.HasValue && dataSourceSystemId.Value.Equals(Guid.Empty))
            {
                return null;
            }
            var dataSourceSystemRepo = new DataSourceSystemRepositoryAsync(this.unitOfWork);

            //需获得所有数据来源系统的所管指标
            var datasourceDepartmentCollection = dataSourceSystemRepo.GetAll().Include(a => a.Indicators);
            if (dataSourceSystemId.HasValue)
            {
                //如果指定了来源科室，则直接获得该科室的所管指标
                datasourceDepartmentCollection = datasourceDepartmentCollection.Where(a => a.DataSourceSystemId == dataSourceSystemId.Value);
            }

            foreach (var dataSource in await datasourceDepartmentCollection.ToListAsync())
            {
                //从数据来源科室中获得的IndicatorId集合
                var dataSourceSystemIndicatorList = dataSource.Indicators.Select(a => a.IndicatorId).ToList();

                var temp = await GetIndicatorDepartmentViewModelList(dataSourceSystemIndicatorList);
                if(temp != null && temp.Count > 0)
                {
                    result.AddRange(temp);
                }

            }
            return result;
        }



        /// <summary>
        /// 根据来源科室获得所管的指标及该指标对应的所有科室，组建成指标-科室集合。
        /// </summary>
        /// <param name="providingDepartmentId"></param>
        /// <returns>List<IndicatorDepartmentViewModel></returns>
        private async Task<List<IndicatorDepartmentViewModel>> GetIndictorDepartmentListFromProvideDepartment(Guid? providingDepartmentId)
        {
            List<IndicatorDepartmentViewModel> result = new List<IndicatorDepartmentViewModel>();
            //如果providingDepartmentId的值是Guid.Empty，则直接返回Null
            if (providingDepartmentId.HasValue && providingDepartmentId.Value.Equals(Guid.Empty))
            {
                return null;
            }
            var departmentRepo = new DepartmentRepositoryAsync(this.unitOfWork);
            //需获得所有科室的所管指标
            var providingDepartmentCollection = departmentRepo.GetAll().Include(a => a.ProvidingIndicators);
            if (providingDepartmentId.HasValue && !providingDepartmentId.Value.Equals(Guid.Empty))
            {
                //如果指定了来源科室，则直接获得该科室的所管指标
                providingDepartmentCollection = providingDepartmentCollection.Where(a => a.DepartmentId == providingDepartmentId.Value);
            }

            foreach (var provideDepartment in await providingDepartmentCollection.ToListAsync())
            {
                //从数据来源科室中获得的IndicatorId集合
                var provideDepartmentIndicatorList = provideDepartment.ProvidingIndicators.Select(a => a.IndicatorId).ToList();
                var temp = await GetIndicatorDepartmentViewModelList(provideDepartmentIndicatorList);
                if(temp != null && temp.Count > 0)
                {
                    result.AddRange(temp);
                }
            }
            return result;
        }

        /// <summary>
        /// 构建IndicatorDepartmentViewModel集合
        /// </summary>
        /// <param name="dataSourceSystemIndicatorList"></param>
        /// <returns></returns>
        private async Task<List<IndicatorDepartmentViewModel>> GetIndicatorDepartmentViewModelList(List<Guid> dataSourceSystemIndicatorList)
        {
            List<IndicatorDepartmentViewModel> result = new List<IndicatorDepartmentViewModel>();
            if(dataSourceSystemIndicatorList == null || dataSourceSystemIndicatorList.Count == 0)
            {
                return result;
            }
            var indicatorRepo = new IndicatorRepositoryAsync(this.unitOfWork);

            //通过indicatorId从算法表中获取resultId集合，
            List<Guid> resultIndicatorList = await GetResultIndicatorList(dataSourceSystemIndicatorList);
            // IndicatorId集合与ResultId集合组合并且去重
            List<Guid> mergerIndicatorList = GetMergerIndicatorList(dataSourceSystemIndicatorList, resultIndicatorList);
            foreach (var indicatorId in mergerIndicatorList)
            {
                var departmentIdCollection = indicatorRepo.GetAll(a => a.IndicatorId == indicatorId).First().IndicatorGroupMapIndicators.Select(i => i.IndicatorGroup)
                    .SelectMany(i => i.DepartmentCategoryMapIndicatorGroups)
                    .Select(d => d.DepartmentCategory)
                    .SelectMany(d => d.Departments).Select(a => a.DepartmentId).ToList();
                foreach (var departmentId in departmentIdCollection)
                {
                    result.Add(new IndicatorDepartmentViewModel
                    {
                        IndicatorId = indicatorId,
                        DepartmentId = departmentId
                    });
                }
            }
            return result;
        }

        /// <summary>
        /// 合并由来源科室管理的IndicatorList及这些Indicator所归属的ResultIndicator
        /// </summary>
        /// <param name="provideDepartmentIndicatorList"></param>
        /// <param name="resultIndicatorList"></param>
        /// <returns></returns>
        private List<Guid> GetMergerIndicatorList(List<Guid> provideDepartmentIndicatorList, List<Guid> resultIndicatorList)
        {
            var result = new List<Guid>();
            if(provideDepartmentIndicatorList == null || provideDepartmentIndicatorList.Count == 0 || resultIndicatorList == null || resultIndicatorList.Count == 0)
            {
                return result;
            }
            result.Capacity = provideDepartmentIndicatorList.Count + resultIndicatorList.Count;
            result.AddRange(provideDepartmentIndicatorList);
            result.AddRange(resultIndicatorList);
            return result.Distinct().ToList();
        }
        /// <summary>
        /// 通过数据来源科室获得Indicator，从算法表中找到所有的ResultId，并且去重。
        /// </summary>
        /// <param name="provideDepartmentIndicatorList"></param>
        /// <returns></returns>
        private async Task<List<Guid>> GetResultIndicatorList(List<Guid> provideDepartmentIndicatorList)
        {
            var result = new List<Guid>();
            if(provideDepartmentIndicatorList == null || provideDepartmentIndicatorList.Count == 0)
            {
                return result;
            }
            foreach (var provideIndicatorId in provideDepartmentIndicatorList)
            {
                IndicatorAlgorithmRepositoryAsync algorithmRepo = new IndicatorAlgorithmRepositoryAsync(this.unitOfWork);
                var algorithmList = await algorithmRepo.GetAll().ToListAsync();
                List<Guid> temp = await GetResultIndicatorListFromIndicatorId(provideIndicatorId, algorithmList);
                if (temp != null && temp.Count > 0)
                {
                    result.AddRange(temp);
                }
            }
            return result.Distinct().ToList();
        }
        /// <summary>
        /// 递归从算法表中获得所有ResultId，需循环往上遍历
        /// </summary>
        /// <param name="provideIndicatorId">作为操作数据的indicatorId</param>
        /// <param name="algorithmList">算法表</param>
        /// <returns></returns>
        private async Task<List<Guid>> GetResultIndicatorListFromIndicatorId(Guid provideIndicatorId, List<IndicatorAlgorithm> algorithmList)
        {
            ///算法表中operand1或operand2等于该IndicatorId的所有ResultId，再将该resultId作为操作数继续向上找，直到找不到为止
            ///
            var result = new List<Guid>();
            var resultIdList = algorithmList.AsParallel().Where(a => a.FirstOperandID == provideIndicatorId || a.SecondOperandID == provideIndicatorId).Select(a => a.ResultId).ToList();
            if (resultIdList != null && resultIdList.Count > 0)
            {
                result.AddRange(resultIdList);
                foreach (var resultId in resultIdList)
                {
                    var temp = await GetResultIndicatorListFromIndicatorId(resultId, algorithmList);
                    if (temp != null && temp.Count > 0)
                    {
                        result.AddRange(temp);
                    }
                }
            }
            return result.Distinct().ToList();

        }
        #endregion
    }

    public enum DataSourceEnum
    {
        PROVIDEDEPARTMENT,
        DATASOURCESYSTEM,
        DEPARTMENT
    }
}