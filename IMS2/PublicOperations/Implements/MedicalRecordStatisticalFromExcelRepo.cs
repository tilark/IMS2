using IMS2.PublicOperations.Interfaces;
using IMS2.ViewModels.ImportDepartmentIndicatorViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExcelEntityOperation;
using IMS2.RepositoryAsync;
using IMS2.ViewModels.UploadFileViews;

namespace IMS2.PublicOperations.Implements
{
    public class MedicalRecordStatisticalFromExcelRepo : FromExcelToDataBase<MedicalRecordStatisticalFromExcel>
    {
        public MedicalRecordStatisticalFromExcelRepo(UploadFileView uploadFileView, IDomainUnitOfWork unitOfWork, IReadFromExcel readFromExcel) : base(uploadFileView.ReporterDate, unitOfWork, readFromExcel)
        {

        }
    }
}