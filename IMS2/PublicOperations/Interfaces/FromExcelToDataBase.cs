using ExcelEntityOperation;
using IMS2.Models;
using IMS2.RepositoryAsync;
using IMS2.ViewModels.UploadFileViews;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace IMS2.PublicOperations.Interfaces
{
    public  class FromExcelToDataBase<T> where T:class, new()
    {
        public DateTime RecordDate { get; set; }
        private IDomainUnitOfWork unitOfWork;
        private IReadFromExcel readFromExcel;
        private Dictionary<string, T> importData;
        public FromExcelToDataBase(DateTime recordDate, IDomainUnitOfWork unitOfWork, IReadFromExcel readFromExcel)
        {
            this.RecordDate = recordDate;
            this.unitOfWork = unitOfWork;
            this.readFromExcel = readFromExcel;
        }

        public void ReadFromExcel(string filePath,  string sheetName = null, string startCellName = "A1", int mergeTitleRow = 1, int keyColumn = 1)
        {
            
            var cellHeader = GetCellHeader();
            var errorMsg = new StringBuilder(100);
            importData = this.readFromExcel.ExcelToEntityDictionary<T>(cellHeader, filePath, out errorMsg, sheetName, startCellName, mergeTitleRow, keyColumn);
            
        }

        public ReadExcelMessage WriteDataToDataBase(string filePath, string sheetName = null, string startCellName = "A1", int mergeTitleRow = 1, int keyColumn = 1)
        {
            var result = new ReadExcelMessage();
            var errorMessage = new StringBuilder(100);
            var cellHeader = GetCellHeader();
            importData = this.readFromExcel.ExcelToEntityDictionary<T>(cellHeader, filePath, out errorMessage, sheetName, startCellName, mergeTitleRow, keyColumn);
            if(errorMessage.Length > 0)
            {
                return new ReadExcelMessage { TotalCount = 0, ErrorMessage = errorMessage.ToString(), ReadFailedCount = 0, ReadSuccessCount = 0 };
            }
           
            if (importData.Count > 0)
            {
                result.TotalCount = importData.Count;
                foreach (var item in importData)
                {
                    foreach(var itemProperty in typeof(T).GetProperties())
                    {
                        var itemType = itemProperty.PropertyType;

                        var itemName = itemProperty.Name;
                        var itemValue =itemProperty.GetValue(item.Value);
                        //即项目名称
                        var itemDisplayAttributeName = GetDisplayAttributeName(itemName);
                        decimal value;
                        if (decimal.TryParse((string)itemValue, out value))
                        {
                            //解析成功，从数据库中查找到对应的DepartmentID，IndicatorID,写入DepartmentIndicatorValue中

                            //根据科室名找到DepartmentID
                            var departmentID = GetDepartmentID(item.Key);
                            if (departmentID.Equals(System.Guid.Empty))
                            {
                                result.ReadFailedCount++;
                                errorMessage.Append(String.Format("未找到科室<{0}>的对应项，请检查数据是否有误!\n", item.Key));
                                continue;
                            }

                            //根据项目名找到IndicatorID
                            var indicatorName = itemDisplayAttributeName;
                            var indicatorID = GetIndicatorID(indicatorName);
                            if (indicatorID.Equals(System.Guid.Empty))
                            {
                                result.ReadFailedCount++;
                                errorMessage.Append(String.Format("未找到项目名称<{0}>的对应项，请检查数据是否有误!\n", indicatorName));
                                continue;
                            }
                            //写入到数据库中
                            string saveErrorMessage;
                            bool successBool = SaveToDataBase(departmentID, indicatorID, value, this.RecordDate, out saveErrorMessage);
                            if (successBool)
                            {
                                result.ReadSuccessCount++;
                            }
                            else
                            {
                                result.ReadFailedCount++;
                                errorMessage.Append(String.Format("项目名称<{0}>，科室<{1}>，值<{2}>保存失败，原因:{3}!\n", item.Key, indicatorName, value, saveErrorMessage));
                            }
                        }
                    }
                }
            }
            if(errorMessage.Length > 0)
            {
                result.ErrorMessage = errorMessage.ToString();
            }
            return result;
        }

        private bool SaveToDataBase(Guid departmentID, Guid indicatorID, decimal value, object reporterDate, out string saveErrorMessage)
        {
            bool result = false;
            saveErrorMessage = String.Empty;
            var repo = new DepartmentIndicatorValueRepositoryAsync(this.unitOfWork);
            var query = repo.GetAll(a => a.DepartmentId == departmentID && a.IndicatorId == indicatorID && a.Time == this.RecordDate).FirstOrDefault();
            if (query == null)
            {
                var departmentIndicatorValue = new DepartmentIndicatorValue { DepartmentIndicatorValueId = System.Guid.NewGuid(), DepartmentId = departmentID, IndicatorId = indicatorID, UpdateTime = System.DateTime.Now, IsLocked = false, Time = this.RecordDate, Value = value };
                repo.Add(departmentIndicatorValue);
                try
                {
                    this.unitOfWork.SaveChangesClientWin();
                    result = true;
                }
                catch (Exception)
                {
                    result = false;
                    saveErrorMessage = "保存数据失败";
                }
            }
            else
            {
                if (!query.IsLocked)
                {
                    query.Value = value;
                    repo.Update(query);
                    try
                    {
                        this.unitOfWork.SaveChangesClientWin();
                        result = true;
                    }
                    catch (Exception)
                    {
                        result = false;
                        saveErrorMessage = "保存数据失败\n";
                    }
                }
                else
                {
                    saveErrorMessage = "该数据已存在于数据库中，并且已审核，无法修改\n";
                }
            }

            return result;
        }



        /// <summary>
        /// 根据项目名称找到项目ID
        /// </summary>
        /// <param name="indicatorName"></param>
        /// <returns></returns>
        protected Guid GetIndicatorID(string indicatorName)
        {
            var indicatorRepo = new IndicatorRepositoryAsync(this.unitOfWork);
            var indicator = indicatorRepo.GetAll(a => a.IndicatorName == indicatorName).FirstOrDefault();
            if (indicator != null)
            {
                return indicator.IndicatorId;
            }
            else
            {
                return System.Guid.Empty;
            }
        }
        /// <summary>
        /// 根据科室名称找到科室ID
        /// </summary>
        /// <param name="departmentName"></param>
        /// <returns></returns>
        protected Guid GetDepartmentID(string departmentName)
        {
            var departmentRepo = new DepartmentRepositoryAsync(this.unitOfWork);
            var department = departmentRepo.GetAll(a => a.DepartmentName == departmentName).FirstOrDefault();
            if (department != null)
            {
                return department.DepartmentId;
            }
            else
            {
                return System.Guid.Empty;
            }
        }
        /// <summary>
        /// 获取类中属性的DisplayAttribute的名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected string GetDisplayAttributeName(string propertyName)
        {
            var typeInfo = typeof(T);
            return (typeInfo.GetProperty(propertyName).GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault() as DisplayAttribute).GetName();
        }
        /// <summary>
        /// 获取类中属性的DisplayAttribute值，该值作为Value，属性名作为Key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected Dictionary<string, string> GetCellHeader()
        {
            List<PropertyInfo> propertyInfoList = new List<PropertyInfo>(typeof(T).GetProperties());
            var cellHeader = new Dictionary<string, string>();
            foreach (var property in propertyInfoList)
            {
                //var displayName2 = property.GetCustomAttribute<DisplayNameAttribute>(true).DisplayName;
                cellHeader[property.Name] = (property.GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault() as DisplayAttribute).GetName();
            }

            return cellHeader;
        }
       
       
       
    }
}