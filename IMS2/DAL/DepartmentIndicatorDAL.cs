//using IMS2.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using IMS2.ViewModels;
//using System.Threading.Tasks;
//using System.Data.Entity.Infrastructure;

//namespace IMS2.DAL
//{
//    public class DepartmentIndicatorDAL
//    {
//        public async Task<DepartmentIndicatorValue> CreateDepartmentIndicatorList(DateTime? searchTime, Indicator indicator, Department department)
//        {
//            if (searchTime == null || indicator == null || department == null)
//            {
//                return null;
//            }
//            bool canAdd = false;
//            switch (indicator.Duration.DurationName)
//            {
//                case "季":
//                    if (searchTime.Value.Month == 3 || searchTime.Value.Month == 6 || searchTime.Value.Month == 9 || searchTime.Value.Month == 12)
//                    {
//                        canAdd = true;
//                    }
//                    break;
//                case "半年":
//                    if (searchTime.Value.Month == 6)
//                    {
//                        canAdd = true;
//                    }
//                    break;
//                case "全年":
//                    if (searchTime.Value.Month == 12)
//                    {
//                        canAdd = true;
//                    }
//                    break;
//                default:
//                    canAdd = true;
//                    break;
//            }
//            if (canAdd)
//            {

//                DepartmentIndicatorValue departmentIndicatorValue = new DepartmentIndicatorValue();
//                departmentIndicatorValue.DepartmentId = department.DepartmentId;
//                departmentIndicatorValue.IndicatorId = indicator.IndicatorId;
//                departmentIndicatorValue.DepartmentIndicatorValueId = System.Guid.NewGuid();
//                departmentIndicatorValue.Time = searchTime.Value;
//                //需找到最新的版本号
//                var standardValue = db.DepartmentIndicatorStandards.Where(d => d.DepartmentId == department.DepartmentId && d.IndicatorId == indicator.IndicatorId
//                        && d.Version == db.DepartmentIndicatorStandards.Where(i => i.DepartmentId == department.DepartmentId && i.IndicatorId == indicator.IndicatorId).Max(v => v.Version))
//                        .FirstOrDefault();
//                departmentIndicatorValue.IndicatorStandardId = standardValue?.DepartmentIndicatorStandardId;
//                departmentIndicatorValue.IsLocked = false;
//                departmentIndicatorValue.UpdateTime = DateTime.Now;
//                //将科室、项目、时间添加到科室值表中
//                return await AddDepartmentIndicatorValue(departmentIndicatorValue);
//            }
//            return null;
//        }

//        public async Task<DepartmentIndicatorValue> AddDepartmentIndicatorValue(DepartmentIndicatorValue departmentIndicatorValue)
//        {
//            DepartmentIndicatorValue item = null;
//            if (departmentIndicatorValue == null)
//            {
//                return null;
//            }
//            //查重
//            item = db.DepartmentIndicatorValues.Where(d => d.DepartmentId == departmentIndicatorValue.DepartmentId
//                            && d.IndicatorId == departmentIndicatorValue.IndicatorId
//                            && d.Time.Year == departmentIndicatorValue.Time.Year && d.Time.Month == departmentIndicatorValue.Time.Month)
//                            .FirstOrDefault();
//            if (item == null)
//            {
//                item = departmentIndicatorValue;
//                db.DepartmentIndicatorValues.Add(item);
//                //client win
//                bool saveFailed;
//                do
//                {
//                    saveFailed = false;
//                    try
//                    {
//                        await db.SaveChangesAsync();

//                    }
//                    catch (DbUpdateConcurrencyException ex)
//                    {
//                        saveFailed = true;

//                        // Update original values from the database 
//                        var entry = ex.Entries.Single();
//                        entry.OriginalValues.SetValues(entry.GetDatabaseValues());
//                    }

//                } while (saveFailed);
//            }
//            return item;
//        }
//    }
//}