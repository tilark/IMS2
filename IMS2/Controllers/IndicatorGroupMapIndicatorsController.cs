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
    public class IndicatorGroupMapIndicatorsController : Controller
    {
        private ImsDbContext db = new ImsDbContext();

        // GET: IndicatorGroupMapIndicators
        public async Task<ActionResult> Index()
        {
            var indicatorGroupMapIndicators = db.IndicatorGroupMapIndicators.Include(i => i.Indicator).Include(i => i.IndicatorGroup).OrderBy(i=>i.IndicatorGroup.IndicatorGroupName).ThenBy(i=>i.Priority);
            return View(await indicatorGroupMapIndicators.ToListAsync());
        }

        // GET: IndicatorGroupMapIndicators/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndicatorGroupMapIndicator indicatorGroupMapIndicator = await db.IndicatorGroupMapIndicators.FindAsync(id);
            if (indicatorGroupMapIndicator == null)
            {
                return HttpNotFound();
            }
            return View(indicatorGroupMapIndicator);
        }

        // GET: IndicatorGroupMapIndicators/Create
        public ActionResult Create()
        {
            ViewBag.IndicatorId = new SelectList(db.Indicators, "IndicatorId", "IndicatorName");
            ViewBag.IndicatorGroupId = new SelectList(db.IndicatorGroups, "IndicatorGroupId", "IndicatorGroupName");
            return View();
        }

        // POST: IndicatorGroupMapIndicators/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IndicatorGroupMapIndicatorId,IndicatorGroupId,IndicatorId,Priority,Remarks,TimeStamp")] IndicatorGroupMapIndicator indicatorGroupMapIndicator)
        {
            if (ModelState.IsValid)
            {
                indicatorGroupMapIndicator.IndicatorGroupMapIndicatorId = Guid.NewGuid();
                db.IndicatorGroupMapIndicators.Add(indicatorGroupMapIndicator);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.IndicatorId = new SelectList(db.Indicators, "IndicatorId", "IndicatorName", indicatorGroupMapIndicator.IndicatorId);
            ViewBag.IndicatorGroupId = new SelectList(db.IndicatorGroups, "IndicatorGroupId", "IndicatorGroupName", indicatorGroupMapIndicator.IndicatorGroupId);
            return View(indicatorGroupMapIndicator);
        }

        // GET: IndicatorGroupMapIndicators/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndicatorGroupMapIndicator indicatorGroupMapIndicator = await db.IndicatorGroupMapIndicators.FindAsync(id);
            if (indicatorGroupMapIndicator == null)
            {
                return HttpNotFound();
            }
            ViewBag.IndicatorId = new SelectList(db.Indicators, "IndicatorId", "IndicatorName", indicatorGroupMapIndicator.IndicatorId);
            ViewBag.IndicatorGroupId = new SelectList(db.IndicatorGroups, "IndicatorGroupId", "IndicatorGroupName", indicatorGroupMapIndicator.IndicatorGroupId);
            return View(indicatorGroupMapIndicator);
        }

        // POST: IndicatorGroupMapIndicators/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IndicatorGroupMapIndicatorId,IndicatorGroupId,IndicatorId,Priority,Remarks,TimeStamp")] IndicatorGroupMapIndicator indicatorGroupMapIndicator)
        {
            if (ModelState.IsValid)
            {
                db.Entry(indicatorGroupMapIndicator).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.IndicatorId = new SelectList(db.Indicators, "IndicatorId", "IndicatorName", indicatorGroupMapIndicator.IndicatorId);
            ViewBag.IndicatorGroupId = new SelectList(db.IndicatorGroups, "IndicatorGroupId", "IndicatorGroupName", indicatorGroupMapIndicator.IndicatorGroupId);
            return View(indicatorGroupMapIndicator);
        }

        // GET: IndicatorGroupMapIndicators/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndicatorGroupMapIndicator indicatorGroupMapIndicator = await db.IndicatorGroupMapIndicators.FindAsync(id);
            if (indicatorGroupMapIndicator == null)
            {
                return HttpNotFound();
            }
            return View(indicatorGroupMapIndicator);
        }

        // POST: IndicatorGroupMapIndicators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            IndicatorGroupMapIndicator indicatorGroupMapIndicator = await db.IndicatorGroupMapIndicators.FindAsync(id);
            db.IndicatorGroupMapIndicators.Remove(indicatorGroupMapIndicator);
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
