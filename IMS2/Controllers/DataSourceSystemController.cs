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
    public class DataSourceSystemController : Controller
    {
        private ImsDbContext db = new ImsDbContext();

        // GET: DataSourceSystem
        public async Task<ActionResult> Index()
        {
            return View(await db.DataSourceSystems.ToListAsync());
        }

        // GET: DataSourceSystem/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSourceSystem dataSourceSystem = await db.DataSourceSystems.FindAsync(id);
            if (dataSourceSystem == null)
            {
                return HttpNotFound();
            }
            return View(dataSourceSystem);
        }

        // GET: DataSourceSystem/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DataSourceSystem/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DataSourceSystemId,DataSourceSystemName,Priority,Remarks,TimeStamp")] DataSourceSystem dataSourceSystem)
        {
            if (ModelState.IsValid)
            {
                dataSourceSystem.DataSourceSystemId = Guid.NewGuid();
                db.DataSourceSystems.Add(dataSourceSystem);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(dataSourceSystem);
        }

        // GET: DataSourceSystem/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSourceSystem dataSourceSystem = await db.DataSourceSystems.FindAsync(id);
            if (dataSourceSystem == null)
            {
                return HttpNotFound();
            }
            return View(dataSourceSystem);
        }

        // POST: DataSourceSystem/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "DataSourceSystemId,DataSourceSystemName,Priority,Remarks,TimeStamp")] DataSourceSystem dataSourceSystem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dataSourceSystem).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(dataSourceSystem);
        }

        // GET: DataSourceSystem/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSourceSystem dataSourceSystem = await db.DataSourceSystems.FindAsync(id);
            if (dataSourceSystem == null)
            {
                return HttpNotFound();
            }
            return View(dataSourceSystem);
        }

        // POST: DataSourceSystem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            DataSourceSystem dataSourceSystem = await db.DataSourceSystems.FindAsync(id);
            db.DataSourceSystems.Remove(dataSourceSystem);
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
