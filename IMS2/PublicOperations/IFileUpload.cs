using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IMS2.PublicOperations
{
    public interface IFileUpload
    {
        string GetTempSavedFilePath(HttpPostedFileBase file, string serverPath);
        bool IsMatchedFileFormat(HttpPostedFileBase file);
    }
}
