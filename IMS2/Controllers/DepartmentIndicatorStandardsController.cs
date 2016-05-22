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
using PagedList;

namespace IMS2.Controllers
{
    public class DepartmentIndicatorStandardsController : Controller
    {
        private ImsDbContext db = new ImsDbContext();

        // GET: DepartmentIndicatorStandards
        public async Task<ActionResult> Index(IMSMessageIdEnum? message, int? page)
        {
            ViewBag.StatusMessage =
               message == IMSMessageIdEnum.CreateSuccess ? "已创建新项。"
               : message == IMSMessageIdEnum.EditdSuccess ? "已更新完成。"
               : message == IMSMessageIdEnum.DeleteSuccess ? "已删除成功。"
               : message == IMSMessageIdEnum.CreateError ? "创建项目出现错误。"
               : message == IMSMessageIdEnum.EditError ? "有重名，无法更新相关信息。"
               : message == IMSMessageIdEnum.DeleteError ? "不允许删除该项。"
               : "";
            int pageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["pagSize"]);
            int pageNumber = (page ?? 1);
            var departmentIndicatorStandards = db.DepartmentIndicatorStandards.Include(d => d.Department).Include(d => d.Indicator);
            return View(await departmentIndicatorStandards.OrderByDescending(i => i.Version).ToPagedListAsync(pageNumber, pageSize));
        }

