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
    public class StatisticsDepartmentIndicatorValueController : Controller
    {
        private IDomainUnitOfWork unitOfWork;
        private ISatisticsValue satisticsValue;
        private IIndicatorDepartment indicatorDepartment;
        // GET: StatisticsDepartmentIndicatorValue
        private DepartmentIndicatorDurationValueRepositoryAsync repo;
        public StatisticsDepartmentIndicatorValueController(IDomainUnitOfWork unitOfWork, ISatisticsValue satisticsValue, IIndicatorDepartment indicatorDepartment)
        {
            this.unitOfWork = unitOfWork;
            this.repo = new DepartmentIndicatorDurationValueRepositoryAsync(this.unitOfWork);
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
                var searchDepartmentID = Expression.Convert(Expression.Constant(searchCondition.DepartmentID), exDepartmentID.Type);

                var equalDepartmentID = Expression.Equal(exDepartmentID, searchDepartmentID);
                finalSearch = Expression.AndAlso(finalSearch, equalDepartmentID);
            }
            #endregion

            #region 指标
            if (searchCondition.IndicatorID.HasValue)
            {
                var exIndicatorID = Expression.Property(satisticParam, "IndicatorID");
                var searchIndicatorID = Expression.Convert(Expression.Constant(searchCondition.IndicatorID), exIndicatorID.Type);

                var equalIndicatorID = Expression.Equal(exIndicatorID, searchIndicatorID);
                finalSearch = Expression.AndAlso(finalSearch, equalIndicatorID);
            }
            #endregion

            #region 跨度
            if (searchCondition.DurationID.HasValue)
            {
                var exDurationID = Expression.Property(satisticParam, "DurationID");
                var searchDurationID = Expression.Convert(Expression.Constant(searchCondition.DurationID), exDurationID.Type);

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

            return expressionDeviceSearch;
        }
        #endregion

        #region 创建
        /// <summary>
        /// 根据跨度计算出算法表中ResultID的指标值
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            ViewBag.DurationSelect = GetDurationSelect();
            return View();
        }

        private SelectList GetDurationSelect()
        {
            var durationRepo = new DurationRepositoryAsync(this.unitOfWork);
            var result = new SelectList(durationRepo.GetAll().ToList(), "DurationId", "DurationName");
            return result;
        }

        /// <summary>
        /// 根据跨度，指标，计算出该算法表中为Result的指标所对应的所有科室在该跨度下的对应值
        /// </summary>
        /// <param name="valueEdit"></param>
        /// <returns>成功返回新添加的各指标列表，否则返回错误信息提示</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> _CaculateAlgorithmResult(DepartmentIndicatorDurationVirtualValueEdit valueEdit)
        {
            if (!IsValidEditInput(valueEdit))
            {
                return new HttpNotFoundResult();
            }
            //找到该指标对应的所有的科室
            var indicatorDepartmentList = await this.indicatorDepartment.GetAlgorithmIndicatorDepartment();
            //转换成DepartmentIndicatorDurationTime格式
            var departmentIndicatorDurationTimeList = TransferToDepartmentIndicatorDurationTime(valueEdit, indicatorDepartmentList);

            //计算各科室的指标、跨度、时间的值            
            List<SatisticsIndicatorHandleMessage> handleMessage = new List<SatisticsIndicatorHandleMessage>();
            //从数据中根据规格取值Value
            foreach (var item in departmentIndicatorDurationTimeList)
            {
                try
                {
                    item.Value = await this.satisticsValue.GetSatisticsValue(item);
                }
                catch (Exception e)
                {
                    var error = new SatisticsIndicatorHandleMessage
                    {
                        IndicatorID = item.IndicatorID,
                        DepartmentID = item.DepartmentId,
                        DurationID = item.DurationId,
                        Time = item.Time,
                        ErrorMessage = e.Message
                    };
                    handleMessage.Add(error);
                }
            }
            //写入到数据库保存
            await SaveDepartmentIndicatorDurationVirtualValueToDatabase(departmentIndicatorDurationTimeList);

            //返回各指标列表
            ViewBag.HandleMessage = handleMessage;
            SatisticsSearchCondition searchCondition = new SatisticsSearchCondition
            {
                DurationID = valueEdit.DurationId,
                SearchTime = valueEdit.Time
            };
            //return await _List(searchCondition);
            return new JsonResult();
        }

        private bool IsValidEditInput(DepartmentIndicatorDurationVirtualValueEdit valueEdit)
        {
            //if (TryUpdateModel(valueEdit))
            //{
            //    return true;
            //}
            //else { return false; }
            return true;
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
                if(item.Value!= null)
                {
                    var temp = new DepartmentIndicatorDurationVirtualValue
                    {
                        DepartmentId = item.DepartmentId,
                        IndicatorId = item.IndicatorID,
                        DurationId = item.DurationId,
                        Time = item.Time,
                        Value = item.Value,
                        CreateTime = DateTime.Now,
                        UpdateTime = System.DateTime.Now
                    };
                    this.repo.Add(temp);
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

        private List<DepartmentIndicatorDurationTime> TransferToDepartmentIndicatorDurationTime(DepartmentIndicatorDurationVirtualValueEdit valueEdit, List<IndicatorDepartment> indicatorDepartmentList)
        {
            var result = new List<DepartmentIndicatorDurationTime>();
            Parallel.ForEach(indicatorDepartmentList, indicatorDepartment =>
            {
                Parallel.ForEach(indicatorDepartment.DepartmentIDList, departmentID =>
                {
                    var temp = new DepartmentIndicatorDurationTime
                    {
                        IndicatorID = indicatorDepartment.IndicatorID,
                        DepartmentId = departmentID,
                        DurationId = valueEdit.DurationId,
                        Time = valueEdit.Time
                    };
                    result.Add(temp);
                });
            });
            return result;
        }
        #endregion



        /// <summary>
        /// 计算出科室指标时段的值
        /// </summary>
        /// <param name="departmentIndicatorDurationTime"></param>
        /// <returns></returns>
        public async Task<ActionResult> CaculateDepartmentIndicatorDurationValue(DepartmentIndicatorDurationTime departmentIndicatorDurationTime)
        {
            //验证输入数据的有效性
            if (IsValidInput(departmentIndicatorDurationTime))
            {
                //获得值
                departmentIndicatorDurationTime.Value = await GetDepartmentIndicatorTimeValue(departmentIndicatorDurationTime);
                //存入数据库，存入到新值表中
                await AddToNewDepartmentIndicatorValueDataTable(departmentIndicatorDurationTime);
            }
            else
            {
                return View(departmentIndicatorDurationTime);
            }

            //返回视图
            return View(departmentIndicatorDurationTime);
        }

        private async Task<decimal?> GetDepartmentIndicatorTimeValue(DepartmentIndicatorDurationTime departmentIndicatorDurationTime)
        {
            return await this.satisticsValue.GetSatisticsValue(departmentIndicatorDurationTime);
        }

        /// <summary>
        /// 存入到新值表中
        /// </summary>
        /// <param name="departmentIndicatorDurationTime"></param>
        private async Task AddToNewDepartmentIndicatorValueDataTable(DepartmentIndicatorDurationTime departmentIndicatorDurationTime)
        {
            //判断是否在数据库中已经存在相关项
            var item = await FindDepartmentIndicatorDurationValue(departmentIndicatorDurationTime);
            if (item != null)
            {
                //如果存在，直接更新Value和UpdateTime

                await UpdateDepartmentIndicatorDurationValue(item, departmentIndicatorDurationTime.Value);
            }
            else
            {
                //如果不存在，则新建
                await CreateNewDepartmentIndicatorDurationTime(departmentIndicatorDurationTime);
            }

        }

        /// <summary>
        /// 更新表中的值
        /// </summary>
        /// <param name="item"></param>
        /// <param name="value"></param>
        private async Task UpdateDepartmentIndicatorDurationValue(DepartmentIndicatorDurationVirtualValue item, decimal? value)
        {
            item.Value = value;
            item.UpdateTime = System.DateTime.Now;
            this.repo.Update(item);
            try
            {
                await this.unitOfWork.SaveChangesClientWinAsync();

            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 从数据库中查找符合条件的departmentIndicatorDurationTime
        /// </summary>
        /// <param name="departmentIndicatorDurationTime"></param>
        /// <returns>找到则返回值，否则返回null</returns>
        private async Task<DepartmentIndicatorDurationVirtualValue> FindDepartmentIndicatorDurationValue(DepartmentIndicatorDurationTime departmentIndicatorDurationTime)
        {
            var query = await this.repo.GetAll(a => a.IndicatorId == departmentIndicatorDurationTime.IndicatorID && a.DurationId == departmentIndicatorDurationTime.DurationId && a.DepartmentId == departmentIndicatorDurationTime.DepartmentId && a.Time == departmentIndicatorDurationTime.Time).FirstOrDefaultAsync();
            return query;
        }

        private async Task CreateNewDepartmentIndicatorDurationTime(DepartmentIndicatorDurationTime departmentIndicatorDurationTime)
        {

            var newDepartmentIndicatorDurationValue = new DepartmentIndicatorDurationVirtualValue
            {
                DepartmentIndicatorDurationVirtualValueID = System.Guid.NewGuid(),
                IndicatorId = departmentIndicatorDurationTime.IndicatorID,
                DepartmentId = departmentIndicatorDurationTime.DepartmentId,
                CreateTime = System.DateTime.Now,
                UpdateTime = System.DateTime.Now,
                DurationId = departmentIndicatorDurationTime.DurationId,
                Time = departmentIndicatorDurationTime.Time,
                Value = departmentIndicatorDurationTime.Value
            };
            repo.Add(newDepartmentIndicatorDurationValue);
            try
            {
                await this.unitOfWork.SaveChangesClientWinAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 验证用户的输入值 的有效性
        /// </summary>
        /// <param name="departmentIndicatorDurationTime"></param>
        /// <returns></returns>
        private bool IsValidInput(DepartmentIndicatorDurationTime departmentIndicatorDurationTime)
        {
            if (TryUpdateModel(departmentIndicatorDurationTime))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}