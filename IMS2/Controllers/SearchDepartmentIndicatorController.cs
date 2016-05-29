using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IMS2.Models;
using System.Data.Entity.Infrastructure;
using IMS2.ViewModels;
using PagedList;
namespace IMS2.Controllers
{
    [Authorize(Roles = "修改全院指标值, 审核全院指标值, Administrators")]

    public class SearchDepartmentIndicatorController : Controller
    {
        private ImsDbContext db = new ImsDbContext();

        // GET: SearchDepartmentIndicator
        [Route("{startTime}/{endTime}/{department}/{page}")]
        public async Task<ActionResult> Index(DateTime? startTime, DateTime? endTime, Guid? department, int? page)
        {
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            ViewBag.departmentID = department;
            ViewBag.department = new SelectList(db.Departments, "DepartmentId", "DepartmentName");
            var departmentIndicatorValues = db.DepartmentIndicatorValues.Include(d => d.Department).Include(d => d.DepartmentIndicatorStandard).Include(d => d.Indicator.Duration)
                                    .Where(d => d.DepartmentId == department.Value);
            if (startTime != null && endTime != null)
            {
               departmentIndicatorValues = departmentIndicatorValues.Where(d=>d.Time.Year >= startTime.Value.Year && d.Time.Month >= startTime.Value.Month
                                    && d.Time.Year <= endTime.Value.Year && d.Time.Month <= endTime.Value.Month);
            }
            int pageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["pagSize"]);
            int pageNumber = (page ?? 1);
            return View(await departmentIndicatorValues.OrderBy(d => d.Indicator.Priority).ToPagedListAsync(pageNumber, pageSize));
        }
        [Authorize(Roles = "审核全院指标值, Administrators")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(List<DepartmentIndicatorValue> departmentIndicatorValue, int? page, DateTime? startTime, DateTime? endTime, Guid? department)
        {
           
            foreach(var indicatorValue in departmentIndicatorValue)
            {
                var departmentIndicatorValueLocked = await db.DepartmentIndicatorValues.FindAsync(indicatorValue.DepartmentIndicatorValueId);
                //如果islocked与之前的相同，则不修改数据中的值
                if (departmentIndicatorValueLocked.IsLocked != indicatorValue.IsLocked)
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
            return RedirectToAction("Index", new { startTime = startTime, endTime = endTime, department = department, page = page });
        }
        private bool IsInRangeTime(DateTime starTime, DateTime midTime, DateTime endTime)
        {
            //midTime如果位于StartTime 与endTime之间，则返回True
            if (midTime.Year >= starTime.Year && midTime.Year <= endTime.Year
                && midTime.Month >= starTime.Month && midTime.Month <= endTime.Month)
            {
                return true;
            }
            else
            {
                return false;
            }


        }
        // GET: SearchDepartmentIndicator/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DepartmentIndicatorValue departmentIndicatorValue = await db.DepartmentIndicatorValues.FindAsync(id);
            if (departmentIndicatorValue == null)
            {
                return HttpNotFound();
            }
            return View(departmentIndicatorValue);
        }


        // GET: SearchDepartmentIndicator/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DepartmentIndicatorValue departmentIndicatorValue = await db.DepartmentIndicatorValues.FindAsync(id);
            if (departmentIndicatorValue == null)
            {
                return HttpNotFound();
            }
            ViewBag.IndicatorStandardId = new SelectList(db.DepartmentIndicatorStandards.Where(d=>d.DepartmentId == departmentIndicatorValue.DepartmentId && d.IndicatorId == departmentIndicatorValue.IndicatorId),
                                                "DepartmentIndicatorStandardId", "Range", departmentIndicatorValue.IndicatorStandardId);
            return View(departmentIndicatorValue);
        }

        // POST: SearchDepartmentIndicator/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "DepartmentIndicatorValueId,DepartmentId,IndicatorId,Time,Value,IndicatorStandardId,IsLocked,UpdateTime,TimeStamp")] DepartmentIndicatorValue departmentIndicatorValue)
        {
            ViewBag.IndicatorStandardId = new SelectList(db.DepartmentIndicatorStandards.Where(d => d.DepartmentId == departmentIndicatorValue.DepartmentId && d.IndicatorId == departmentIndicatorValue.IndicatorId), "DepartmentIndicatorStandardId", "Range", departmentIndicatorValue.IndicatorStandardId);

            if (ModelState.IsValid)
            {
                DepartmentIndicatorValue departmentIndicatorValueModify = await db.DepartmentIndicatorValues.FindAsync(departmentIndicatorValue.DepartmentIndicatorValueId);
                if(departmentIndicatorValueModify == null)
                {
                    return HttpNotFound();
                }

                if(departmentIndicatorValueModify.Value != departmentIndicatorValue.Value
                    || departmentIndicatorValueModify.IsLocked != departmentIndicatorValue.IsLocked
                    || departmentIndicatorValueModify.IndicatorStandardId != departmentIndicatorValue.IndicatorStandardId)
                {
                    departmentIndicatorValueModify.Value = departmentIndicatorValue.Value;
                    departmentIndicatorValueModify.IsLocked = departmentIndicatorValue.IsLocked;
                    departmentIndicatorValueModify.IndicatorStandardId = departmentIndicatorValue.IndicatorStandardId;
                    departmentIndicatorValueModify.UpdateTime = DateTime.Now;
                }
                //client win
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

                        // Update original values from the database 
                        var entry = ex.Entries.Single();
                        entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                    }

                } while (saveFailed);
                return View(departmentIndicatorValueModify);
            }
            return View(departmentIndicatorValue);
        }


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
