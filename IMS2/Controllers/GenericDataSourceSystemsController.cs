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
    public class GenericDataSourceSystemsController : Controller
    {
        //private ImsDbContext db = new ImsDbContext();
        private GenericUnitOfWork uow = null;
        public GenericDataSourceSystemsController()
            :this(new GenericUnitOfWork())
        {
        }
        public GenericDataSourceSystemsController(GenericUnitOfWork _uow)
        {
            this.uow = _uow;
        }
        // GET: GenericDataSourceSystems
        public ActionResult Index()
        {
            return View(uow.Repository<DataSourceSystem>().GetAll().OrderBy(d => d.Priority).ToList());
        }

        // GET: GenericDataSourceSystems/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id.Equals(Guid.Empty))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSourceSystem dataSourceSystem = uow.Repository<DataSourceSystem>()
                .Get(c => c.DataSourceSystemId == id.Value);
            if (dataSourceSystem == null)
            {
                return HttpNotFound();
            }
            return View(dataSourceSystem);
        }

        // GET: GenericDataSourceSystems/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GenericDataSourceSystems/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DataSourceSystemId,DataSourceSystemName,Priority,Remarks,TimeStamp")] DataSourceSystem dataSourceSystem)
        {
            if (ModelState.IsValid)
            {
                //查重
                var query = uow.Repository<DataSourceSystem>().Get(d => d.DataSourceSystemName == dataSourceSystem.DataSourceSystemName);
                if(query == null)
                {
                    dataSourceSystem.DataSourceSystemId = Guid.NewGuid();
                    uow.Repository<DataSourceSystem>().Add(dataSourceSystem);
                    uow.SaveChanges();
                    return RedirectToAction("Index");
                }
                
            }

            return View(dataSourceSystem);
        }

        // GET: GenericDataSourceSystems/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id.Equals(Guid.Empty))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSourceSystem dataSourceSystem = uow.Repository<DataSourceSystem>()
                .Get(d => d.DataSourceSystemId == id.Value);
            if (dataSourceSystem == null)
            {
                return HttpNotFound();
            }
            return View(dataSourceSystem);
        }

        // POST: GenericDataSourceSystems/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DataSourceSystemId,DataSourceSystemName,Priority,Remarks,TimeStamp")] DataSourceSystem dataSourceSystem)
        {
            if (ModelState.IsValid)
            {
                uow.Repository<DataSourceSystem>().Update(dataSourceSystem);
                uow.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dataSourceSystem);
        }

        // GET: GenericDataSourceSystems/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id.Equals(Guid.Empty))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSourceSystem dataSourceSystem = uow.Repository<DataSourceSystem>()
                .Get(c => c.DataSourceSystemId == id.Value);
            if (dataSourceSystem == null)
            {
                return HttpNotFound();
            }
            return View(dataSourceSystem);
        }

        // POST: GenericDataSourceSystems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            DataSourceSystem dataSourceSystem = uow.Repository<DataSourceSystem>()
                .Get(d => d.DataSourceSystemId == id);
            uow.Repository<DataSourceSystem>().Delete(dataSourceSystem);
            uow.SaveChanges();
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
