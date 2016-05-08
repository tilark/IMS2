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
    public class DepartmentCategoryMapIndicatorGroupsController : Controller
    {
        private ImsDbContext db = new ImsDbContext();

        // GET: DepartmentCategoryMapIndicatorGroups
        public async Task<ActionResult> Index()
        {
            var departmentCategoryMapIndicatorGroups = db.DepartmentCategoryMapIndicatorGroups.Include(d => d.DepartmentCategory).Include(d => d.IndicatorGroup)
                .OrderBy(d=>d.DepartmentCategory.DepartmentCategoryName).ThenBy(d=>d.Priority);
            return View(await departmentCategoryMapIndicatorGroups.ToListAsync());
        }

        // GET: DepartmentCategoryMapIndicatorGroups/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DepartmentCategoryMapIndicatorGroup departmentCategoryMapIndicatorGroup = await db.DepartmentCategoryMapIndicatorGroups.FindAsync(id);
            if (departmentCategoryMapIndicatorGroup == null)
            {
                return HttpNotFound();
            }
            return View(departmentCategoryMapIndicatorGroup);
        }

        // GET: DepartmentCategoryMapIndicatorGroups/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentCategoryId = new SelectList(db.DepartmentCategories, "DepartmentCategoryId", "DepartmentCategoryName");
            ViewBag.IndicatorGroupId = new SelectList(db.IndicatorGroups, "IndicatorGroupId", "IndicatorGroupName");
            return View();
        }

        // POST: DepartmentCategoryMapIndicatorGroups/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DepartmentCategoryMapIndicatorGroupId,DepartmentCategoryId,IndicatorGroupId,Priority,Remarks,TimeStamp")] DepartmentCategoryMapIndicatorGroup departmentCategoryMapIndicatorGroup)
        {
            if (ModelState.IsValid)
            {
                departmentCategoryMapIndicatorGroup.DepartmentCategoryMapIndicatorGroupId = Guid.NewGuid();
                db.DepartmentCategoryMapIndicatorGroups.Add(departmentCategoryMapIndicatorGroup);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.DepartmentCategoryId = new SelectList(db.DepartmentCategories, "DepartmentCategoryId", "DepartmentCategoryName", departmentCategoryMapIndicatorGroup.DepartmentCategoryId);
            ViewBag.IndicatorGroupId = new SelectList(db.IndicatorGroups, "IndicatorGroupId", "IndicatorGroupName", departmentCategoryMapIndicatorGroup.IndicatorGroupId);
            return View(departmentCategoryMapIndicatorGroup);
        }

        // GET: DepartmentCategoryMapIndicatorGroups/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DepartmentCategoryMapIndicatorGroup departmentCategoryMapIndicatorGroup = await db.DepartmentCategoryMapIndicatorGroups.FindAsync(id);
            if (departmentCategoryMapIndicatorGroup == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentCategoryId = new SelectList(db.DepartmentCategories, "DepartmentCategoryId", "DepartmentCategoryName", departmentCategoryMapIndicatorGroup.DepartmentCategoryId);
            ViewBag.IndicatorGroupId = new SelectList(db.IndicatorGroups, "IndicatorGroupId", "IndicatorGroupName", departmentCategoryMapIndicatorGroup.IndicatorGroupId);
            return View(departmentCategoryMapIndicatorGroup);
        }

        // POST: DepartmentCategoryMapIndicatorGroups/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "DepartmentCategoryMapIndicatorGroupId,DepartmentCategoryId,IndicatorGroupId,Priority,Remarks,TimeStamp")] DepartmentCategoryMapIndicatorGroup departmentCategoryMapIndicatorGroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(departmentCategoryMapIndicatorGroup).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentCategoryId = new SelectList(db.DepartmentCategories, "DepartmentCategoryId", "DepartmentCategoryName", departmentCategoryMapIndicatorGroup.DepartmentCategoryId);
            ViewBag.IndicatorGroupId = new SelectList(db.IndicatorGroups, "IndicatorGroupId", "IndicatorGroupName", departmentCategoryMapIndicatorGroup.IndicatorGroupId);
            return View(departmentCategoryMapIndicatorGroup);
        }

        // GET: DepartmentCategoryMapIndicatorGroups/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DepartmentCategoryMapIndicatorGroup departmentCategoryMapIndicatorGroup = await db.DepartmentCategoryMapIndicatorGroups.FindAsync(id);
            if (departmentCategoryMapIndicatorGroup == null)
            {
                return HttpNotFound();
            }
            return View(departmentCategoryMapIndicatorGroup);
        }

        // POST: DepartmentCategoryMapIndicatorGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            DepartmentCategoryMapIndicatorGroup departmentCategoryMapIndicatorGroup = await db.DepartmentCategoryMapIndicatorGroups.FindAsync(id);
            db.DepartmentCategoryMapIndicatorGroups.Remove(departmentCategoryMapIndicatorGroup);
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
