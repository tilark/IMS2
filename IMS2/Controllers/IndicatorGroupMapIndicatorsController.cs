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
using PagedList;
using System.Data.Entity.Infrastructure;

namespace IMS2.Controllers
{
    [Authorize(Roles = "管理基础数据, Administrators")]

    public class IndicatorGroupMapIndicatorsController : Controller
    {
        private ImsDbContext db = new ImsDbContext();

        // GET: IndicatorGroupMapIndicators
        public async Task<ActionResult> Index(int? page)
        {
            int pageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["pagSize"]);
            int pageNumber = (page ?? 1);
            ViewBag.pageSize = pageSize;
            ViewBag.pageNumber = pageNumber;
            var viewModel = new List<IndicatorGroupIndicatorView>();
            var indicatorGroups = await db.IndicatorGroups.OrderBy(i => i.Priority).ToListAsync();

            foreach (var indicatorGroup in indicatorGroups)
            {
                var view = new IndicatorGroupIndicatorView();
                view.Indicators = new List<Indicator>();
                view.IndicatorGroupId = indicatorGroup.IndicatorGroupId;
                view.IndicatorGroupName = indicatorGroup.IndicatorGroupName;
                view.Priority = indicatorGroup.Priority;
                view.Remarks = indicatorGroup.Remarks;
                view.Indicators = await indicatorGroup.IndicatorGroupMapIndicators.Select(i => i.Indicator).OrderBy(i => i.Priority).ToListAsync();
                viewModel.Add(view);
            }
            return View(await viewModel.OrderBy(o => o.Priority).ToPagedListAsync(pageNumber, pageSize));
        }

        // GET: IndicatorGroupMapIndicators/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var viewModel = new IndicatorGroupIndicatorView();
            var indicatorGroup = await db.IndicatorGroups.FindAsync(id.Value);
            if(indicatorGroup == null)
            {
                return HttpNotFound();
            }
            viewModel.IndicatorGroupId = indicatorGroup.IndicatorGroupId;
            viewModel.IndicatorGroupName = indicatorGroup.IndicatorGroupName;
            viewModel.Priority = indicatorGroup.Priority;
            viewModel.Remarks = indicatorGroup.Remarks;
            viewModel.Indicators = new List<Indicator>();
            viewModel.Indicators = await indicatorGroup.IndicatorGroupMapIndicators.Select(i => i.Indicator).OrderBy(i => i.Priority).ToListAsync();

