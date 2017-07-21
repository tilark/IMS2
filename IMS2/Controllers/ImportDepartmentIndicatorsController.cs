using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Text;
using ExcelEntityOperation;
using IMS2.PublicOperations;
using IMS2.RepositoryAsync;
using IMS2.ViewModels.ImportDepartmentIndicatorViews;
using IMS2.ViewModels.UploadFileViews;
using System.ComponentModel.DataAnnotations;
using IMS2.Models;
using IMS2.PublicOperations.Interfaces;

namespace IMS2.Controllers
{
    public class ImportDepartmentIndicatorsController : Controller
    {
        private IReadFromExcel readFromExcel;
        private IDomainUnitOfWork unitOfWork;
        private IFileUpload fileUpload;
        private DepartmentIndicatorValueRepositoryAsync repo;
        private string TempFolder = System.Configuration.ConfigurationManager.AppSettings["UpLoadFileDirectory"].ToString() ?? "/UploadFiles";
        public ImportDepartmentIndicatorsController(IReadFromExcel readFromExcel, IDomainUnitOfWork unitOfWork, IFileUpload fileUpload)
        {
            this.readFromExcel = readFromExcel;
            this.unitOfWork = unitOfWork;
            this.fileUpload = fileUpload;
            this.repo = new DepartmentIndicatorValueRepositoryAsync(this.unitOfWork);
            //this.TempFolder = System.Configuration.ConfigurationManager.AppSettings["UpLoadFileDirectory"].ToString() ?? "/UploadFiles";

        }
        // GET: ImportDepartmentIndicators
        public ActionResult Index()
        {
            return View();
        }

