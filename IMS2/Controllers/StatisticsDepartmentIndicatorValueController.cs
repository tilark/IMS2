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
            //var expressionSearch = CreateSearchSatisticsExpressionTree<DepartmentIndicatorDurationVirtualValue>();
            var expressionSearch = searchCondition.CreateSearchSatisticsExpressionTree<DepartmentIndicatorDurationVirtualValue>();
            //取数
            var viewModel = await this.repo.GetAll(expressionSearch).AsNoTracking().Select(a => new DepartmentIndicatorDurationVirtualValueView { IndicatorName = a.Indicator.IndicatorName, DurationName = a.Duration.DurationName, DepartmentName = a.Department.DepartmentName, CreateTime = a.CreateTime, UpdateTime = a.UpdateTime, Time = a.Time, Value = a.Value }).ToListAsync();
            return viewModel;
        }
        #endregion

        #region 通过数据来源系统更新新值表
        /// <summary>
        /// 通过数据来源系统更新新值表
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateVirtualValueDataSourceSystem()
        {
            GetDurationSelect();
            GetDataSourceSystem();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateVirtualValueDataSourceSystem(DepartmentIndicatorDurationVirtualValueCreate departmentIndicatorDurationVirtualValueCreate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await departmentIndicatorDurationVirtualValueCreate.UpdateDepartmentIndicatorDurationVirtualTable(unitOfWork, satisticsValue, DataSourceEnum.DATASOURCESYSTEM);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    throw;
                }
            }
            GetDurationSelect();
            GetDataSourceSystem();
            return View(departmentIndicatorDurationVirtualValueCreate);
        }
        #endregion

        #region 通过科室更新新值表
        public ActionResult CreateVirtualValueDepartment()
        {
            GetDurationSelect();
            PoPulateDeaprtmentList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateVirtualValueDepartment(DepartmentIndicatorDurationVirtualValueCreate departmentIndicatorDurationVirtualValueCreate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await departmentIndicatorDurationVirtualValueCreate.UpdateDepartmentIndicatorDurationVirtualTable(unitOfWork, satisticsValue, DataSourceEnum.DEPARTMENT);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    throw;
                }
            }
            GetDurationSelect();
            PoPulateDeaprtmentList();
            return View(departmentIndicatorDurationVirtualValueCreate);
        }
        #endregion

        #region 通过数据来源科室更新新值表
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        public ActionResult CreateVirtualValueProvidingDepartment()
        {
            //创建跨度表和来源科室的下拉列表
            GetDurationSelect();
            PoPulateProvideDepartmentList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateVirtualValueProvidingDepartment(DepartmentIndicatorDurationVirtualValueCreate departmentIndicatorDurationVirtualValueCreate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await departmentIndicatorDurationVirtualValueCreate.UpdateDepartmentIndicatorDurationVirtualTable(unitOfWork, satisticsValue, DataSourceEnum.PROVIDEDEPARTMENT);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    throw;
                }
                ////获得来源科室对应的指标，再获得该指标对应的科室，构成指标、科室集合
                //var indicatorDepartmentList = await GetIndictorDepartmentListFromProvideDepartment(departmentIndicatorDurationVirtualValueCreate.DataSourceId);

                ////获得指定的Duration集合
                //var durationList = await GetDurationSelect(departmentIndicatorDurationVirtualValueCreate.DurationId);

                ////将指标科室集合、Duration集合和时间组建成科室、指标、跨度、时间的集合
                //var indicatorDepartmentDurationTimeList = TransferToIndicatorDepartmentDurationTimeList(indicatorDepartmentList, durationList, departmentIndicatorDurationVirtualValueCreate.Time);

                ////从Satistics中获得值
                //if (indicatorDepartmentDurationTimeList != null && indicatorDepartmentDurationTimeList.Count > 0)
                //{

                //    foreach (var item in indicatorDepartmentDurationTimeList)
                //    {
                //        try
                //        {
                //            item.Value = await this.satisticsValue.GetSatisticsValue(item.IndicatorID, item.DurationId, item.DepartmentId, item.Time);
                //        }
                //        catch (NullReferenceException)
                //        {
                //            continue;
                //        }
                //        catch (Exception)
                //        {

                //            item.Value = null;
                //        }

                //    }
                //    //同时需计算出该指标所对应的结果指标的值，再写入到数据库
                //    //写入数据库的新值表中保存
                //    await SaveDepartmentIndicatorDurationVirtualValueToDatabase(indicatorDepartmentDurationTimeList);

                //    return RedirectToAction(nameof(Index));
                //}
                //else
                //{
                //    ModelState.AddModelError(String.Empty, "创建指标科室跨度时间集合失败！");
                //}
            }
            GetDurationSelect();
            PoPulateProvideDepartmentList();
            return View(departmentIndicatorDurationVirtualValueCreate);
        }

       
        #endregion

        #region 下拉列表
        private void GetDataSourceSystem()
        {
            var repo = new DataSourceSystemRepositoryAsync(this.unitOfWork);
            ViewBag.DataSourceSystemSelect = new SelectList(repo.GetAll().OrderBy(a => a.Priority).Select(a => new SelectListItem { Text = a.DataSourceSystemName, Value = a.DataSourceSystemId.ToString() }), "Value", "Text");
        }
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