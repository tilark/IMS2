using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IMS2.Models;
using IMS2.DAL;
namespace IMS2.Controllers
{
    public class DataSourceSystemBaseController : Controller
    {
        //private ImsDbContext db = new ImsDbContext();
        private UnitOfWork unitOfWork = null;
        public DataSourceSystemBaseController()
            : this(new UnitOfWork())
        {

        }
        public DataSourceSystemBaseController(UnitOfWork uow)
        {
            this.unitOfWork = uow;
        }
        // GET: DataSourceSystemBase
        public ActionResult Index()
        {
            return View(unitOfWork.DataSourceSystemRepository.GetAllDataSourceSystem());
        }

        // GET: DataSourceSystemBase/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id.Equals(Guid.Empty))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSourceSystem dataSourceSystem = unitOfWork.DataSourceSystemRepository.GetDataSourceSystemById(id.Value);
            if (dataSourceSystem == null)
            {
                return HttpNotFound();
            }
            return View(dataSourceSystem);
        }

        // GET: DataSourceSystemBase/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DataSourceSystemBase/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DataSourceSystemId,DataSourceSystemName,Priority,Remarks,TimeStamp")] DataSourceSystem dataSourceSystem)
        {
            if (ModelState.IsValid)
            {
                dataSourceSystem.DataSourceSystemId = Guid.NewGuid();
                unitOfWork.DataSourceSystemRepository.AddDataSourceSystem(dataSourceSystem);
                unitOfWork.DataSourceSystemRepository.Save();
                //db.DataSourceSystems.Add(dataSourceSystem);
                //db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dataSourceSystem);
        }

        // GET: DataSourceSystemBase/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id.Equals(Guid.Empty))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSourceSystem dataSourceSystem = unitOfWork.DataSourceSystemRepository.GetDataSourceSystemById(id.Value);
            if (dataSourceSystem == null)
            {
                return HttpNotFound();
            }
            return View(dataSourceSystem);
        }

        // POST: DataSourceSystemBase/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DataSourceSystemId,DataSourceSystemName,Priority,Remarks,TimeStamp")] DataSourceSystem dataSourceSystem)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.DataSourceSystemRepository.UpdateDataSourceSystem(dataSourceSystem);
                unitOfWork.DataSourceSystemRepository.Save();
                //db.Entry(dataSourceSystem).State = EntityState.Modified;
                //db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dataSourceSystem);
        }

        // GET: DataSourceSystemBase/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id.Equals(Guid.Empty))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSourceSystem dataSourceSystem = unitOfWork.DataSourceSystemRepository.GetDataSourceSystemById(id.Value);
            if (dataSourceSystem == null)
            {
                return HttpNotFound();
            }
            return View(dataSourceSystem);
        }

        // POST: DataSourceSystemBase/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            DataSourceSystem dataSourceSystem = unitOfWork.DataSourceSystemRepository.GetDataSourceSystemById(id);
            unitOfWork.DataSourceSystemRepository.DeleteDataSourceSystem(dataSourceSystem);
            unitOfWork.DataSourceSystemRepository.Save();
                
            //db.DataSourceSystems.Remove(dataSourceSystem);
            //db.SaveChanges();
            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
