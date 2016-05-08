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
    public class DepartmentCategoryController : Controller
    {
        private ImsDbContext db = new ImsDbContext();

        // GET: DepartmentCategory
        public async Task<ActionResult> Index()
        {
            return View(await db.DepartmentCategories.OrderBy(d=>d.Priority).ToListAsync());
        }

        // GET: DepartmentCategory/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DepartmentCategory departmentCategory = await db.DepartmentCategories.FindAsync(id);
            if (departmentCategory == null)
            {
                return HttpNotFound();
            }
            return View(departmentCategory);
        }

        // GET: DepartmentCategory/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DepartmentCategory/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DepartmentCategoryId,DepartmentCategoryName,Priority,Remarks,TimeStamp")] DepartmentCategory departmentCategory)
        {
            if (ModelState.IsValid)
            {
                departmentCategory.DepartmentCategoryId = Guid.NewGuid();
                db.DepartmentCategories.Add(departmentCategory);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(departmentCategory);
        }

        // GET: DepartmentCategory/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DepartmentCategory departmentCategory = await db.DepartmentCategories.FindAsync(id);
            if (departmentCategory == null)
            {
                return HttpNotFound();
            }
            return View(departmentCategory);
        }

        // POST: DepartmentCategory/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "DepartmentCategoryId,DepartmentCategoryName,Priority,Remarks,TimeStamp")] DepartmentCategory departmentCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(departmentCategory).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(departmentCategory);
        }

        // GET: DepartmentCategory/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DepartmentCategory departmentCategory = await db.DepartmentCategories.FindAsync(id);
            if (departmentCategory == null)
            {
                return HttpNotFound();
            }
            return View(departmentCategory);
        }

        // POST: DepartmentCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            DepartmentCategory departmentCategory = await db.DepartmentCategories.FindAsync(id);
            db.DepartmentCategories.Remove(departmentCategory);
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
