using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IMS2.BusinessModel.SatisticsValueModel;
using IMS2.Models;
using IMS2.ViewModels;
using IMS2.RepositoryAsync;
using System.Data.Entity;

using System.Threading.Tasks;

namespace IMS2.Controllers
{
    public class StatisticsDepartmentIndicatorValueController : Controller
    {
        private IDomainUnitOfWork unitOfWork;
        private ISatisticsValue satisticsValue;
        // GET: StatisticsDepartmentIndicatorValue
        private DepartmentIndicatorDurationValueRepositoryAsync repo;
        public StatisticsDepartmentIndicatorValueController(IDomainUnitOfWork unitOfWork, ISatisticsValue satisticsValue)
        {
            this.unitOfWork = unitOfWork;
            this.repo = new DepartmentIndicatorDurationValueRepositoryAsync(this.unitOfWork);
            this.satisticsValue = satisticsValue;
        }



        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 创建新值表
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Create(DepartmentIndicatorDurationTime departmentIndicatorDurationTime)
        {
            await AddToNewDepartmentIndicatorValueDataTable(departmentIndicatorDurationTime);
            return View();
        }

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
        private async Task<DepartmentIndicatorDurationVirtualValue >FindDepartmentIndicatorDurationValue(DepartmentIndicatorDurationTime departmentIndicatorDurationTime)
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