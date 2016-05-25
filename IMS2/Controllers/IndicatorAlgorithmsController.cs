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
namespace IMS2.Controllers
{
    [Authorize(Roles = "管理基础数据, Administrators")]

    public class IndicatorAlgorithmsController : Controller
    {
        private ImsDbContext db = new ImsDbContext();

        // GET: IndicatorAlgorithms
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
            return View(await db.IndicatorAlgorithms.ToListAsync());
        }

        // GET: IndicatorAlgorithms/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndicatorAlgorithm indicatorAlgorithm = await db.IndicatorAlgorithms.FindAsync(id);
            if (indicatorAlgorithm == null)
            {
                return HttpNotFound();
            }
            IndicatorAlgorithmsView viewModel = new IndicatorAlgorithmsView();

            var result = await db.Indicators.FindAsync(indicatorAlgorithm.ResultId);
            var firstOperand = await db.Indicators.FindAsync(indicatorAlgorithm.FirstOperandID);
            var secondOperand = await db.Indicators.FindAsync(indicatorAlgorithm.SecondOperandID);
            
            if(result != null && firstOperand != null && secondOperand != null)
            {
                viewModel = new IndicatorAlgorithmsView
                {
                    IndicatorAlgorithmsId = indicatorAlgorithm.IndicatorAlgorithmsId,
                    Result = result.IndicatorName,
                    FirstOperand = firstOperand.IndicatorName,
                    SecondOperand = secondOperand.IndicatorName,
                    OperationMethod = (OperationMethod)Enum.Parse(typeof(OperationMethod), indicatorAlgorithm.OperationMethod),
                    Remarks = indicatorAlgorithm.Remarks,
                };

            }
            return View(viewModel);
        }

        // GET: IndicatorAlgorithms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: IndicatorAlgorithms/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IndicatorAlgorithmsView model)
        {
            if (ModelState.IsValid)
            {
                var resultID = await db.Indicators.Where(i => i.IndicatorName == model.Result).FirstOrDefaultAsync();
                if(resultID != null)
                {
                    //查重，看是否存在相同项，利用Result
                    var query = await db.IndicatorAlgorithms.Where(i => i.ResultId == resultID.IndicatorId).FirstOrDefaultAsync();
                    if (query == null)
                    {

                        var firstOperandID = await db.Indicators.Where(i => i.IndicatorName == model.FirstOperand).FirstOrDefaultAsync();
                        var secondOperandID = await db.Indicators.Where(i => i.IndicatorName == model.SecondOperand).FirstOrDefaultAsync();
                        if (firstOperandID != null && secondOperandID != null)
                        {
                            IndicatorAlgorithm item = new IndicatorAlgorithm();
                            item.IndicatorAlgorithmsId = Guid.NewGuid();
                            item.ResultId = resultID.IndicatorId;
                            item.FirstOperandID = firstOperandID.IndicatorId;
                            item.SecondOperandID = secondOperandID.IndicatorId;
                            item.OperationMethod = model.OperationMethod.ToString();
                            item.Remarks = model.Result;
                            db.IndicatorAlgorithms.Add(item);
                            await db.SaveChangesAsync();
                            return RedirectToAction("Index", new { message = IMSMessageIdEnum.CreateSuccess });
                        }
                    }
               
                }
                return RedirectToAction("Index", new { message = IMSMessageIdEnum.CreateError });
            }
            return View(model);

        }


        // GET: IndicatorAlgorithms/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndicatorAlgorithm indicatorAlgorithm = await db.IndicatorAlgorithms.FindAsync(id);
            if (indicatorAlgorithm == null)
            {
                return HttpNotFound();
            }
            return View(indicatorAlgorithm);
        }

        // POST: IndicatorAlgorithms/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IndicatorAlgorithmsId,ResultId,FirstOperandID,SecondOperandID,OperationMethod,Remarks")] IndicatorAlgorithm indicatorAlgorithm)
        {
            if (ModelState.IsValid)
            {
                db.Entry(indicatorAlgorithm).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(indicatorAlgorithm);
        }

        // GET: IndicatorAlgorithms/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IndicatorAlgorithm indicatorAlgorithm = await db.IndicatorAlgorithms.FindAsync(id);
            if (indicatorAlgorithm == null)
            {
                return HttpNotFound();
            }
            return View(indicatorAlgorithm);
        }

        // POST: IndicatorAlgorithms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            IndicatorAlgorithm indicatorAlgorithm = await db.IndicatorAlgorithms.FindAsync(id);
            db.IndicatorAlgorithms.Remove(indicatorAlgorithm);
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
