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
    public class IndicatorAlgorithmsController : Controller
    {
        private ImsDbContext db = new ImsDbContext();

        // GET: IndicatorAlgorithms
        public async Task<ActionResult> Index()
        {
            return View(await db.IndicatorAlgorithms.ToListAsync());
        }

        // GET: IndicatorAlgorithms/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndicatorAlgorithm indicatorAlgorithm = await db.IndicatorAlgorithms.FindAsync(id);
            if (indicatorAlgorithm == null)
            {
                return HttpNotFound();
            }
            return View(indicatorAlgorithm);
        }

        // GET: IndicatorAlgorithms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: IndicatorAlgorithms/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IndicatorAlgorithmsId,ResultId,FirstOperandID,SecondOperandID,OperationMethod,Remarks")] IndicatorAlgorithm indicatorAlgorithm)
        {
            if (ModelState.IsValid)
            {
                indicatorAlgorithm.IndicatorAlgorithmsId = Guid.NewGuid();
                db.IndicatorAlgorithms.Add(indicatorAlgorithm);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(indicatorAlgorithm);
        }

        // GET: IndicatorAlgorithms/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndicatorAlgorithm indicatorAlgorithm = await db.IndicatorAlgorithms.FindAsync(id);
            if (indicatorAlgorithm == null)
            {
                return HttpNotFound();
            }
            return View(indicatorAlgorithm);
        }

        // POST: IndicatorAlgorithms/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IndicatorAlgorithmsId,ResultId,FirstOperandID,SecondOperandID,OperationMethod,Remarks")] IndicatorAlgorithm indicatorAlgorithm)
        {
            if (ModelState.IsValid)
            {
                db.Entry(indicatorAlgorithm).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(indicatorAlgorithm);
        }

        // GET: IndicatorAlgorithms/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndicatorAlgorithm indicatorAlgorithm = await db.IndicatorAlgorithms.FindAsync(id);
            if (indicatorAlgorithm == null)
            {
                return HttpNotFound();
            }
            return View(indicatorAlgorithm);
        }

        // POST: IndicatorAlgorithms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            IndicatorAlgorithm indicatorAlgorithm = await db.IndicatorAlgorithms.FindAsync(id);
            db.IndicatorAlgorithms.Remove(indicatorAlgorithm);
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
