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

namespace IMS2.Controllers
{
    public class SearchDepartmentIndicatorController : Controller
    {
        private ImsDbContext db = new ImsDbContext();

        // GET: SearchDepartmentIndicator
        public async Task<ActionResult> Index(DateTime? startTime, DateTime? endTime, Guid? department)
        {
            ViewBag.department = new SelectList(db.Departments, "DepartmentId", "DepartmentName");
            if(startTime != null && endTime != null && department != null)
            {
               var departmentIndicatorValues = db.DepartmentIndicatorValues.Include(d => d.Department).Include(d => d.DepartmentIndicatorStandard).Include(d => d.Indicator.Duration)
                                    .Where(d => d.DepartmentId == department.Value
                                    && d.Time.Year >= startTime.Value.Year && d.Time.Month >= startTime.Value.Month
                                    && d.Time.Year <= endTime.Value.Year && d.Time.Month <= endTime.Value.Month);
                return View(await departmentIndicatorValues.ToListAsync());

            }
            return View();
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

        // GET: SearchDepartmentIndicator/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName");
            ViewBag.IndicatorStandardId = new SelectList(db.DepartmentIndicatorStandards, "DepartmentIndicatorStandardId", "Remarks");
            ViewBag.IndicatorId = new SelectList(db.Indicators, "IndicatorId", "IndicatorName");
            return View();
        }

        // POST: SearchDepartmentIndicator/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DepartmentIndicatorValueId,DepartmentId,IndicatorId,Time,Value,IndicatorStandardId,IsLocked,UpdateTime,TimeStamp")] DepartmentIndicatorValue departmentIndicatorValue)
        {
            if (ModelState.IsValid)
            {
                departmentIndicatorValue.DepartmentIndicatorValueId = Guid.NewGuid();
                db.DepartmentIndicatorValues.Add(departmentIndicatorValue);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", departmentIndicatorValue.DepartmentId);
            ViewBag.IndicatorStandardId = new SelectList(db.DepartmentIndicatorStandards, "DepartmentIndicatorStandardId", "Remarks", departmentIndicatorValue.IndicatorStandardId);
            ViewBag.IndicatorId = new SelectList(db.Indicators, "IndicatorId", "IndicatorName", departmentIndicatorValue.IndicatorId);
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
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", departmentIndicatorValue.DepartmentId);
            ViewBag.IndicatorStandardId = new SelectList(db.DepartmentIndicatorStandards, "DepartmentIndicatorStandardId", "Remarks", departmentIndicatorValue.IndicatorStandardId);
            ViewBag.IndicatorId = new SelectList(db.Indicators, "IndicatorId", "IndicatorName", departmentIndicatorValue.IndicatorId);
            return View(departmentIndicatorValue);
        }

        // POST: SearchDepartmentIndicator/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "DepartmentIndicatorValueId,DepartmentId,IndicatorId,Time,Value,IndicatorStandardId,IsLocked,UpdateTime,TimeStamp")] DepartmentIndicatorValue departmentIndicatorValue)
        {
            if (ModelState.IsValid)
            {
                db.Entry(departmentIndicatorValue).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", departmentIndicatorValue.DepartmentId);
            ViewBag.IndicatorStandardId = new SelectList(db.DepartmentIndicatorStandards, "DepartmentIndicatorStandardId", "Remarks", departmentIndicatorValue.IndicatorStandardId);
            ViewBag.IndicatorId = new SelectList(db.Indicators, "IndicatorId", "IndicatorName", departmentIndicatorValue.IndicatorId);
            return View(departmentIndicatorValue);
        }

        // GET: SearchDepartmentIndicator/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
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

        // POST: SearchDepartmentIndicator/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            DepartmentIndicatorValue departmentIndicatorValue = await db.DepartmentIndicatorValues.FindAsync(id);
            db.DepartmentIndicatorValues.Remove(departmentIndicatorValue);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
