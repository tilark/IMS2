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
    [Authorize(Roles = "修改科室指标值, Administrators, 修改全院指标值,")]

    public class DepartmentIndicatorValuesController : Controller
    {
        private ImsDbContext db = new ImsDbContext();

        // GET: DepartmentIndicatorValues
        [Route("Index/{searchTime}/{departmentCategory}")]
        public ActionResult Index(DateTime? searchTime, Guid? departmentCategory)
        {
            //填充科室类别下拉框，如果选中的时间不为空，显示科室列表，并能提示有多少项指标

            //var departmentIndicatorValues = db.DepartmentIndicatorValues.Include(d => d.Department).Include(d => d.DepartmentIndicatorStandard).Include(d => d.Indicator);

            ViewBag.departmentCategory = new SelectList(db.DepartmentCategories, "DepartmentCategoryId", "DepartmentCategoryName");
            if (searchTime != null && departmentCategory != null)
            {
                List<DepartmentIndicatorCountView> viewModel = new List<DepartmentIndicatorCountView>();
                var category = db.DepartmentCategories.Include(d => d.Departments).Include(d => d.DepartmentCategoryMapIndicatorGroups).Where(d => d.DepartmentCategoryId == departmentCategory).FirstOrDefault();
                if (category != null)
                {
                    ViewBag.departmentCategoryID = category.DepartmentCategoryId;
                    foreach (var department in category.Departments)
                    {

                        DepartmentIndicatorCountView view = new DepartmentIndicatorCountView();
                        view.Department = department;
                        int count = db.DepartmentIndicatorValues.Where(d => d.DepartmentId == department.DepartmentId && d.Time.Year == searchTime.Value.Year
                                                                    && d.Time.Month == searchTime.Value.Month).Count();
                        //如果发现为0的话，就自动创建该月份的项
                        if (count == 0)
                        {
                            CreateDepartmentIndicatorList(searchTime, department, out count);
                        }
                        view.IndicatorCount = count;
                        view.SearchTime = searchTime;
                        viewModel.Add(view);
                    }

                    return View(viewModel);
                }
            }
            return View();
        }

        private void CreateDepartmentIndicatorList(DateTime? searchTime, Department department, out int count)
        {
            count = 0;
            if (department == null || searchTime == null)
            {
                return;
            }
            var indicatorCollection = department.DepartmentCategory.DepartmentCategoryMapIndicatorGroups
                           .SelectMany(c => c.IndicatorGroup.IndicatorGroupMapIndicators).Select(c => c.Indicator).OrderBy(d=>d.Priority).Distinct();
            foreach (var indicator in indicatorCollection)
            {
                //根据时段的名称来判断是否添加到该月，如季不会出现在1、2月份中，半年不会出现在1、2、3、4、5月中，全年只会在12月时出现。
                bool canAdd = false;
                switch (indicator.Duration.DurationName)
                {
                    case "季":
                        if (searchTime.Value.Month == 3 || searchTime.Value.Month == 6 || searchTime.Value.Month == 9 || searchTime.Value.Month == 12)
                        {
                            canAdd = true;
                        }
                        break;
                    case "半年":
                        if(searchTime.Value.Month == 6)
                        {
                            canAdd = true;
                        }
                        break;
                    case "全年":
                        if(searchTime.Value.Month == 12)
                        {
                            canAdd = true;
                        }
                        break;
                    default:
                        canAdd = true;
                        break;
                }
                if (canAdd)
                {
                    count++;
                    DepartmentIndicatorValue departmentIndicatorValue = new DepartmentIndicatorValue();
                    departmentIndicatorValue.DepartmentId = department.DepartmentId;
                    departmentIndicatorValue.IndicatorId = indicator.IndicatorId;
                    departmentIndicatorValue.DepartmentIndicatorValueId = System.Guid.NewGuid();
                    departmentIndicatorValue.Time = searchTime.Value;
                    //需找到最新的版本号
                    var standardValue = db.DepartmentIndicatorStandards.Where(d => d.DepartmentId == department.DepartmentId && d.IndicatorId == indicator.IndicatorId
                            && d.Version == db.DepartmentIndicatorStandards.Where(i => i.DepartmentId == department.DepartmentId && i.IndicatorId == indicator.IndicatorId).Max(v => v.Version))
                            .FirstOrDefault();
                    departmentIndicatorValue.IndicatorStandardId = standardValue?.DepartmentIndicatorStandardId;
                    departmentIndicatorValue.IsLocked = false;
                    departmentIndicatorValue.UpdateTime = DateTime.Now;
                    //将科室、项目、时间添加到科室值表中
                    AddDepartmentIndicatorValue(departmentIndicatorValue);
                }
                
            }
        }

        private DepartmentIndicatorValue AddDepartmentIndicatorValue(DepartmentIndicatorValue departmentIndicatorValue)
        {
            DepartmentIndicatorValue item = null;
            if (departmentIndicatorValue == null)
            {
                return null;
            }
            //查重
            item = db.DepartmentIndicatorValues.Where(d => d.DepartmentId == departmentIndicatorValue.DepartmentId
                            && d.IndicatorId == departmentIndicatorValue.IndicatorId
                            && d.Time.Year == departmentIndicatorValue.Time.Year && d.Time.Month == departmentIndicatorValue.Time.Month)
                            .FirstOrDefault();
            if (item == null)
            {
                item = departmentIndicatorValue;
                db.DepartmentIndicatorValues.Add(item);
                db.SaveChanges();
            }
            return item;

        }


        // GET: DepartmentIndicatorValues/Details/5
        [Route("Details/{id}/{time}")]
        public async Task<ActionResult> Details(Guid? id, DateTime? time)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //查找到该department       
            var department = await db.Departments.FindAsync(id.Value);
            DepartmentIndicatorCountView viewModel = new DepartmentIndicatorCountView();
            viewModel.DepartmentIndicatorValues = new List<DepartmentIndicatorValue>();
            viewModel.Department = department;
            //从DepartmentIndicatorValue找值
            viewModel.SearchTime = time;
            var departmentIndicatorValues = await db.Departments.SelectMany(c => c.DepartmentIndicatorValues).Include(d => d.Indicator.Duration)
                                                .Where(d => d.DepartmentId == department.DepartmentId && d.Time.Year == time.Value.Year && d.Time.Month == time.Value.Month).ToListAsync();
            foreach (var departmentIndicatorValue in departmentIndicatorValues)
            {
                viewModel.DepartmentIndicatorValues.Add(departmentIndicatorValue);
            }
            return View(viewModel);
        }

        // GET: DepartmentIndicatorValues/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName");
            ViewBag.IndicatorStandardId = new SelectList(db.DepartmentIndicatorStandards, "DepartmentIndicatorStandardId", "Remarks");
            ViewBag.IndicatorId = new SelectList(db.Indicators, "IndicatorId", "IndicatorName");
            return View();
        }
        //传入的为科室
        //public ActionResult Create(Guid? id)
        //{

        //}
        // POST: DepartmentIndicatorValues/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DepartmentIndicatorValueId,DepartmentId,IndicatorId,Time,Value,IndicatorStandardId,IsLocked,UpdateTime,TimeStamp")] DepartmentIndicatorValue departmentIndicatorValue)
        {
            if (ModelState.IsValid)
            {
                departmentIndicatorValue.DepartmentIndicatorValueId = Guid.NewGuid();
                db.DepartmentIndicatorValues.Add(departmentIndicatorValue);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", departmentIndicatorValue.DepartmentId);
            ViewBag.IndicatorStandardId = new SelectList(db.DepartmentIndicatorStandards, "DepartmentIndicatorStandardId", "Remarks", departmentIndicatorValue.IndicatorStandardId);
            ViewBag.IndicatorId = new SelectList(db.Indicators, "IndicatorId", "IndicatorName", departmentIndicatorValue.IndicatorId);
            return View(departmentIndicatorValue);
        }

        // GET: DepartmentIndicatorValues/Edit/5
        //public async Task<ActionResult> Edit(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    DepartmentIndicatorValue departmentIndicatorValue = await db.DepartmentIndicatorValues.FindAsync(id);
        //    if (departmentIndicatorValue == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", departmentIndicatorValue.DepartmentId);
        //    ViewBag.IndicatorStandardId = new SelectList(db.DepartmentIndicatorStandards, "DepartmentIndicatorStandardId", "Remarks", departmentIndicatorValue.IndicatorStandardId);
        //    ViewBag.IndicatorId = new SelectList(db.Indicators, "IndicatorId", "IndicatorName", departmentIndicatorValue.IndicatorId);
        //    return View(departmentIndicatorValue);
        //}
        [Route("Details/{id}/{time}")]
        public async Task<ActionResult> Edit(Guid? id, DateTime? time)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //查找到该department       
            var department = await db.Departments.FindAsync(id.Value);
            DepartmentIndicatorCountView viewModel = new DepartmentIndicatorCountView();
            viewModel.DepartmentIndicatorValues = new List<DepartmentIndicatorValue>();
            viewModel.Department = department;
            //从DepartmentIndicatorValue找值
            viewModel.SearchTime = time;
            var departmentIndicatorValues = await db.Departments.SelectMany(c => c.DepartmentIndicatorValues).Include(d => d.Indicator)
                                                .Where(d => d.DepartmentId == department.DepartmentId && d.Time.Year == time.Value.Year && d.Time.Month == time.Value.Month).ToArrayAsync();
            foreach (var departmentIndicatorValue in departmentIndicatorValues)
            {
                viewModel.DepartmentIndicatorValues.Add(departmentIndicatorValue);
            }
            return View(viewModel);
        }
        // POST: DepartmentIndicatorValues/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "DepartmentIndicatorValueId,DepartmentId,IndicatorId,Time,Value,IndicatorStandardId,IsLocked,UpdateTime,TimeStamp")] DepartmentIndicatorValue departmentIndicatorValue)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(departmentIndicatorValue).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", departmentIndicatorValue.DepartmentId);
        //    ViewBag.IndicatorStandardId = new SelectList(db.DepartmentIndicatorStandards, "DepartmentIndicatorStandardId", "Remarks", departmentIndicatorValue.IndicatorStandardId);
        //    ViewBag.IndicatorId = new SelectList(db.Indicators, "IndicatorId", "IndicatorName", departmentIndicatorValue.IndicatorId);
        //    return View(departmentIndicatorValue);
        //}
        public async Task<ActionResult> Edit(Department department, IEnumerable<DepartmentIndicatorValue> departmentIndicatorValues, DateTime? searchTime)
        {
            var viewModel = new DepartmentIndicatorCountView();
            if (department != null)
            {
                viewModel.Department = await db.Departments.FindAsync(department.DepartmentId);
            }
            viewModel.SearchTime = searchTime;
            viewModel.DepartmentIndicatorValues = new List<DepartmentIndicatorValue>();
            foreach (var departmentIdicatorValuequery in departmentIndicatorValues)
            {
                //保存值
                var departmentIdicatorValue = await db.DepartmentIndicatorValues
                                             .FindAsync(departmentIdicatorValuequery.DepartmentIndicatorValueId);
                //if (TryUpdateModel(departmentIdicatorValue, "", new string[] { "Value" }))
                //{
                try
                {
                    if (departmentIdicatorValuequery.Value != null &&
                        departmentIdicatorValue.Value != departmentIdicatorValuequery.Value)
                    {
                        departmentIdicatorValue.Value = departmentIdicatorValuequery.Value;
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
                }
                catch (Exception)
                {

                    ModelState.AddModelError("", String.Format("无法更新指标{0}的值！", departmentIdicatorValue.Indicator.IndicatorName));
                }
                //}
                viewModel.DepartmentIndicatorValues.Add(departmentIdicatorValue);
            }
            return View(viewModel);
        }

        private void UpdateDepartmentIndicatorValue(decimal? value, DepartmentIndicatorValue departmentIdicatorValue)
        {
            if (value == null)
            {
                return;
            }
            //var item = db.DepartmentIndicatorValues.FindAsync
        }

        // GET: DepartmentIndicatorValues/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DepartmentIndicatorValue departmentIndicatorValue = await db.DepartmentIndicatorValues.FindAsync(id);
            if (departmentIndicatorValue == null)
            {
                return HttpNotFound();
            }
            return View(departmentIndicatorValue);
        }

        // POST: DepartmentIndicatorValues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            DepartmentIndicatorValue departmentIndicatorValue = await db.DepartmentIndicatorValues.FindAsync(id);
            db.DepartmentIndicatorValues.Remove(departmentIndicatorValue);
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
