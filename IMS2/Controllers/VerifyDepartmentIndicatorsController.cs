using IMS2.Models;
using IMS2.ViewModels.VerifyDepartmentIndicatorView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Net;
using IMS2.RepositoryAsync;
using IMS2.BusinessModel.ObserverMode.Dad;
using Ninject;
namespace IMS2.Controllers
{
    public class VerifyDepartmentIndicatorsController : Controller
    {
        //private ImsDbContext db = new ImsDbContext();
        //private IDomainUnitOfWork unitOfWork;
        //private DepartmentIndicatorValueRepositoryAsync repo;
        private IKernel kernel;
        //public VerifyDepartmentIndicatorsController(IDomainUnitOfWork unitOfWork, IKernel kernel)
        //{
        //    this.unitOfWork = unitOfWork;
        //    this.repo = new DepartmentIndicatorValueRepositoryAsync(this.unitOfWork);
        //    this.kernel = kernel;
        //}
        public VerifyDepartmentIndicatorsController(IKernel kernel)
        {
            this.kernel = kernel;
        }
        // GET: VerifyDepartmentIndicators
        public ActionResult Index()
        {
            PoluateDepartmentSelect();
            return View();
        }


        [Authorize(Roles = "审核全院指标值, Administrators")]

        public async Task<ActionResult> _VerifyList(VerifySearchCondition searchCondition)
        {
            ViewBag.startTime = searchCondition.SearchStartTime;
            ViewBag.endTime = searchCondition.SearchEndTime;
            ViewBag.department = searchCondition.DepartmentId;
            if (TryUpdateModel(searchCondition))
            {
                return PartialView(await GetIndicatorValueList(searchCondition));
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.NonAuthoritativeInformation);
            }
        }
      
        #region 一键审核

