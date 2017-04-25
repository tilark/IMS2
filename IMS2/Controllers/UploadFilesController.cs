using IMS2.PublicOperations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace IMS2.Controllers
{
    public class UploadFilesController : Controller, IFileUpload
    {
        private string TempFolder;

        public UploadFilesController()
        {
            this.TempFolder = System.Configuration.ConfigurationManager.AppSettings["UpLoadFileDirectory"].ToString() ?? "/UpLoadFileDirectory";
        }
        // GET: UploadFiles
        public ActionResult Index()
        {
            return View();
        }

        #region 文件上传
        public ActionResult _UploadFile(string actionName, string controllerName, string importExcelName)
        {
            ViewBag.ActionName = actionName;
            ViewBag.ControllerName = controllerName;
            ViewBag.ImportExcelName = importExcelName;
            return PartialView();
        }
        #endregion

        #region 实现IFileUpload       

        public bool IsMatchedFileFormat(HttpPostedFileBase file)
        {
            return Regex.IsMatch(file.FileName, ".xlsx$");
        }


        public string GetTempSavedFilePath(HttpPostedFileBase file, string serverPath)
        {
            // Define destination
            if (Directory.Exists(serverPath) == false)
            {
                Directory.CreateDirectory(serverPath);
            }

            // Generate unique file name
            var fileName = Path.GetFileName(file.FileName);
            fileName = SaveTemporaryAvatarFileImage(file, serverPath, fileName);

            // Clean up old files after every save
            CleanUpTempFolder(1);
            return Path.Combine(this.TempFolder, fileName);
        }

        private static string SaveTemporaryAvatarFileImage(HttpPostedFileBase file, string serverPath, string fileName)
        {

            var fullFileName = Path.Combine(serverPath, fileName);
            if (System.IO.File.Exists(fullFileName))
            {
                System.IO.File.Delete(fullFileName);
            }

            file.SaveAs(fullFileName);
            return Path.GetFileName(file.FileName);
        }
       
        /// <summary>
        /// 清除临时文件夹中已保留一个小时的文件。此项工作可以放到一个线程中处理。
        /// </summary>
        /// <param name="hoursOld"></param>
        private void CleanUpTempFolder(int hoursOld)
        {
            try
            {
                var currentUtcNow = DateTime.UtcNow;
                var serverPath = HttpContext.Server.MapPath(this.TempFolder);
                if (!Directory.Exists(serverPath)) return;
                var fileEntries = Directory.GetFiles(serverPath);
                foreach (var fileEntry in fileEntries)
                {
                    var fileCreationTime = System.IO.File.GetCreationTimeUtc(fileEntry);
                    var res = currentUtcNow - fileCreationTime;
                    if (res.TotalHours > hoursOld)
                    {
                        System.IO.File.Delete(fileEntry);
                    }
                }
            }
            catch
            {
                // Deliberately empty.
            }
        }
        #endregion
    }
}