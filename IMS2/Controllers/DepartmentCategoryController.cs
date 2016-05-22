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
    public class DepartmentCategoryController : Controller
    {
        private ImsDbContext db = new ImsDbContext();

        // GET: DepartmentCategory
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
            return View(await db.DepartmentCategories.OrderBy(d => d.Priority).ToListAsync());
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
                var query = await db.DepartmentCategories.Where(d => d.DepartmentCategoryId == departmentCategory.DepartmentCategoryId || d.DepartmentCategoryName == departmentCategory.DepartmentCategoryName)
                            .SingleOrDefaultAsync();
                if (query == null)
                {
                    db.DepartmentCategories.Add(departmentCategory);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", new { message = IMSMessageIdEnum.CreateSuccess });

                }
                else
                {
                    return RedirectToAction("Index", new { message = IMSMessageIdEnum.CreateError });
                }
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
            ViewBag.DepartmentCategoryName = departmentCategory.DepartmentCategoryName;

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
                //是否有重名
                var query = await db.DepartmentCategories.Where(d => d.DepartmentCategoryName == departmentCategory.DepartmentCategoryName
                                                && d.DepartmentCategoryId != departmentCategory.DepartmentCategoryId).FirstOrDefaultAsync();
                if (query != null)
                {
                    //有两个同名的科室
                    ModelState.AddModelError("", String.Format("不能出现同名：{0}", departmentCategory.DepartmentCategoryName));

                }
                else
                {
                    //只能更改优先级与备注信息
                    db.Entry(departmentCategory).State = EntityState.Modified;
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
            if (departmentCategory.DepartmentCategoryMapIndicatorGroups.Count <= 0
                && departmentCategory.Departments.Count <= 0)
            {
                db.DepartmentCategories.Remove(departmentCategory);
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
                return RedirectToAction("Index", new { message = IMSMessageIdEnum.DeleteSuccess });
            }
            return RedirectToAction("Index", new { message = IMSMessageIdEnum.DeleteError });
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