        [Authorize(Roles = "审核全院指标值, Administrators")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> _VerifyAllList(DateTime startTime, DateTime endTime, Guid? departmentID)
        {
            var searchCondition = new VerifySearchCondition { SearchStartTime = startTime, SearchEndTime = endTime, DepartmentId = departmentID };
            var departmentIndicatorValues = await GetIndicatorValueList(searchCondition);

            var departmentIndicatorValueSubjectList = TransferDepartmentIndicatorValueToSubject(departmentIndicatorValues);


            foreach (var indicatorSubject in departmentIndicatorValueSubjectList)
            {
                indicatorSubject.SetLocked();
            }
            return Json(new { updateNumber = departmentIndicatorValues.Where(a => a.Value.HasValue).Count() });
        }
       
        #endregion

        #region 一键取消审核
        [Authorize(Roles = "审核全院指标值, Administrators")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> _CancelVerifyAllList(DateTime startTime, DateTime endTime, Guid? departmentID)
        {
            var searchCondition = new VerifySearchCondition { SearchStartTime = startTime, SearchEndTime = endTime, DepartmentId = departmentID };
            var departmentIndicatorValues = await GetIndicatorValueList(searchCondition);
            var departmentIndicatorValueSubjectList = TransferDepartmentIndicatorValueToSubject(departmentIndicatorValues);


            foreach (var indicatorSubject in departmentIndicatorValueSubjectList)
            {
                indicatorSubject.SetUnlocked();
            }
            return Json(new { updateNumber = departmentIndicatorValueSubjectList.Count() });
        }

        private List<DepartmentIndicatorValueSubject> TransferDepartmentIndicatorValueToSubject(List<DepartmentIndicatorValue> departmentIndicatorValues)
        {
            var results = new List<DepartmentIndicatorValueSubject>();

            object myLock = new object();
            Parallel.ForEach(departmentIndicatorValues,
            () => new List<DepartmentIndicatorValueSubject>(),
            (a, loopstate, localStorage) =>
            {
                DepartmentIndicatorValueSubject departmentIndicatorValueSubject = this.kernel.Get<DepartmentIndicatorValueSubject>();
                departmentIndicatorValueSubject.DepartmentIndicatorValueId = a.DepartmentIndicatorValueId;
                departmentIndicatorValueSubject.IndicatorId = a.IndicatorId;
                departmentIndicatorValueSubject.DepartmentId = a.DepartmentId;
                departmentIndicatorValueSubject.Time = a.Time;
                localStorage.Add(departmentIndicatorValueSubject);
                return localStorage;
            },
            (finalStorage) =>
            {
                lock (myLock)
                {
                    results.AddRange(finalStorage);
                };
            });

            return results;
        }

       


        #endregion

        #region 单个审核
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isLock"></param>
        /// <param name="id">DepartmentIndicatorValue</param>
        /// <returns></returns>
        [Authorize(Roles = "审核全院指标值, Administrators")]

        [HttpPost]

        public async Task<ActionResult> _VerifyLocked(bool isLock, Guid id)
        {
            var departmentIndicatorValueLocked = new DepartmentIndicatorValue() ;
            using(var context = new ImsDbContext())
            {
                departmentIndicatorValueLocked = await context.DepartmentIndicatorValues.FindAsync(id);
            }
            if (departmentIndicatorValueLocked != null && departmentIndicatorValueLocked.Value.HasValue)
            {

                try
                {

                    HandleLockDepartmentIndicatorValue(departmentIndicatorValueLocked.DepartmentIndicatorValueId, departmentIndicatorValueLocked.IndicatorId, departmentIndicatorValueLocked.DepartmentId, departmentIndicatorValueLocked.Time, isLock);


                    return Json(new { success = true });
                }
                catch (Exception)
                {
                    return Json(new { success = false });

                    //throw;
                }
            }
            else
            {
                return Json(new { success = false });
            }

        }
        //public async Task<ActionResult> _VerifyLocked(bool isLock, Guid id)
        //{
        //    var departmentIndicatorValueLocked = await this.repo.SingleAsync(id);
        //    if (departmentIndicatorValueLocked.Value.HasValue)
        //    {

        //        try
        //        {

        //            HandleLockDepartmentIndicatorValue(departmentIndicatorValueLocked.DepartmentIndicatorValueId, departmentIndicatorValueLocked.IndicatorId, departmentIndicatorValueLocked.DepartmentId, departmentIndicatorValueLocked.Time, isLock);


        //            return Json(new { success = true });
        //        }
        //        catch (Exception)
        //        {
        //            return Json(new { success = false });

        //            //throw;
        //        }
        //    }
        //    else
        //    {
        //        return Json(new { success = false });
        //    }

        //}

        #endregion



        #region Private Method
        private void HandleLockDepartmentIndicatorValue(Guid departmentIndicatorValueId, Guid indicatorId, Guid departmentId, DateTime time, bool isLock)
        {
            try
            {

                var departmentIndicatorValueSubject = this.kernel.Get<DepartmentIndicatorValueSubject>();

                departmentIndicatorValueSubject.DepartmentIndicatorValueId = departmentIndicatorValueId;
                departmentIndicatorValueSubject.IndicatorId = indicatorId;
                departmentIndicatorValueSubject.DepartmentId = departmentId;
                departmentIndicatorValueSubject.Time = time;
                if (isLock)
                {
                    departmentIndicatorValueSubject.SetLocked();

                }
                else
                {
                    departmentIndicatorValueSubject.SetUnlocked();

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void UnLockDepartmentIndicatorValue(Guid departmentIndicatorValueId, Guid indicatorId, Guid departmentId, DateTime time)
        {
            try
            {

                var departmentIndicatorValueSubject = this.kernel.Get<DepartmentIndicatorValueSubject>();

                departmentIndicatorValueSubject.DepartmentIndicatorValueId = departmentIndicatorValueId;
                departmentIndicatorValueSubject.IndicatorId = indicatorId;
                departmentIndicatorValueSubject.DepartmentId = departmentId;
                departmentIndicatorValueSubject.Time = time;

                departmentIndicatorValueSubject.SetUnlocked();

            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<List<DepartmentIndicatorValue>> GetIndicatorValueList(VerifySearchCondition searchCondition)
        {
            //var departmentIndicatorValues = db.DepartmentIndicatorValues.Include(d => d.Department).Include(d => d.DepartmentIndicatorStandard).Include(d => d.Indicator.Duration);
            using(var context = new ImsDbContext())
            {
                var departmentIndicatorValues = context.DepartmentIndicatorValues.Include(d => d.Department).Include(d => d.DepartmentIndicatorStandard).Include(d => d.Indicator.Duration);
                if (searchCondition.DepartmentId.HasValue)
                {
                    departmentIndicatorValues = departmentIndicatorValues.Where(d => d.DepartmentId == searchCondition.DepartmentId);
                }
                switch (searchCondition.LockStatus)
                {

                    case LockStatus.Locked:
                        departmentIndicatorValues = departmentIndicatorValues.Where(a => a.IsLocked == true);
                        break;
                    case LockStatus.UnLock:
                        departmentIndicatorValues = departmentIndicatorValues.Where(a => a.IsLocked == false);
                        break;
                    case LockStatus.All:
                        break;
                }
                departmentIndicatorValues = departmentIndicatorValues.Where(d => d.Time.Year >= searchCondition.SearchStartTime.Year && d.Time.Month >= searchCondition.SearchStartTime.Month && d.Time.Year <= searchCondition.SearchEndTime.Year && d.Time.Month <= searchCondition.SearchEndTime.Month);

                return await departmentIndicatorValues.OrderBy(d => d.Indicator.Priority).ToListAsync();
            }
           
        }

        #region unitOfWork GetIndicatorValueList
        //private async Task<List<DepartmentIndicatorValue>> GetIndicatorValueList(VerifySearchCondition searchCondition)
        //{
        //    //var departmentIndicatorValues = db.DepartmentIndicatorValues.Include(d => d.Department).Include(d => d.DepartmentIndicatorStandard).Include(d => d.Indicator.Duration);
        //    var departmentIndicatorValues = this.repo.GetAll().Include(d => d.Department).Include(d => d.DepartmentIndicatorStandard).Include(d => d.Indicator.Duration);
        //    if (searchCondition.DepartmentId.HasValue)
        //    {
        //        departmentIndicatorValues = departmentIndicatorValues.Where(d => d.DepartmentId == searchCondition.DepartmentId);
        //    }
        //    switch (searchCondition.LockStatus)
        //    {

        //        case LockStatus.Locked:
        //            departmentIndicatorValues = departmentIndicatorValues.Where(a => a.IsLocked == true);
        //            break;
        //        case LockStatus.UnLock:
        //            departmentIndicatorValues = departmentIndicatorValues.Where(a => a.IsLocked == false);
        //            break;
        //        case LockStatus.All:
        //            break;
        //    }
        //    departmentIndicatorValues = departmentIndicatorValues.Where(d => d.Time.Year >= searchCondition.SearchStartTime.Year && d.Time.Month >= searchCondition.SearchStartTime.Month && d.Time.Year <= searchCondition.SearchEndTime.Year && d.Time.Month <= searchCondition.SearchEndTime.Month);

        //    return await departmentIndicatorValues.OrderBy(d => d.Indicator.Priority).ToListAsync();
        //}
        #endregion

        #endregion
        #region 科室列表
        private void PoluateDepartmentSelect()
        {
            ViewBag.DepartmentSelect = GetDepartmentSingleSelectList();

        }


        #region unitOfWork 
        //public SelectList GetDepartmentSingleSelectList()
        //{
        //    var httpRuntimeCache = HttpRuntime.Cache;
        //    if (httpRuntimeCache != null && httpRuntimeCache["DepartmentSingleSelectID"] != null)
        //    {
        //        return httpRuntimeCache["DepartmentSingleSelectID"] as SelectList;
        //    }
        //    else
        //    {
        //        var departmentRepo = new DepartmentRepositoryAsync(this.unitOfWork);
        //        var result = new SelectList(departmentRepo.GetAll().Select(a => new SelectListItem { Value = a.DepartmentId.ToString(), Text = a.DepartmentName }).ToList(), "Value", "Text");
        //        httpRuntimeCache.Insert("DepartmentSingleSelectID", result);
        //        return result;
        //    }
        //}
        #endregion
        public SelectList GetDepartmentSingleSelectList()
        {
           using (var context = new ImsDbContext())
            {
                var departmentRepo = context.Departments.ToList();
                var result = new SelectList(departmentRepo.AsParallel().Select(a => new SelectListItem { Value = a.DepartmentId.ToString(), Text = a.DepartmentName }).ToList(), "Value", "Text");
                return result;
            }
              
          
        }
        #endregion

    }
}