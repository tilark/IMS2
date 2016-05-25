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
using PagedList;
using System.Data.Entity.Infrastructure;
using IMS2.ViewModels;

namespace IMS2.Controllers
{
    [Authorize(Roles = "管理基础数据, Administrators")]

    public class IndicatorsController : Controller
    {
        private ImsDbContext db = new ImsDbContext();

        // GET: Indicators
        public async Task<ActionResult> Index(int? page, IMSMessageIdEnum? message)
        {
            ViewBag.StatusMessage =
              message == IMSMessageIdEnum.CreateSuccess ? "已创建新项。"
              : message == IMSMessageIdEnum.EditdSuccess ? "已更新完成。"
              : message == IMSMessageIdEnum.DeleteSuccess ? "已删除成功。"
              : message == IMSMessageIdEnum.CreateError ? "创建项目出现错误。"
              : message == IMSMessageIdEnum.EditError ? "有重名，无法更新相关信息。"
              : message == IMSMessageIdEnum.DeleteError ? "不允许删除该项。"
              : "";
            var indicators = db.Indicators.Include(i => i.DataSourceSystem).Include(i => i.Department).Include(i => i.ProvidingDepartment).Include(i => i.Duration).OrderBy(i => i.Priority);
            int pageSize = 30;
            int pageNumber = page ?? 1;
            return View(await indicators.ToPagedListAsync(pageNumber, pageSize));
        }

        // GET: Indicators/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Indicator indicator = await db.Indicators.FindAsync(id);
            if (indicator == null)
            {
                return HttpNotFound();
            }
            return View(indicator);
        }

        // GET: Indicators/Create
        public ActionResult Create()
        {
            ViewBag.DataSourceSystemId = new SelectList(db.DataSourceSystems, "DataSourceSystemId", "DataSourceSystemName");
            ViewBag.DutyDepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName");
            ViewBag.ProvidingDepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName");
            ViewBag.DurationId = new SelectList(db.Durations, "DurationId", "DurationName");
            return View();
        }

        // POST: Indicators/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IndicatorId,IndicatorName,Unit,IsAutoGetData,ProvidingDepartmentId,DataSourceSystemId,DutyDepartmentId,DurationId,Priority,Remarks,TimeStamp")] Indicator indicator)
        {
            if (ModelState.IsValid)
            {
                var query = await db.Indicators.Where(i => i.IndicatorName == indicator.IndicatorName).FirstOrDefaultAsync();
                if(query == null)
                {
                    if (indicator.IsAutoGetData == true && indicator.DataSourceSystemId != null)
                    {
                        indicator.ProvidingDepartmentId = null;
                    }
                    else if(indicator.IsAutoGetData == false && indicator.ProvidingDepartmentId != null)
                    {
                        indicator.DataSourceSystemId = null;
                    }
                    else
                    {
                        //错误
                        return RedirectToAction("Index", new { message = IMSMessageIdEnum.CreateError });

                    }
                    indicator.IndicatorId = Guid.NewGuid();
                    db.Indicators.Add(indicator);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", new { message = IMSMessageIdEnum.CreateSuccess });

                }
                else
                {
                    return RedirectToAction("Index", new { message = IMSMessageIdEnum.CreateError });
                }

            }

            ViewBag.DataSourceSystemId = new SelectList(db.DataSourceSystems, "DataSourceSystemId", "DataSourceSystemName", indicator.DataSourceSystemId);
            ViewBag.DutyDepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", indicator.DutyDepartmentId);
            ViewBag.ProvidingDepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", indicator.ProvidingDepartmentId);
            ViewBag.DurationId = new SelectList(db.Durations, "DurationId", "DurationName", indicator.DurationId);
            return View(indicator);
        }

        // GET: Indicators/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Indicator indicator = await db.Indicators.FindAsync(id);
            if (indicator == null)
            {
                return HttpNotFound();
            }
            ViewBag.DataSourceSystemId = new SelectList(db.DataSourceSystems, "DataSourceSystemId", "DataSourceSystemName", indicator.DataSourceSystemId);
            ViewBag.DutyDepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", indicator.DutyDepartmentId);
            ViewBag.ProvidingDepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", indicator.ProvidingDepartmentId);
            ViewBag.DurationId = new SelectList(db.Durations, "DurationId", "DurationName", indicator.DurationId);
            return View(indicator);
        }

        // POST: Indicators/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Indicator indicator)
        {
            if (ModelState.IsValid)
            {
                if (TryUpdateModel(indicator, "", new string[] { "IndicatorName", "Unit", "IsAutoGetData", "ProvidingDepartmentId", "DataSourceSystemId", "DutyDepartmentId", "DurationId", "Priority" ,"Remarks" }))
                {
                    var query = await db.Indicators.Where(d => d.IndicatorName == indicator.IndicatorName && d.IndicatorId != indicator.IndicatorId).FirstOrDefaultAsync();
                    if (query != null)
                    {
                        ModelState.AddModelError("", String.Format("已有指标名：{0}", indicator.IndicatorName));

                    }
                    else
                    {
                        if (indicator.IsAutoGetData == true)
                        {
                            //DataSourceSystemId不能为Null，否则就出错
                            indicator.ProvidingDepartmentId = null;
                        }
                        else
                        {
                            indicator.DataSourceSystemId = null;
                        }
                        db.Entry(indicator).State = EntityState.Modified;
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
            ViewBag.DataSourceSystemId = new SelectList(db.DataSourceSystems, "DataSourceSystemId", "DataSourceSystemName", indicator.DataSourceSystemId);
            ViewBag.DutyDepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", indicator.DutyDepartmentId);
            ViewBag.ProvidingDepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", indicator.ProvidingDepartmentId);
            ViewBag.DurationId = new SelectList(db.Durations, "DurationId", "DurationName", indicator.DurationId);
            return View(indicator);
        }

        // GET: Indicators/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Indicator indicator = await db.Indicators.FindAsync(id);
            if (indicator == null)
            {
                return HttpNotFound();
            }
            return View(indicator);
        }

        // POST: Indicators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {

            Indicator indicator = await db.Indicators.FindAsync(id);
            if(indicator.DepartmentIndicatorStandards.Count <= 0
                && indicator.DepartmentIndicatorValues.Count <= 0
                && indicator.IndicatorGroupMapIndicators.Count <= 0)
            {
                db.Indicators.Remove(indicator);
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
