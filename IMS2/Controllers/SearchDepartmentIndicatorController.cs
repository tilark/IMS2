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
    public class SearchDepartmentIndicatorController : Controller
    {
        private ImsDbContext db = new ImsDbContext();

        // GET: SearchDepartmentIndicator
        [Route("{startTime}/{endTime}/{department}")]
        public async Task<ActionResult> Index(DateTime? startTime, DateTime? endTime, Guid? department)
        {
            ViewBag.department = new SelectList(db.Departments, "DepartmentId", "DepartmentName");
            if(startTime != null && endTime != null && department != null)
            {
               var departmentIndicatorValues = db.DepartmentIndicatorValues.Include(d => d.Department).Include(d => d.DepartmentIndicatorStandard).Include(d => d.Indicator.Duration)
                                    .Where(d => d.DepartmentId == department.Value
                                    && d.Time.Year >= startTime.Value.Year && d.Time.Month >= startTime.Value.Month
                                    && d.Time.Year <= endTime.Value.Year && d.Time.Month <= endTime.Value.Month).OrderBy(d=>d.Indicator.Priority);
                return View(await departmentIndicatorValues.ToListAsync());

            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(IEnumerable<DepartmentIndicatorValueView> departmentIndicatorValue)
        {
            Guid departmentID = new Guid();
            DateTime searchTime = DateTime.Now; ;
            
            foreach(var indicatorValue in departmentIndicatorValue)
            {
                var departmentIndicatorValueLocked = await db.DepartmentIndicatorValues.FindAsync(indicatorValue.DepartmentIndicatorValueId);
                departmentID = departmentIndicatorValueLocked.DepartmentId;
                searchTime = departmentIndicatorValueLocked.Time;
                //如果islocked与之前的相同，则不修改数据中的值
                if (departmentIndicatorValueLocked.IsLocked != indicatorValue.IsLocked)
                {
                    departmentIndicatorValueLocked.IsLocked = indicatorValue.IsLocked;
                    departmentIndicatorValueLocked.UpdateTime = DateTime.Now;
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
            ViewBag.department = new SelectList(db.Departments, "DepartmentId", "DepartmentName");
            return View( await db.DepartmentIndicatorValues.Include(d => d.Department).Include(d => d.DepartmentIndicatorStandard).Include(d => d.Indicator.Duration)
                                    .Where(d => d.DepartmentId == departmentID
                                    && d.Time.Year == searchTime.Year && d.Time.Month == searchTime.Month)
                                    .OrderBy(d => d.Indicator.Priority).ToListAsync());
        }
        private bool IsInRangeTime(DateTime starTime, DateTime midTime, DateTime endTime)
        {
            //midTime如果位于StartTime 与endTime之间，则返回True
            if (midTime.Year >= starTime.Year && midTime.Year <= endTime.Year
                && midTime.Month >= starTime.Month && midTime.Month <= endTime.Month)
            {
                return true;
            }
            else
            {
                return false;
            }


        }
        // GET: SearchDepartmentIndicator/Details/5
        public async Task<ActionResult> Details(Guid? id)
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

        // GET: SearchDepartmentIndicator/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName");
            ViewBag.IndicatorStandardId = new SelectList(db.DepartmentIndicatorStandards, "DepartmentIndicatorStandardId", "Remarks");
            ViewBag.IndicatorId = new SelectList(db.Indicators, "IndicatorId", "IndicatorName");
            return View();
        }

        // POST: SearchDepartmentIndicator/Create
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

        // GET: SearchDepartmentIndicator/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
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
            ViewBag.IndicatorStandardId = new SelectList(db.DepartmentIndicatorStandards.Where(d=>d.DepartmentId == departmentIndicatorValue.DepartmentId && d.IndicatorId == departmentIndicatorValue.IndicatorId),
                                                "DepartmentIndicatorStandardId", "Range", departmentIndicatorValue.IndicatorStandardId);
            return View(departmentIndicatorValue);
        }

        // POST: SearchDepartmentIndicator/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "DepartmentIndicatorValueId,DepartmentId,IndicatorId,Time,Value,IndicatorStandardId,IsLocked,UpdateTime,TimeStamp")] DepartmentIndicatorValue departmentIndicatorValue)
        {
            if (ModelState.IsValid)
            {
                DepartmentIndicatorValue departmentIndicatorValueModify = await db.DepartmentIndicatorValues.FindAsync(departmentIndicatorValue.DepartmentIndicatorValueId);
                if(departmentIndicatorValueModify == null)
                {
                    return HttpNotFound();
                }

                if(departmentIndicatorValueModify.Value != departmentIndicatorValue.Value
                    || departmentIndicatorValueModify.IsLocked != departmentIndicatorValue.IsLocked
                    || departmentIndicatorValueModify.IndicatorStandardId != departmentIndicatorValue.IndicatorStandardId)
                {
                    departmentIndicatorValueModify.Value = departmentIndicatorValue.Value;
                    departmentIndicatorValueModify.IsLocked = departmentIndicatorValue.IsLocked;
                    departmentIndicatorValueModify.IndicatorStandardId = departmentIndicatorValue.IndicatorStandardId;
                    departmentIndicatorValueModify.UpdateTime = DateTime.Now;
                }
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
                ViewBag.IndicatorStandardId = new SelectList(db.DepartmentIndicatorStandards, "DepartmentIndicatorStandardId", "Remarks", departmentIndicatorValue.IndicatorStandardId);
                return View(departmentIndicatorValueModify);
                //return RedirectToAction("Index");
            }
            ViewBag.IndicatorStandardId = new SelectList(db.DepartmentIndicatorStandards, "DepartmentIndicatorStandardId", "Remarks", departmentIndicatorValue.IndicatorStandardId);
            return View(departmentIndicatorValue);
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
