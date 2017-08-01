using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Threading.Tasks;
using IMS2.BusinessModel.SatisticsValueModel;
using IMS2.Models;
using IMS2.ViewModels;
using IMS2.RepositoryAsync;
using IMS2.ViewModels.StatisticsDepartmentIndicatorValueViews;
using IMS2.BusinessModel.IndicatorDepartmentModel;
namespace IMS2.Controllers
{
    /// <summary>
    /// 统计科室指标值
    /// </summary>
    /// 
    [Authorize(Roles = "修改全院指标值, Administrators")]

    public class StatisticsDepartmentIndicatorValueController : Controller
    {
        private IDomainUnitOfWork unitOfWork;
        private ISatisticsValue satisticsValue;
        private IIndicatorDepartment indicatorDepartment;
        // GET: StatisticsDepartmentIndicatorValue
        private DepartmentIndicatorDurationVirtualValueRepositoryAsync repo;
        public StatisticsDepartmentIndicatorValueController(IDomainUnitOfWork unitOfWork, ISatisticsValue satisticsValue, IIndicatorDepartment indicatorDepartment)
        {
            this.unitOfWork = unitOfWork;
            this.repo = new DepartmentIndicatorDurationVirtualValueRepositoryAsync(this.unitOfWork);
            this.satisticsValue = satisticsValue;
            this.indicatorDepartment = indicatorDepartment;
        }

        #region 列表
        /// <summary>
        /// 列出统计值表中的项目
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            PoPulateDeaprtmentList();
            GetDurationSelect();
            return View();
        }

        /// <summary>
        /// 根据查询条件返回科室指标值列表
        /// </summary>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        public async Task<ActionResult> _List(SatisticsSearchCondition searchCondition)
        {

            List<DepartmentIndicatorDurationVirtualValueView> viewModel = await GetDepartmentIndicatorDurationVirtualValueViewModel(searchCondition);
            return PartialView("_List", viewModel);
        }

        private async Task<List<DepartmentIndicatorDurationVirtualValueView>> GetDepartmentIndicatorDurationVirtualValueViewModel(SatisticsSearchCondition searchCondition)
        {
            //生成Lamber表达式
            var expressionSearch = CreateSearchSatisticsExpressionTree<DepartmentIndicatorDurationVirtualValue>(searchCondition);
            //取数
            var viewModel = await this.repo.GetAll(expressionSearch).AsNoTracking().Select(a => new DepartmentIndicatorDurationVirtualValueView { IndicatorName = a.Indicator.IndicatorName, DurationName = a.Duration.DurationName, DepartmentName = a.Department.DepartmentName, CreateTime = a.CreateTime, UpdateTime = a.UpdateTime, Time = a.Time, Value = a.Value }).ToListAsync();
            return viewModel;
        }

