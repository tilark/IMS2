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
    public class IndicatorsController : Controller
    {
        private ImsDbContext db = new ImsDbContext();

        // GET: Indicators
        public async Task<ActionResult> Index()
        {
            var indicators = db.Indicators.Include(i => i.DataSourceSystem).Include(i => i.Department).Include(i => i.Department1).Include(i => i.Duration).OrderBy(i=>i.Priority);
            return View(await indicators.ToListAsync());
        }

        // GET: Indicators/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Indicator indicator = await db.Indicators.FindAsync(id);
            if (indicator == null)
            {
                return HttpNotFound();
            }
            return View(indicator);
        }

        // GET: Indicators/Create
        public ActionResult Create()
        {
            ViewBag.DataSourceSystemId = new SelectList(db.DataSourceSystems, "DataSourceSystemId", "DataSourceSystemName");
            ViewBag.DutyDepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName");
            ViewBag.ProvidingDepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName");
            ViewBag.DurationId = new SelectList(db.Durations, "DurationId", "DurationName");
            return View();
        }

        // POST: Indicators/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IndicatorId,IndicatorName,Unit,IsAutoGetData,ProvidingDepartmentId,DataSourceSystemId,DutyDepartmentId,DurationId,Priority,Remarks,TimeStamp")] Indicator indicator)
        {
            if (ModelState.IsValid)
            {
                indicator.IndicatorId = Guid.NewGuid();
                db.Indicators.Add(indicator);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.DataSourceSystemId = new SelectList(db.DataSourceSystems, "DataSourceSystemId", "DataSourceSystemName", indicator.DataSourceSystemId);
            ViewBag.DutyDepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", indicator.DutyDepartmentId);
            ViewBag.ProvidingDepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", indicator.ProvidingDepartmentId);
            ViewBag.DurationId = new SelectList(db.Durations, "DurationId", "DurationName", indicator.DurationId);
            return View(indicator);
        }

        // GET: Indicators/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Indicator indicator = await db.Indicators.FindAsync(id);
            if (indicator == null)
            {
                return HttpNotFound();
            }
            ViewBag.DataSourceSystemId = new SelectList(db.DataSourceSystems, "DataSourceSystemId", "DataSourceSystemName", indicator.DataSourceSystemId);
            ViewBag.DutyDepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", indicator.DutyDepartmentId);
            ViewBag.ProvidingDepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", indicator.ProvidingDepartmentId);
            ViewBag.DurationId = new SelectList(db.Durations, "DurationId", "DurationName", indicator.DurationId);
            return View(indicator);
        }

        // POST: Indicators/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IndicatorId,IndicatorName,Unit,IsAutoGetData,ProvidingDepartmentId,DataSourceSystemId,DutyDepartmentId,DurationId,Priority,Remarks,TimeStamp")] Indicator indicator)
        {
            if (ModelState.IsValid)
            {
                db.Entry(indicator).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.DataSourceSystemId = new SelectList(db.DataSourceSystems, "DataSourceSystemId", "DataSourceSystemName", indicator.DataSourceSystemId);
            ViewBag.DutyDepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", indicator.DutyDepartmentId);
            ViewBag.ProvidingDepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", indicator.ProvidingDepartmentId);
            ViewBag.DurationId = new SelectList(db.Durations, "DurationId", "DurationName", indicator.DurationId);
            return View(indicator);
        }

        // GET: Indicators/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Indicator indicator = await db.Indicators.FindAsync(id);
            if (indicator == null)
            {
                return HttpNotFound();
            }
            return View(indicator);
        }

        // POST: Indicators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            Indicator indicator = await db.Indicators.FindAsync(id);
            db.Indicators.Remove(indicator);
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
