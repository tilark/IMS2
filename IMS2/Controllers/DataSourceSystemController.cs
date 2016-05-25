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
using System.Data.Entity.Infrastructure;
using IMS2.ViewModels;

namespace IMS2.Controllers
{
    [Authorize(Roles = "管理基础数据, Administrators")]

    public class DataSourceSystemController : Controller
    {
        private ImsDbContext db = new ImsDbContext();

        // GET: DataSourceSystem
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
                //查找是否有要同的Guid与相同的名称
                var query = await db.DataSourceSystems.Where(d => d.DataSourceSystemId == dataSourceSystem.DataSourceSystemId || d.DataSourceSystemName == dataSourceSystem.DataSourceSystemName)
                            .SingleOrDefaultAsync();
                if (query == null)
                {
                    db.DataSourceSystems.Add(dataSourceSystem);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", new { message = IMSMessageIdEnum.CreateSuccess });

                }
                else
                {
                    return RedirectToAction("Index", new { message = IMSMessageIdEnum.CreateError });
                }
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
            ViewBag.DataSourceSystemName = dataSourceSystem.DataSourceSystemName;
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
                //只能更改优先级与备注信息
                db.Entry(dataSourceSystem).State = EntityState.Modified;
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
            //如果数据来源系统中有指标集合，不能删除
            if (dataSourceSystem.Indicators.Count <= 0)
            {
                db.DataSourceSystems.Remove(dataSourceSystem);
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
