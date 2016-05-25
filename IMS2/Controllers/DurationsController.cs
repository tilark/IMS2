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
    [Authorize(Roles = "管理基础数据, Administrators")]

    public class DurationsController : Controller
    {
        private ImsDbContext db = new ImsDbContext();

        // GET: Durations
        public async Task<ActionResult> Index()
        {
            return View(await db.Durations.ToListAsync());
        }

        // GET: Durations/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Duration duration = await db.Durations.FindAsync(id);
            if (duration == null)
            {
                return HttpNotFound();
            }
            return View(duration);
        }

        // GET: Durations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Durations/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DurationId,DurationName,Remarks")] Duration duration)
        {
            if (ModelState.IsValid)
            {
                if(await IsNullByName(duration.DurationName))
                {
                    duration.DurationId = Guid.NewGuid();
                    db.Durations.Add(duration);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", String.Format("\"{0}\" 已存在！", duration.DurationName));
                }
            }
            return View(duration);
        }
        private async Task<bool> IsNullByName(string name)
        {
            bool result = false;
            if (!String.IsNullOrEmpty(name))
            {
                var query = await db.Durations.Where(d => d.DurationName == name).FirstOrDefaultAsync();
                result = query == null ? true : false;
            }
            return result;
        }
        // GET: Durations/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Duration duration = await db.Durations.FindAsync(id);
            if (duration == null)
            {
                return HttpNotFound();
            }
            return View(duration);
        }

        // POST: Durations/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "DurationId,DurationName,Remarks")] Duration duration)
        {
            if (ModelState.IsValid)
            {
                var query = await db.Durations.Where(d => d.DurationName == duration.DurationName && d.DurationId != duration.DurationId).FirstOrDefaultAsync();
                if (query == null)
                {
                    db.Entry(duration).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", String.Format("\"{0}\" 已存在！", duration.DurationName));
                }
            }
            return View(duration);
        }

        // GET: Durations/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Duration duration = await db.Durations.FindAsync(id);
            if (duration == null)
            {
                return HttpNotFound();
            }
            return View(duration);
        }

        // POST: Durations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            Duration duration = await db.Durations.FindAsync(id);
            db.Durations.Remove(duration);
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