        private Expression<Func<T, bool>> CreateSearchSatisticsExpressionTree<T>(SatisticsSearchCondition searchCondition)
        {
            Expression<Func<T, bool>> expressionDeviceSearch = null;

            ParameterExpression satisticParam = Expression.Parameter(typeof(T), "satistic");
            Expression finalSearch = Expression.Constant(true);

            #region 科室
            if (searchCondition.DepartmentID.HasValue)
            {
                var exDepartmentID = Expression.Property(satisticParam, "DepartmentID");
                var searchDepartmentID = Expression.Convert(Expression.Constant(searchCondition.DepartmentID.Value), exDepartmentID.Type);

                var equalDepartmentID = Expression.Equal(exDepartmentID, searchDepartmentID);
                finalSearch = Expression.AndAlso(finalSearch, equalDepartmentID);
            }
            #endregion

            #region 指标
            if (searchCondition.IndicatorID.HasValue)
            {
                var exIndicatorID = Expression.Property(satisticParam, "IndicatorID");
                var searchIndicatorID = Expression.Convert(Expression.Constant(searchCondition.IndicatorID.Value), exIndicatorID.Type);

                var equalIndicatorID = Expression.Equal(exIndicatorID, searchIndicatorID);
                finalSearch = Expression.AndAlso(finalSearch, equalIndicatorID);
            }
            #endregion

            #region 跨度
            if (searchCondition.DurationID.HasValue)
            {
                var exDurationID = Expression.Property(satisticParam, "DurationID");
                var searchDurationID = Expression.Convert(Expression.Constant(searchCondition.DurationID.Value), exDurationID.Type);

                var equalDurationID = Expression.Equal(exDurationID, searchDurationID);
                finalSearch = Expression.AndAlso(finalSearch, equalDurationID);
            }
            #endregion

            #region 时间
            var time = new DateTime(searchCondition.SearchTime.Year, searchCondition.SearchTime.Month, 1);
            var exsearchTime = Expression.Property(satisticParam, "Time");
            var searchTime = Expression.Convert(Expression.Constant(time), exsearchTime.Type);
            var equalTime = Expression.Equal(exsearchTime, searchTime);
            finalSearch = Expression.AndAlso(finalSearch, equalTime);
            #endregion

            if (finalSearch != null)
            {
                expressionDeviceSearch = Expression.Lambda<Func<T, bool>>(finalSearch, satisticParam);
            }
            return expressionDeviceSearch;
        }
        #endregion

      
        #region 创建新值表
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        public ActionResult CreateVirtualValue()
        {
            //创建跨度表和来源科室的下拉列表
            GetDurationSelect();
            PoPulateProvideDepartmentList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateVirtualValue(DepartmentIndicatorDurationVirtualValueCreate departmentIndicatorDurationVirtualValueCreate)
        {
            if (ModelState.IsValid)
            {
                //获得来源科室对应的指标，再获得该指标对应的科室，构成指标、科室集合
                var indicatorDepartmentList = await GetIndictorDepartmentList(departmentIndicatorDurationVirtualValueCreate.ProvidingDepartmentId);

                //获得指定的Duration集合
                var durationList = await GetDurationSelect(departmentIndicatorDurationVirtualValueCreate.DurationId);

                //将指标科室集合、Duration集合和时间组建成科室、指标、跨度、时间的集合
                var indicatorDepartmentDurationTimeList = TransferToIndicatorDepartmentDurationTimeList(indicatorDepartmentList, durationList, departmentIndicatorDurationVirtualValueCreate.Time);

                //从Satistics中获得值
                if(indicatorDepartmentDurationTimeList != null && indicatorDepartmentDurationTimeList.Count > 0)
                {
                   
                    foreach(var item in indicatorDepartmentDurationTimeList)
                    {
                        try
                        {
                            item.Value = await this.satisticsValue.GetSatisticsValue(item.IndicatorID, item.DurationId, item.DepartmentId, item.Time);
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
                    //写入数据库的新值表中保存
                    await SaveDepartmentIndicatorDurationVirtualValueToDatabase(indicatorDepartmentDurationTimeList);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "创建指标科室跨度时间集合失败！");
                }
            }
            GetDurationSelect();
            PoPulateProvideDepartmentList();
            return View(departmentIndicatorDurationVirtualValueCreate);
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
        /// 根据来源科室获得所管的指标及该指标对应的所有科室，组建成指标-科室集合。
        /// </summary>
        /// <param name="providingDepartmentId"></param>
        /// <returns>List<IndicatorDepartmentViewModel></returns>
        private async Task<List<IndicatorDepartmentViewModel>> GetIndictorDepartmentList(Guid? providingDepartmentId)
        {
            List<IndicatorDepartmentViewModel> result = new List<IndicatorDepartmentViewModel>();
            //如果providingDepartmentId的值是Guid.Empty，则直接返回Null
            if (providingDepartmentId.HasValue && providingDepartmentId.Value.Equals(Guid.Empty))
            {
                return null;
            }
            var departmentRepo = new DepartmentRepositoryAsync(this.unitOfWork);
            var indicatorRepo = new IndicatorRepositoryAsync(this.unitOfWork);
            //需获得所有科室的所管指标
            var providingDepartmentCollection = departmentRepo.GetAll().Include(a => a.ProvidingIndicators);
            if (providingDepartmentId.HasValue)
            {
                //如果指定了来源科室，则直接获得该科室的所管指标
                providingDepartmentCollection = providingDepartmentCollection.Where(a => a.DepartmentId == providingDepartmentId.Value);
            }
            
            foreach(var provideDepartment in await providingDepartmentCollection.ToListAsync())
            {
               foreach(var indicatorId in provideDepartment.ProvidingIndicators.Select(a => a.IndicatorId).ToList())
                {
                    var departmentIdCollection =  indicatorRepo.GetAll(a => a.IndicatorId == indicatorId).First().IndicatorGroupMapIndicators.Select(i => i.IndicatorGroup)
                        .SelectMany(i => i.DepartmentCategoryMapIndicatorGroups)
                        .Select(d => d.DepartmentCategory)
                        .SelectMany(d => d.Departments).Select(a => a.DepartmentId).ToList();
                    foreach(var departmentId in departmentIdCollection)
                    {
                        result.Add(new IndicatorDepartmentViewModel
                        {
                            IndicatorId = indicatorId,
                            DepartmentId = departmentId
                        });
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 将计算出来的DepartmentIndicatorDurationTime的值写入到数据库
        /// </summary>
        /// <param name="departmentDurationValue"></param>
        /// <returns></returns>
        private async Task SaveDepartmentIndicatorDurationVirtualValueToDatabase(List<DepartmentIndicatorDurationTime> departmentDurationValue)
        {
            foreach (var item in departmentDurationValue)
            {
                if (item.Value != null)
                {

                    ///先判断在新值表中
                    var query = await repo.GetAll(a => a.IndicatorId == item.IndicatorID && a.DepartmentId == item.DepartmentId && a.DurationId == item.DurationId && a.Time == item.Time).FirstOrDefaultAsync();
                    if(query != null)
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
                        this.repo.Add(temp);                        
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
        }
        #endregion      
      
        #region 下拉列表
        private void GetDurationSelect()
        {
            var durationRepo = new DurationRepositoryAsync(this.unitOfWork);
            ViewBag.DurationSelect = new SelectList(durationRepo.GetAll().ToList(), "DurationId", "DurationName");
            
        }

        /// <summary>
        /// 来源科室的列表
        /// </summary>
        private void PoPulateProvideDepartmentList()
        {
            var indicatorRepo = new IndicatorRepositoryAsync(this.unitOfWork);
            ViewBag.ProvideDepartmentSelect = new SelectList(indicatorRepo.GetAll().Select(i => i.ProvidingDepartment).Distinct().OrderBy(d => d.Priority).Select(a => new SelectListItem { Text = a.DepartmentName, Value = a.DepartmentId.ToString() }), "Value", "Text");
        }

        private void PoPulateDeaprtmentList()
        {
            var departmentRepo = new DepartmentRepositoryAsync(this.unitOfWork);
            ViewBag.DepartmentSelect = new SelectList(departmentRepo.GetAll().OrderBy(a => a.Priority).Select(a => new SelectListItem { Text = a.DepartmentName, Value = a.DepartmentId.ToString() }), "Value", "Text");
        }
        #endregion
    }
}