        // GET: DepartmentIndicatorStandards/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DepartmentIndicatorStandard departmentIndicatorStandard = await db.DepartmentIndicatorStandards.FindAsync(id);
            if (departmentIndicatorStandard == null)
            {
                return HttpNotFound();
            }
            DepartmentIndicatorStandardView viewModel = new DepartmentIndicatorStandardView
            {
                DepartmentIndicatorStandardId = departmentIndicatorStandard.DepartmentIndicatorStandardId,
                DepartmentId = departmentIndicatorStandard.DepartmentId,
                IndicatorId = departmentIndicatorStandard.IndicatorId,
                Range = departmentIndicatorStandard.Range,
                UpdateTime = departmentIndicatorStandard.UpdateTime,
                Version = departmentIndicatorStandard.Version,
                Remarks = departmentIndicatorStandard.Remarks,
                DepartmentName = departmentIndicatorStandard.Department.DepartmentName,
                IndicatorName = departmentIndicatorStandard.Indicator.IndicatorName
            };
            return View(viewModel);
        }

        // GET: DepartmentIndicatorStandards/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments.OrderBy(d => d.Priority), "DepartmentId", "DepartmentName");
            ViewBag.IndicatorId = new SelectList(db.Indicators.OrderBy(i => i.Priority), "IndicatorId", "IndicatorName");
            return View();
        }

        // POST: DepartmentIndicatorStandards/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DepartmentIndicatorStandardView model)
        {
            if (ModelState.IsValid)
            {
                //查重，根据科室、指标、版本号
                var query = await db.DepartmentIndicatorStandards.Where(d => d.DepartmentId == model.DepartmentId
                                                                && d.IndicatorId == model.IndicatorId
                                                                && d.Version == model.Version).FirstOrDefaultAsync();
                if (query == null)
                {
                    DepartmentIndicatorStandard departmentIndicatorStandard = new DepartmentIndicatorStandard
                    {
                        DepartmentIndicatorStandardId = System.Guid.NewGuid(),
                        UpdateTime = System.DateTime.Now,
                        DepartmentId = model.DepartmentId,
                        IndicatorId = model.IndicatorId,
                        UpperBound = model.UpperBound,
                        UpperBoundIncluded = model.UpperBoundIncluded,
                        LowerBound = model.LowerBound,
                        LowerBoundIncluded = model.LowerBoundIncluded,
                        Version = model.Version,
                        Remarks = model.Remarks
                    };

                    db.DepartmentIndicatorStandards.Add(departmentIndicatorStandard);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", new { message = IMSMessageIdEnum.CreateSuccess });
                }
                else
                {
                    ModelState.AddModelError("", "有相同版本标准值!");
                }
            }

            ViewBag.DepartmentId = new SelectList(db.Departments.OrderBy(d => d.Priority), "DepartmentId", "DepartmentName", model.DepartmentId);
            ViewBag.IndicatorId = new SelectList(db.Indicators.OrderBy(d => d.Priority), "IndicatorId", "IndicatorName", model.IndicatorId);
            return View(model);
        }

        // GET: DepartmentIndicatorStandards/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DepartmentIndicatorStandard departmentIndicatorStandard = await db.DepartmentIndicatorStandards.FindAsync(id);
            if (departmentIndicatorStandard == null)
            {
                return HttpNotFound();
            }
            DepartmentIndicatorStandardView viewModel = new DepartmentIndicatorStandardView
            {
                DepartmentIndicatorStandardId = departmentIndicatorStandard.DepartmentIndicatorStandardId,
                DepartmentId = departmentIndicatorStandard.DepartmentId,
                IndicatorId = departmentIndicatorStandard.IndicatorId,
                UpperBoundIncluded = departmentIndicatorStandard.UpperBoundIncluded != null ? departmentIndicatorStandard.UpperBoundIncluded.Value : false,
                LowerBoundIncluded = departmentIndicatorStandard.LowerBoundIncluded != null ? departmentIndicatorStandard.LowerBoundIncluded.Value : false,
                UpdateTime = departmentIndicatorStandard.UpdateTime,
                Version = departmentIndicatorStandard.Version,
                Remarks = departmentIndicatorStandard.Remarks,
                DepartmentName = departmentIndicatorStandard.Department.DepartmentName,
                IndicatorName = departmentIndicatorStandard.Indicator.IndicatorName
            };
            if (departmentIndicatorStandard.UpperBound.HasValue)
            {
                viewModel.UpperBound = departmentIndicatorStandard.UpperBound.Value;
            }
            if (departmentIndicatorStandard.LowerBound.HasValue)
            {
                viewModel.LowerBound = departmentIndicatorStandard.LowerBound.Value;
            }
            return View(viewModel);
        }

        // POST: DepartmentIndicatorStandards/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(DepartmentIndicatorStandardView model)
        {
            if (ModelState.IsValid)
            {
                var departmentIndicatorStandard = await db.DepartmentIndicatorStandards.FindAsync(model.DepartmentIndicatorStandardId);
                //查重，根据科室、指标、版本号
                var query = await db.DepartmentIndicatorStandards.Where(d => d.DepartmentId == departmentIndicatorStandard.DepartmentId
                                                                && d.IndicatorId == departmentIndicatorStandard.IndicatorId
                                                                && d.Version == departmentIndicatorStandard.Version
                                                                && d.DepartmentIndicatorStandardId != departmentIndicatorStandard.DepartmentIndicatorStandardId).FirstOrDefaultAsync();
                if (query != null)
                {
                    //有两个同名的科室
                    ModelState.AddModelError("", String.Format("相同科室与指标不能出现同版本项目：{0}", departmentIndicatorStandard.Version));

                }
                else
                {
                    departmentIndicatorStandard.UpdateTime = System.DateTime.Now;
                    departmentIndicatorStandard.UpperBound = model.UpperBound;
                    departmentIndicatorStandard.UpperBoundIncluded = model.UpperBoundIncluded;
                    departmentIndicatorStandard.LowerBound = model.LowerBound;
                    departmentIndicatorStandard.LowerBoundIncluded = model.LowerBoundIncluded;
                    departmentIndicatorStandard.Version = model.Version;
                    departmentIndicatorStandard.Remarks = model.Remarks;
                    //db.Entry(departmentIndicatorStandard).State = EntityState.Modified;
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
            //ViewBag.DepartmentId = new SelectList(db.Departments.OrderBy(d=>d.Priority), "DepartmentId", "DepartmentName", departmentIndicatorStandard.DepartmentId);
            //ViewBag.IndicatorId = new SelectList(db.Indicators.OrderBy(d => d.Priority), "IndicatorId", "IndicatorName", departmentIndicatorStandard.IndicatorId);
            return View(model);
        }

        // GET: DepartmentIndicatorStandards/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DepartmentIndicatorStandard departmentIndicatorStandard = await db.DepartmentIndicatorStandards.FindAsync(id);
            if (departmentIndicatorStandard == null)
            {
                return HttpNotFound();
            }
            return View(departmentIndicatorStandard);
        }

        // POST: DepartmentIndicatorStandards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            DepartmentIndicatorStandard departmentIndicatorStandard = await db.DepartmentIndicatorStandards.FindAsync(id);
            if (departmentIndicatorStandard.DepartmentIndicatorValues.Count <= 0)
            {
                db.DepartmentIndicatorStandards.Remove(departmentIndicatorStandard);
                await db.SaveChangesAsync();
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
