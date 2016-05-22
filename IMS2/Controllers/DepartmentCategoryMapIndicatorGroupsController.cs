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
    public class DepartmentCategoryMapIndicatorGroupsController : Controller
    {
        private ImsDbContext db = new ImsDbContext();

        // GET: DepartmentCategoryMapIndicatorGroups
        public async Task<ActionResult> Index()
        {
            var viewModel = new List<DepartmentCategoryIndicatorGroupView>();
            var departmentCategories = await db.DepartmentCategories.OrderBy(d => d.Priority).ToListAsync();

            foreach(var departmentCategory in departmentCategories)
            {
                var view = new DepartmentCategoryIndicatorGroupView();
                view.IndicatorGroups = new List<IndicatorGroup>();
                view.DepartmentCategoryId = departmentCategory.DepartmentCategoryId;
                view.DepartmentCategoryName = departmentCategory.DepartmentCategoryName;
                view.Priority = departmentCategory.Priority;
                view.Remarks = departmentCategory.Remarks;
                view.IndicatorGroups = departmentCategory.DepartmentCategoryMapIndicatorGroups.Select(d => d.IndicatorGroup).OrderBy(d => d.Priority).ToList();
                viewModel.Add(view);
            }
            return View(viewModel);
        }

        // GET: DepartmentCategoryMapIndicatorGroups/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var viewModel = new DepartmentCategoryIndicatorGroupView();
            var departmentCategory = await db.DepartmentCategories.FindAsync(id.Value);
            if(departmentCategory == null)
            {
                return HttpNotFound();
            }
            viewModel.DepartmentCategoryId = departmentCategory.DepartmentCategoryId;
            viewModel.DepartmentCategoryName = departmentCategory.DepartmentCategoryName;
            viewModel.Priority = departmentCategory.Priority;
            viewModel.Remarks = departmentCategory.Remarks;
            viewModel.IndicatorGroups = new List<IndicatorGroup>();
            viewModel.IndicatorGroups =  departmentCategory.DepartmentCategoryMapIndicatorGroups.Select(d => d.IndicatorGroup).OrderBy(d => d.Priority).ToList();
            return View(viewModel);
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
            var viewModel = new DepartmentCategoryIndicatorGroupView();
            var departmentCategory = await db.DepartmentCategories.FindAsync(id.Value);
            if (departmentCategory == null)
            {
                return HttpNotFound();
            }
            viewModel.DepartmentCategoryId = departmentCategory.DepartmentCategoryId;
            viewModel.DepartmentCategoryName = departmentCategory.DepartmentCategoryName;
            viewModel.Priority = departmentCategory.Priority;
            viewModel.Remarks = departmentCategory.Remarks;
            viewModel.IndicatorGroups = new List<IndicatorGroup>();
            viewModel.IndicatorGroups = departmentCategory.DepartmentCategoryMapIndicatorGroups.Select(d => d.IndicatorGroup).OrderBy(d => d.Priority).ToList();
            await PopulateAssignedIndicatorGroupData(departmentCategory);
            return View(viewModel);
        }

        // POST: DepartmentCategoryMapIndicatorGroups/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit( DepartmentCategoryIndicatorGroupView model, string[] selectedItems)
        {
            var departmentCategory = await db.DepartmentCategories.FindAsync(model.DepartmentCategoryId);
            if (ModelState.IsValid)
            {
                departmentCategory.Priority = model.Priority;
                departmentCategory.Remarks = model.Remarks;
                await UpdateSelectedItems(selectedItems, departmentCategory);
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
            await PopulateAssignedIndicatorGroupData(departmentCategory);
            return View(model);
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
        private async Task PopulateAssignedIndicatorGroupData(DepartmentCategory departmentCategory)
        {
            var allIndicatorGroups = new List<IndicatorGroup>();
            allIndicatorGroups = await db.IndicatorGroups.OrderBy(d => d.Priority).ToListAsync();
            var groupIndicators = new HashSet<Guid>(departmentCategory.DepartmentCategoryMapIndicatorGroups.Select(i => i.IndicatorGroupId));
            var viewModel = new List<AssignedIndicatorGroupData>();
            foreach (var indicatorGroup in allIndicatorGroups)
            {
                viewModel.Add(new AssignedIndicatorGroupData
                {
                    IndicatorGroupId = indicatorGroup.IndicatorGroupId,
                    IndicatorGroupName = indicatorGroup.IndicatorGroupName,
                    Assigned = groupIndicators.Contains(indicatorGroup.IndicatorGroupId)
                });
            }

            ViewBag.IndicatorsGroup = viewModel;
        }
        private async Task UpdateSelectedItems(string[] selectedItems, DepartmentCategory departmentCategoryToUpdate)
        {
            if (selectedItems == null)
            {
                //需将DepartmentCategoryMapIndicatorGroups表中有关IndicatorGroup与DepartmentCategory的信息删除
                var departmentCategoryMapIndicatorGroups = await db.DepartmentCategoryMapIndicatorGroups.Where(i => i.DepartmentCategoryId == departmentCategoryToUpdate.DepartmentCategoryId).ToListAsync();
                foreach (var item in departmentCategoryMapIndicatorGroups)
                {
                    db.DepartmentCategoryMapIndicatorGroups.Remove(item);
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
            var selectedItemsHS = new HashSet<string>(selectedItems);
            var hasItems = new HashSet<Guid>
                (departmentCategoryToUpdate.DepartmentCategoryMapIndicatorGroups.Select(u => u.IndicatorGroupId));
            var allItems = await db.IndicatorGroups.ToListAsync();
            foreach (var item in allItems)
            {
                if (selectedItemsHS.Contains(item.IndicatorGroupId.ToString()))
                {
                    if (!hasItems.Contains(item.IndicatorGroupId))
                    {
                        //将IndicatorGroup 与Indicator加入到IndicatorGroupMapIndicator表中
                        DepartmentCategoryMapIndicatorGroup newItem = new DepartmentCategoryMapIndicatorGroup();
                        newItem.DepartmentCategoryMapIndicatorGroupId = System.Guid.NewGuid();
                        newItem.DepartmentCategoryId = departmentCategoryToUpdate.DepartmentCategoryId;
                        newItem.IndicatorGroupId = item.IndicatorGroupId;
                        newItem.Priority = departmentCategoryToUpdate.Priority;
                        db.DepartmentCategoryMapIndicatorGroups.Add(newItem);
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
                    if (hasItems.Contains(item.IndicatorGroupId))
                    {
                        var deleItem = await db.DepartmentCategoryMapIndicatorGroups.Where(i => i.DepartmentCategoryId == departmentCategoryToUpdate.DepartmentCategoryId
                                        && i.IndicatorGroupId == item.IndicatorGroupId).FirstAsync();
                        db.DepartmentCategoryMapIndicatorGroups.Remove(deleItem);

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
