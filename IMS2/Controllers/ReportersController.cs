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
    [Authorize]
    public class ReportersController : Controller
    {
        private ImsDbContext db = new ImsDbContext();

        // GET: Reporters
        public async Task<ActionResult> Index()
        {

            await InitialAssignedDepartmentCategoryData();
            await InitialAssignedIndicatorGroupData();
            return View();
        }

        public ActionResult TestDateTimePicker()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(DateTime? startTime, DateTime? endTime, string[] selectedDepartmentCategory, string[] selectedIndicatorGroup)
        {
            if (selectedDepartmentCategory == null)
            {
                await InitialAssignedDepartmentCategoryData();
                await InitialAssignedIndicatorGroupData();
                return View();
            }
            else if (startTime != null && endTime != null && selectedDepartmentCategory != null && selectedIndicatorGroup != null)
            {
                if (endTime.Value.Year < startTime.Value.Year || (endTime.Value.Year == startTime.Value.Year && endTime.Value.Month < startTime.Value.Month))
                {
                    ViewBag.Status = "截止时间需大于等于开始时间！";
                }
                else
                {
                    Report(startTime, endTime, selectedDepartmentCategory, selectedIndicatorGroup);
                }
            }
            await PopulateAssignedDepartmentCategoryData(selectedDepartmentCategory);
            await PopulateAssignedIndicatorGroupData(selectedDepartmentCategory);
            return View();

        }
        public async Task<ActionResult> IndicatorGroupPartial(DateTime? startTime, DateTime? endTime, string[] selectedDepartmentCategory)
        {

            await PopulateAssignedIndicatorGroupData(selectedDepartmentCategory);
            //return PartialView("IndicatorGroupPartial");
            return PartialView("_IndicatorGroupPartial");
        }
        /// <summary>
        /// Reports the specified start time.
        /// </summary>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="selectedDepartmentCategory">The selected department category.</param>
        /// <param name="selectedIndicatorGroup">The selected indicator group.</param>
        public ActionResult Report(DateTime? startTime, DateTime? endTime, string[] selectedDepartmentCategory, string[] selectedIndicatorGroup)
        {
            var report = new Report(selectedDepartmentCategory.Select(c => new Guid(c)).ToList(), selectedIndicatorGroup.Select(c => new Guid(c)).ToList(), startTime.Value, endTime.Value);
            report.GetData();
            return View("ReportView", report);
        }

        #region AssignedDepartmentCategory
        private async Task InitialAssignedDepartmentCategoryData()
        {
            var allDepartmentCategories = await db.DepartmentCategories.OrderBy(d => d.Priority).ToListAsync();
            var viewModel = new List<AssignedDepartmentCategoryData>();

            foreach (var departmentCategory in allDepartmentCategories)
            {
                viewModel.Add(new AssignedDepartmentCategoryData
                {
                    DepartmentCategoryID = departmentCategory.DepartmentCategoryId,
                    DepartmentCategoryName = departmentCategory.DepartmentCategoryName,
                    Assigned = false
                });
            }
            ViewBag.DepartmentCategories = viewModel;
        }
        private async Task PopulateAssignedDepartmentCategoryData(string[] selectedDepartmentCategory)
        {
            var color = BackGroundColorEnum.BurlyWood;
            var allDepartmentCategories = await db.DepartmentCategories.OrderBy(d => d.Priority).ToListAsync();
            var viewModel = new List<AssignedDepartmentCategoryData>();
            var selectedCategory = new HashSet<Guid>();
            foreach (var category in selectedDepartmentCategory)
            {
                selectedCategory.Add(Guid.Parse(category));
            }
            foreach (var departmentCategory in allDepartmentCategories)
            {
                viewModel.Add(new AssignedDepartmentCategoryData
                {
                    DepartmentCategoryID = departmentCategory.DepartmentCategoryId,
                    DepartmentCategoryName = departmentCategory.DepartmentCategoryName,
                    Assigned = selectedCategory.Contains(departmentCategory.DepartmentCategoryId),
                    Color = color.ToString()
                });
                if ((int)color++ >= 14)
                    color = BackGroundColorEnum.BurlyWood;
            }
            ViewBag.DepartmentCategories = viewModel;
        }
        #endregion
        #region AssignedIndicatorGroup
        private async Task InitialAssignedIndicatorGroupData()
        {
            var allIndicatorGroup = await db.IndicatorGroups.OrderBy(i => i.Priority).ToListAsync();
            var viewModel = new List<AssignedIndicatorGroupData>();

            foreach (var indicatorGroup in allIndicatorGroup)
            {
                viewModel.Add(new AssignedIndicatorGroupData
                {
                    IndicatorGroupId = indicatorGroup.IndicatorGroupId,
                    IndicatorGroupName = indicatorGroup.IndicatorGroupName,
                    Assigned = false
                });
            }
            ViewBag.IndicatorsGroup = viewModel;

        }

        private async Task PopulateAssignedIndicatorGroupData(string[] selectedDepartmentCategory)
        {
            if (selectedDepartmentCategory == null)
            {
                return;
            }
            var allIndicatorGroups = new List<IndicatorGroup>();
            allIndicatorGroups = await db.IndicatorGroups.OrderBy(d => d.Priority).ToListAsync();
            var viewModel = new List<AssignedIndicatorGroupData>();
            foreach (var selectedCategory in selectedDepartmentCategory)
            {
                //找到相关的科室类别
                DepartmentCategory departmentCategory = await db.DepartmentCategories.FindAsync(Guid.Parse(selectedCategory));
                var groupIndicators = new HashSet<Guid>(departmentCategory.DepartmentCategoryMapIndicatorGroups.Select(i => i.IndicatorGroupId));

                foreach (var indicatorGroup in allIndicatorGroups)
                {
                    var view = new AssignedIndicatorGroupData
                    {
                        IndicatorGroupId = indicatorGroup.IndicatorGroupId,
                        IndicatorGroupName = indicatorGroup.IndicatorGroupName,
                        Assigned = groupIndicators.Contains(indicatorGroup.IndicatorGroupId)
                    };
                    //得查重，不能重复出现
                    var query = viewModel.Find(i => i.IndicatorGroupId == view.IndicatorGroupId);
                    if (query == null)
                    {
                        viewModel.Add(view);
                    }
                    else
                    {
                        query.Assigned |= view.Assigned;
                    }
                }
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
        #endregion
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