        #region 病案统计室数据
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportermonth"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]      
        public ActionResult _MedicalRecordStatistical(UploadFileView uploadFileView, IEnumerable<HttpPostedFileBase> files)
        {
            
            if (!IsMatchedFileFormat(files))
            {
                return Json(new { success = false, errorMessage = "File is of wrong format." });
            }
            var file = files.FirstOrDefault();  // get ONE only

            var serverPath = HttpContext.Server.MapPath(TempFolder);

            var filePath = fileUpload.GetTempSavedFilePath(file, serverPath);

            //开始解析文件操作  
            filePath = filePath.Replace("\\", "/");
            filePath = HttpContext.Server.MapPath(filePath);

            //uploadFileView.StandardrizeReporterDate();          
           
            try
            {
                var viewModel = new FromExcelToDataBase<MedicalRecordStatisticalFromExcel>(uploadFileView.ReporterDate, this.unitOfWork, this.readFromExcel).WriteDataToDataBase(filePath, "", "B2", 1, 1);

                return PartialView("_ShowResultReadFromExcel", viewModel);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, errorMessage = ex.Message });
            }
        }



        #endregion

        #region 病理科
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _PathologyStatistical(UploadFileView uploadFileView, IEnumerable<HttpPostedFileBase> files)
        {
            var filePath = GetFilePathOrDefault(files);
            if(filePath == null)
            {
                 return Json(new { success = false, errorMessage = "File is of wrong format." });
            }

            try
            {
                var viewModel = new FromExcelToDataBase<PathologyFromExcel>(uploadFileView.ReporterDate, this.unitOfWork, this.readFromExcel).WriteDataToDataBase(filePath, "", "B2", 1, 1);

                return PartialView("_ShowResultReadFromExcel", viewModel);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, errorMessage = ex.Message });
            }
        }
        #endregion

        #region 财务部
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _FinanceStatistical(UploadFileView uploadFileView, IEnumerable<HttpPostedFileBase> files)
        {
            var filePath = GetFilePathOrDefault(files);
            if (filePath == null)
            {
                return Json(new { success = false, errorMessage = "File is of wrong format." });
            }

            try
            {
                var viewModel = new FromExcelToDataBase<FinanceFromExcel>(uploadFileView.ReporterDate, this.unitOfWork, this.readFromExcel).WriteDataToDataBase(filePath, "", "B2", 1, 1);

                return PartialView("_ShowResultReadFromExcel", viewModel);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, errorMessage = ex.Message });
            }
        }
        #endregion

        #region 超声科
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _UltrasoundStatistical(UploadFileView uploadFileView, IEnumerable<HttpPostedFileBase> files)
        {
            var filePath = GetFilePathOrDefault(files);
            if (filePath == null)
            {
                return Json(new { success = false, errorMessage = "File is of wrong format." });
            }

            try
            {
                var viewModel = new FromExcelToDataBase<UltrasoundFromExcel>(uploadFileView.ReporterDate, this.unitOfWork, this.readFromExcel).WriteDataToDataBase(filePath, "", "B2", 1, 1);

                return PartialView("_ShowResultReadFromExcel", viewModel);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, errorMessage = ex.Message });
            }
        }
        #endregion

        #region 放射科
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _RadiologyStatistical(UploadFileView uploadFileView, IEnumerable<HttpPostedFileBase> files)
        {
            var filePath = GetFilePathOrDefault(files);
            if (filePath == null)
            {
                return Json(new { success = false, errorMessage = "File is of wrong format." });
            }


            try
            {
                var viewModel = new FromExcelToDataBase<RadiologyFromExcel>(uploadFileView.ReporterDate, this.unitOfWork, this.readFromExcel).WriteDataToDataBase(filePath, "", "B2", 1, 1);

                return PartialView("_ShowResultReadFromExcel", viewModel);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, errorMessage = ex.Message });
            }
        }
        #endregion

        #region 感染管理部门
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _InfectionManagementStatistical(UploadFileView uploadFileView, IEnumerable<HttpPostedFileBase> files)
        {
            var filePath = GetFilePathOrDefault(files);
            if (filePath == null)
            {
                return Json(new { success = false, errorMessage = "File is of wrong format." });
            }

            try
            {
                var viewModel = new FromExcelToDataBase<InfectionManagementFromExcel>(uploadFileView.ReporterDate, this.unitOfWork, this.readFromExcel).WriteDataToDataBase(filePath, "", "B2", 1, 1);

                return PartialView("_ShowResultReadFromExcel", viewModel);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, errorMessage = ex.Message });
            }
        }
        #endregion


        #region 检验科
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _ExamineStatistical(UploadFileView uploadFileView, IEnumerable<HttpPostedFileBase> files)
        {
            var filePath = GetFilePathOrDefault(files);
            if (filePath == null)
            {
                return Json(new { success = false, errorMessage = "File is of wrong format." });
            }

            try
            {
                var viewModel = new FromExcelToDataBase<ExamineFromExcel>(uploadFileView.ReporterDate, this.unitOfWork, this.readFromExcel).WriteDataToDataBase(filePath, "", "B2", 1, 1);

                return PartialView("_ShowResultReadFromExcel", viewModel);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, errorMessage = ex.Message });
            }
        }
        #endregion

        #region 康复医学科
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _RehabilitationStatistical(UploadFileView uploadFileView, IEnumerable<HttpPostedFileBase> files)
        {
            var filePath = GetFilePathOrDefault(files);
            if (filePath == null)
            {
                return Json(new { success = false, errorMessage = "File is of wrong format." });
            }
            try
            {
                var viewModel = new FromExcelToDataBase<RehabilitationFromExcel>(uploadFileView.ReporterDate, this.unitOfWork, this.readFromExcel).WriteDataToDataBase(filePath, "", "B2", 1, 1);

                return PartialView("_ShowResultReadFromExcel", viewModel);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        #endregion

        #region 麻醉科
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _AnesthesiologyStatistical(UploadFileView uploadFileView, IEnumerable<HttpPostedFileBase> files)
        {
            var filePath = GetFilePathOrDefault(files);
            if (filePath == null)
            {
                return Json(new { success = false, errorMessage = "File is of wrong format." });
            }

             

            try
            {
                var viewModel = new FromExcelToDataBase<AnesthesiologyFromExcel>(uploadFileView.ReporterDate, this.unitOfWork, this.readFromExcel).WriteDataToDataBase(filePath, "", "B2", 1, 1);

                return PartialView("_ShowResultReadFromExcel", viewModel);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        #endregion

        #region 门诊部
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _ClinicStatistical(UploadFileView uploadFileView, IEnumerable<HttpPostedFileBase> files)
        {
            var filePath = GetFilePathOrDefault(files);
            if (filePath == null)
            {
                return Json(new { success = false, errorMessage = "File is of wrong format." });
            }

             

            try
            {
                var viewModel = new FromExcelToDataBase<ClinicFromExcel>(uploadFileView.ReporterDate, this.unitOfWork, this.readFromExcel).WriteDataToDataBase(filePath, "", "B2", 1, 1);

                return PartialView("_ShowResultReadFromExcel", viewModel);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        #endregion

        #region 药学部
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _PharmaceuticalStatistical(UploadFileView uploadFileView, IEnumerable<HttpPostedFileBase> files)
        {
            var filePath = GetFilePathOrDefault(files);
            if (filePath == null)
            {
                return Json(new { success = false, errorMessage = "File is of wrong format." });
            }

             

            try
            {
                var viewModel = new FromExcelToDataBase<PharmaceuticalFromExcel>(uploadFileView.ReporterDate, this.unitOfWork, this.readFromExcel).WriteDataToDataBase(filePath, "", "B2", 1, 1);

                return PartialView("_ShowResultReadFromExcel", viewModel);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        #endregion

        #region 判断文件路径
        /// <summary>
        /// 获得上传文件的路径名称，如果文件不存在则返回null
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private string GetFilePathOrDefault(IEnumerable<HttpPostedFileBase> files)
        {
            string filePath = null;
            if (!IsMatchedFileFormat(files))
            {
                return filePath;
            }
            var file = files.FirstOrDefault();  // get ONE only

            var serverPath = HttpContext.Server.MapPath(TempFolder);

            filePath = fileUpload.GetTempSavedFilePath(file, serverPath);

            //开始解析文件操作  
            filePath = filePath.Replace("\\", "/");
            filePath = HttpContext.Server.MapPath(filePath);
            return filePath;
        }
        private bool IsMatchedFileFormat(IEnumerable<HttpPostedFileBase> files)
        {
            //仅需要一个文件地址即可操作           
            if (files == null || !files.Any()) return false;
            var file = files.FirstOrDefault();  // get ONE only
            if (file == null || !fileUpload.IsMatchedFileFormat(file))
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}