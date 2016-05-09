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
    public class ProvidingDepartmentIndicatorController : Controller
    {
        private ImsDbContext db = new ImsDbContext();

        // GET: ProvidingDepartmentIndicator
        public async Task<ActionResult> Index(DateTime? searchTime, Guid? departmentCategory)
        {
            //应该选择提供科室名列表
            ViewBag.departmentCategory = new SelectList(db.Indicators.Where(i=>i.IsAutoGetData == false).Select(d=>d.ProvidingDepartment).OrderBy(o=>o.Priority), "DepartmentId", "DepartmentName");

            var departmentIndicatorValues = db.DepartmentIndicatorValues.Include(d => d.Department).Include(d => d.DepartmentIndicatorStandard).Include(d => d.Indicator);
            return View(await departmentIndicatorValues.ToListAsync());
        }

        // GET: ProvidingDepartmentIndicator/Details/5
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

        // GET: ProvidingDepartmentIndicator/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName");
            ViewBag.IndicatorStandardId = new SelectList(db.DepartmentIndicatorStandards, "DepartmentIndicatorStandardId", "Remarks");
            ViewBag.IndicatorId = new SelectList(db.Indicators, "IndicatorId", "IndicatorName");
            return View();
        }

        // POST: ProvidingDepartmentIndicator/Create
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

        // GET: ProvidingDepartmentIndicator/Edit/5
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

        // POST: ProvidingDepartmentIndicator/Edit/5
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

        // GET: ProvidingDepartmentIndicator/Delete/5
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

        // POST: ProvidingDepartmentIndicator/Delete/5
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
