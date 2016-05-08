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
    public class DepartmentController : Controller
    {
        private ImsDbContext db = new ImsDbContext();

        // GET: Department
        public async Task<ActionResult> Index()
        {
            var departments = db.Departments.Include(d => d.DepartmentCategory);
            return View(await departments.OrderBy(d=>d.Priority).ToListAsync());
        }

        // GET: Department/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = await db.Departments.FindAsync(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // GET: Department/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentCategoryId = new SelectList(db.DepartmentCategories, "DepartmentCategoryId", "DepartmentCategoryName");
            return View();
        }

        // POST: Department/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DepartmentId,DepartmentCategoryId,DepartmentName,Priority,Remarks,TimeStamp")] Department department)
        {
            if (ModelState.IsValid)
            {
                department.DepartmentId = Guid.NewGuid();
                db.Departments.Add(department);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.DepartmentCategoryId = new SelectList(db.DepartmentCategories, "DepartmentCategoryId", "DepartmentCategoryName", department.DepartmentCategoryId);
            return View(department);
        }

        // GET: Department/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = await db.Departments.FindAsync(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentCategoryId = new SelectList(db.DepartmentCategories, "DepartmentCategoryId", "DepartmentCategoryName", department.DepartmentCategoryId);
            return View(department);
        }

        // POST: Department/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "DepartmentId,DepartmentCategoryId,DepartmentName,Priority,Remarks,TimeStamp")] Department department)
        {
            if (ModelState.IsValid)
            {
                db.Entry(department).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentCategoryId = new SelectList(db.DepartmentCategories, "DepartmentCategoryId", "DepartmentCategoryName", department.DepartmentCategoryId);
            return View(department);
        }

        // GET: Department/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = await db.Departments.FindAsync(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            Department department = await db.Departments.FindAsync(id);
            db.Departments.Remove(department);
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
