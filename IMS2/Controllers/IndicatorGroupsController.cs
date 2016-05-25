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

    public class IndicatorGroupsController : Controller
    {
        private ImsDbContext db = new ImsDbContext();

        // GET: IndicatorGroups
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
            return View(await db.IndicatorGroups.OrderBy(i=>i.Priority).ToListAsync());
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
                try
                {
                    var query = await db.IndicatorGroups.Where(d => d.IndicatorGroupId == indicatorGroup.IndicatorGroupId || d.IndicatorGroupName == indicatorGroup.IndicatorGroupName)
                              .SingleOrDefaultAsync();
                    if (query == null)
                    {
                        indicatorGroup.IndicatorGroupId = System.Guid.NewGuid();
                        db.IndicatorGroups.Add(indicatorGroup);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index", new { message = IMSMessageIdEnum.CreateSuccess });

                    }
                    else
                    {
                        return RedirectToAction("Index", new { message = IMSMessageIdEnum.CreateError });
                    }

                }
                catch (Exception)
                {

                    ModelState.AddModelError("", "数据库中有重名，与管理员联系解决！");
                }
                //查找是否有要同的Guid与相同的名称
               
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
                var query = await db.IndicatorGroups.Where(i => i.IndicatorGroupName == indicatorGroup.IndicatorGroupName
                           && i.IndicatorGroupId != indicatorGroup.IndicatorGroupId).SingleOrDefaultAsync();
                if(query != null)
                {
                    ModelState.AddModelError("", String.Format("不能出现同名：{0}", indicatorGroup.IndicatorGroupName));
                }
                else
                {
                    //只能更改优先级与备注信息
                    db.Entry(indicatorGroup).State = EntityState.Modified;
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
            if(indicatorGroup.IndicatorGroupMapIndicators.Count <= 0 &&
                indicatorGroup.DepartmentCategoryMapIndicatorGroups.Count <= 0)
            {
                db.IndicatorGroups.Remove(indicatorGroup);
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
