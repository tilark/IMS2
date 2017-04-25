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
namespace IMS2.Controllers
{
    public class VerifyDepartmentIndicatorsController : Controller
    {
        private ImsDbContext db = new ImsDbContext();
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
                //var departmentIndicatorValues = db.DepartmentIndicatorValues.Include(d => d.Department).Include(d => d.DepartmentIndicatorStandard).Include(d => d.Indicator.Duration);
                //if (searchCondition.DepartmentId.HasValue)
                //{
                //    departmentIndicatorValues = departmentIndicatorValues.Where(d => d.DepartmentId == searchCondition.DepartmentId);
                //}
                //departmentIndicatorValues = departmentIndicatorValues.Where(d => d.Time.Year >= searchCondition.SearchStartTime.Year && d.Time.Month >= searchCondition.SearchStartTime.Month&& d.Time.Year <= searchCondition.SearchEndTime.Year && d.Time.Month <= searchCondition.SearchEndTime.Month);

                //return PartialView(await departmentIndicatorValues.OrderBy(d => d.Indicator.Priority).ToListAsync());
                return PartialView(await  GetIndicatorValueList(searchCondition));
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.NonAuthoritativeInformation);
            }
        }
        private async Task<List<DepartmentIndicatorValue>> GetIndicatorValueList(VerifySearchCondition searchCondition)
        {
            var departmentIndicatorValues = db.DepartmentIndicatorValues.Include(d => d.Department).Include(d => d.DepartmentIndicatorStandard).Include(d => d.Indicator.Duration);
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
        #region 一键审核
        [Authorize(Roles = "审核全院指标值, Administrators")]

        public async Task<ActionResult> _VerifyAllList(DateTime startTime, DateTime endTime, Guid? departmentID)
        {
            var searchCondition = new VerifySearchCondition { SearchStartTime = startTime, SearchEndTime = endTime, DepartmentId = departmentID };
            var departmentIndicatorValues = await GetIndicatorValueList(searchCondition);
            foreach(var indicatorValue in departmentIndicatorValues)
            {
                if (indicatorValue.Value.HasValue)
                {
                    indicatorValue.IsLocked = true;
                    indicatorValue.UpdateTime = System.DateTime.Now;
                    //database win
                    bool saveFailed;
                    do
                    {
                        saveFailed = false;
                        try
                        {
                            await db.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {
                            saveFailed = true;
                            // Update the values of the entity that failed to save from the store 
                            ex.Entries.Single().Reload();
                        }
                    } while (saveFailed);
                }
            }
            return Json(new { updateNumber = departmentIndicatorValues.Where(a => a.Value.HasValue).Count() });
        }
        #endregion

        #region 一键取消审核
        [Authorize(Roles = "审核全院指标值, Administrators")]

        public async Task<ActionResult> _CancelVerifyAllList(DateTime startTime, DateTime endTime, Guid? departmentID)
        {
            var searchCondition = new VerifySearchCondition { SearchStartTime = startTime, SearchEndTime = endTime, DepartmentId = departmentID };
            var departmentIndicatorValues = await GetIndicatorValueList(searchCondition);
            foreach (var indicatorValue in departmentIndicatorValues)
            {
                if (indicatorValue.Value.HasValue)
                {
                    indicatorValue.IsLocked = false;
                    indicatorValue.UpdateTime = System.DateTime.Now;
                    //database win
                    bool saveFailed;
                    do
                    {
                        saveFailed = false;
                        try
                        {
                            await db.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {
                            saveFailed = true;
                            // Update the values of the entity that failed to save from the store 
                            ex.Entries.Single().Reload();
                        }
                    } while (saveFailed);
                }
            }
            return Json(new { updateNumber = departmentIndicatorValues.Where(a => a.Value.HasValue).Count() });
        }
        #endregion
        [Authorize(Roles = "审核全院指标值, Administrators")]

        [HttpPost]
        public async Task<ActionResult> _VerifyLocked(bool isLock, Guid id)
        {
            var departmentIndicatorValueLocked = await db.DepartmentIndicatorValues.FindAsync(id);
            if(departmentIndicatorValueLocked.Value.HasValue && departmentIndicatorValueLocked.IsLocked != isLock)
            {
                departmentIndicatorValueLocked.IsLocked = isLock;
                departmentIndicatorValueLocked.UpdateTime = DateTime.Now;
                //database win
                bool saveFailed;
                do
                {
                    saveFailed = false;
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        saveFailed = true;
                        // Update the values of the entity that failed to save from the store 
                        ex.Entries.Single().Reload();
                    }
                } while (saveFailed);

                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }

        }
        [Authorize(Roles = "审核全院指标值, Administrators")]
        //, DateTime? startTime, DateTime? endTime, Guid? department
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> _VerifyList(IEnumerable<DepartmentIndicatorValue> departmentIndicatorValue)
        {

            foreach (var indicatorValue in departmentIndicatorValue)
            {
                var departmentIndicatorValueLocked = await db.DepartmentIndicatorValues.FindAsync(indicatorValue.DepartmentIndicatorValueId);
                //无值情况不应该被审核，如果islocked与之前的相同，则不修改数据中的值
                if (indicatorValue.Value.HasValue && departmentIndicatorValueLocked.IsLocked != indicatorValue.IsLocked)
                {
                    departmentIndicatorValueLocked.IsLocked = indicatorValue.IsLocked;
                    departmentIndicatorValueLocked.UpdateTime = DateTime.Now;
                    //database win
                    bool saveFailed;
                    do
                    {
                        saveFailed = false;
                        try
                        {
                            await db.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {
                            saveFailed = true;
                            // Update the values of the entity that failed to save from the store 
                            ex.Entries.Single().Reload();
                        }
                    } while (saveFailed);
                }
            }
            //var searchCondition = new VerifySearchCondition { SearchStartTime = startTime.Value, SearchEndTime = endTime.Value, DepartmentId = department };
            //return RedirectToAction("_VerifyList", new { searchCondition = searchCondition });
            return Json( new { success = true});
        }
        #region 科室列表
        private void PoluateDepartmentSelect()
        {
            ViewBag.DepartmentSelect = GetDepartmentSingleSelectList();

        }
        public SelectList GetDepartmentSingleSelectList()
        {
            var httpRuntimeCache = HttpRuntime.Cache;
            if (httpRuntimeCache != null && httpRuntimeCache["DepartmentSingleSelectID"] != null)
            {
                return httpRuntimeCache["DepartmentSingleSelectID"] as SelectList;
            }
            else
            {
                var result = new SelectList(this.db.Departments.Select(a => new SelectListItem { Value = a.DepartmentId.ToString(), Text = a.DepartmentName}).ToList(), "Value", "Text");
                httpRuntimeCache.Insert("DepartmentSingleSelectID", result);
                return result;
            }
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}