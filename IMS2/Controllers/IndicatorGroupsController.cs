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
    public class IndicatorGroupsController : Controller
    {
        private ImsDbContext db = new ImsDbContext();

        // GET: IndicatorGroups
        public async Task<ActionResult> Index()
        {
            return View(await db.IndicatorGroups.ToListAsync());
        }

        // GET: IndicatorGroups/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndicatorGroup indicatorGroup = await db.IndicatorGroups.FindAsync(id);
            if (indicatorGroup == null)
            {
                return HttpNotFound();
            }
            return View(indicatorGroup);
        }

        // GET: IndicatorGroups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: IndicatorGroups/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IndicatorGroupId,IndicatorGroupName,Priority,Remarks,TimeStamp")] IndicatorGroup indicatorGroup)
        {
            if (ModelState.IsValid)
            {
                indicatorGroup.IndicatorGroupId = Guid.NewGuid();
                db.IndicatorGroups.Add(indicatorGroup);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(indicatorGroup);
        }

        // GET: IndicatorGroups/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndicatorGroup indicatorGroup = await db.IndicatorGroups.FindAsync(id);
            if (indicatorGroup == null)
            {
                return HttpNotFound();
            }
            return View(indicatorGroup);
        }

        // POST: IndicatorGroups/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IndicatorGroupId,IndicatorGroupName,Priority,Remarks,TimeStamp")] IndicatorGroup indicatorGroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(indicatorGroup).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(indicatorGroup);
        }

        // GET: IndicatorGroups/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndicatorGroup indicatorGroup = await db.IndicatorGroups.FindAsync(id);
            if (indicatorGroup == null)
            {
                return HttpNotFound();
            }
            return View(indicatorGroup);
        }

        // POST: IndicatorGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            IndicatorGroup indicatorGroup = await db.IndicatorGroups.FindAsync(id);
            db.IndicatorGroups.Remove(indicatorGroup);
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