            return View(viewModel);
        }

       

        // GET: IndicatorGroupMapIndicators/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var viewModel = new IndicatorGroupIndicatorView();
            var indicatorGroup = await db.IndicatorGroups.FindAsync(id.Value);
            if (indicatorGroup == null)
            {
                return HttpNotFound();
            }
            viewModel.IndicatorGroupId = indicatorGroup.IndicatorGroupId;
            viewModel.IndicatorGroupName = indicatorGroup.IndicatorGroupName;
            viewModel.Priority = indicatorGroup.Priority;
            viewModel.Remarks = indicatorGroup.Remarks;
            viewModel.Indicators = new List<Indicator>();
            viewModel.Indicators = await indicatorGroup.IndicatorGroupMapIndicators.Select(i => i.Indicator).OrderBy(i => i.Priority).ToListAsync();
            await PopulateAssignedIndicatorData(indicatorGroup);
            return View(viewModel);
        }

        // POST: IndicatorGroupMapIndicators/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(IndicatorGroupIndicatorView model, string[] selectedIndicator)
        {
            var indicatorGroup = await db.IndicatorGroups.FindAsync(model.IndicatorGroupId);
            if (ModelState.IsValid)
            {
                indicatorGroup.Priority = model.Priority;
                indicatorGroup.Remarks = model.Remarks;
                await UpdateUserDepartments(selectedIndicator, indicatorGroup);

                //database win
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
                        // Update the values of the entity that failed to save from the store 
                        ex.Entries.Single().Reload();
                    }
                } while (saveFailed);
            }
            await PopulateAssignedIndicatorData(indicatorGroup);

            return View(model);
        }
        private async Task  PopulateAssignedIndicatorData(IndicatorGroup indicatorGroup)
        {
            var allIndicators = new List<Indicator>();
            allIndicators = await db.Indicators.OrderBy(d => d.Priority).ToListAsync();
            var groupIndicators = new HashSet<Guid>(indicatorGroup.IndicatorGroupMapIndicators.Select(i => i.IndicatorId));
            var viewModel = new List<AssignedIndicatorData>();
            foreach (var indicator in allIndicators)
            {
                viewModel.Add(new AssignedIndicatorData
                {
                    IndicatorId = indicator.IndicatorId,
                    IndicatorName = indicator.IndicatorName,
                    Assigned = groupIndicators.Contains(indicator.IndicatorId)
                });
            }

            ViewBag.GroupIndicators = viewModel;
        }
        private async Task UpdateUserDepartments(string[] selectedIndicator, IndicatorGroup indicatorGroupToUpdate)
        {
            if (selectedIndicator == null)
            {
                //需将IndicatorGroupMapIndicator表中有关Group与Indicator的信息删除
                var indicatorGroupMapIndicators = await db.IndicatorGroupMapIndicators.Where(i => i.IndicatorGroupId == indicatorGroupToUpdate.IndicatorGroupId).ToListAsync();
                foreach(var item in indicatorGroupMapIndicators)
                {
                    db.IndicatorGroupMapIndicators.Remove(item);
                    #region//database win
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
                            // Update the values of the entity that failed to save from the store 
                            ex.Entries.Single().Reload();
                        }
                    } while (saveFailed);
                    #endregion
                }
                return;
            }
            var selectedDepartmentsHS = new HashSet<string>(selectedIndicator);
            var groupIndicators = new HashSet<Guid>
                (indicatorGroupToUpdate.IndicatorGroupMapIndicators.Select(u => u.IndicatorId));
            var allIndicators = await db.Indicators.ToListAsync();
            foreach (var indicator in allIndicators)
            {
                if (selectedDepartmentsHS.Contains(indicator.IndicatorId.ToString()))
                {
                    if (!groupIndicators.Contains(indicator.IndicatorId))
                    {
                        //将IndicatorGroup 与Indicator加入到IndicatorGroupMapIndicator表中
                        IndicatorGroupMapIndicator item = new IndicatorGroupMapIndicator();
                        item.IndicatorGroupMapIndicatorId = System.Guid.NewGuid();
                        item.IndicatorGroupId = indicatorGroupToUpdate.IndicatorGroupId;
                        item.IndicatorId = indicator.IndicatorId;
                        item.Priority = indicatorGroupToUpdate.Priority;
                        db.IndicatorGroupMapIndicators.Add(item);
                        #region //database win
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
                                // Update the values of the entity that failed to save from the store 
                                ex.Entries.Single().Reload();
                            }
                        } while (saveFailed);
                        #endregion
                    }
                }
                else
                {
                    if (groupIndicators.Contains(indicator.IndicatorId))
                    {
                        var item = await db.IndicatorGroupMapIndicators.Where(i => i.IndicatorGroupId == indicatorGroupToUpdate.IndicatorGroupId
                                        && i.IndicatorId == indicator.IndicatorId).FirstAsync();
                        db.IndicatorGroupMapIndicators.Remove(item);
                       
                        #region//database win
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
                                // Update the values of the entity that failed to save from the store 
                                ex.Entries.Single().Reload();
                            }
                        } while (saveFailed);
                        #endregion
                    }
                }
            }
        }
        // GET: IndicatorGroupMapIndicators/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndicatorGroupMapIndicator indicatorGroupMapIndicator = await db.IndicatorGroupMapIndicators.FindAsync(id);
            if (indicatorGroupMapIndicator == null)
            {
                return HttpNotFound();
            }
            return View(indicatorGroupMapIndicator);
        }

        // POST: IndicatorGroupMapIndicators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            IndicatorGroupMapIndicator indicatorGroupMapIndicator = await db.IndicatorGroupMapIndicators.FindAsync(id);
            db.IndicatorGroupMapIndicators.Remove(indicatorGroupMapIndicator);
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
