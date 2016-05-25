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
using IMS2.ViewModels;
using System.Data.Entity.Infrastructure;

namespace IMS2.Controllers
{
    [Authorize(Roles = "管理基础数据, Administrators")]

    public class DepartmentController : Controller
    {
        private ImsDbContext db = new ImsDbContext();

        // GET: Department
        public async Task<ActionResult> Index(IMSMessageIdEnum? message)
        {
            ViewBag.StatusMessage =
               message == IMSMessageIdEnum.CreateSuccess ? "已创建新项。"
               : message == IMSMessageIdEnum.EditdSuccess ? "已更新完成。"
               : message == IMSMessageIdEnum.DeleteSuccess ? "已删除成功。"
               : message == IMSMessageIdEnum.CreateError ? "创建项目出现错误。"
               : message == IMSMessageIdEnum.EditError ? "有重名，无法更新相关信息。"
               : message == IMSMessageIdEnum.DeleteError ? "不允许删除该项。"
               : "";
            var departments = db.Departments.Include(d => d.DepartmentCategory);
            return View(await departments.OrderBy(d => d.Priority).ToListAsync());
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
                var query = await db.Departments.Where(d => d.DepartmentId == department.DepartmentId || d.DepartmentName == department.DepartmentName)
                            .SingleOrDefaultAsync();
                if (query == null)
                {
                    db.Departments.Add(department);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", new { message = IMSMessageIdEnum.CreateSuccess });

                }
                else
                {
                    return RedirectToAction("Index", new { message = IMSMessageIdEnum.CreateError });
                }
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
                if (TryUpdateModel(department, "", new string[] { "DepartmentCategoryId", "DepartmentName", "Priority", "Remarks" }))
                {
                    var query = await db.Departments.Where(d => d.DepartmentName == department.DepartmentName
                                        && d.DepartmentId != department.DepartmentId).FirstOrDefaultAsync();
                    if (query != null)
                    {
                        ModelState.AddModelError("", String.Format("已有科室名：{0}", department.DepartmentName));

                    }
                    else
                    {
                        db.Entry(department).State = EntityState.Modified;
                        //client win
                        bool saveFailed;
                        do
                        {
                            saveFailed = false;
                            try
                            {
                                await db.SaveChangesAsync();

                            }
                            catch (DbUpdateConcurrencyException ex)
                            {
                                saveFailed = true;

                                // Update original values from the database 
                                var entry = ex.Entries.Single();
                                entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                            }

                        } while (saveFailed);

                        return RedirectToAction("Index", new { message = IMSMessageIdEnum.EditdSuccess });
                    }
                    
                }
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
            if( department.DepartmentIndicatorValues.Count <= 0
                && department.DepartmentIndicatorStandards.Count <= 0
                && department.ProvidingIndicators.Count <= 0)
            {
                db.Departments.Remove(department);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new { message = IMSMessageIdEnum.DeleteSuccess });
            }
            else
            {
                return RedirectToAction("Index", new { message = IMSMessageIdEnum.DeleteError });
            }
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
