using IMS2.Models;
using OperateExcel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace IMS2.DAL
{
    public class IndicatorItem
    {
        public string Name { get; set; }
        public Guid GuidId { get; set; }

        public string Unit { get; set; }
        public string IsAuto { get; set; }
        public string DataSystem { get; set; }
        public string Department { get; set; }
        public string Remarks { get; set; }
        public string Duration { get; set; }

        public string DutyDepartment { get; set; }
        public decimal Priority { get; set; }

    }
    public class DepartmentCategoryMapIndicatorGroupItem
    {
        public string DepartmentCategoryName { get; set; }
        public string IndicatorGroupName { get; set; }
        public decimal Priority { get; set; }
    }

    public class InitialItemData
    {
        internal async void InitialDepartmentCategory()
        {
            var fileName = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager
                 .AppSettings["DepartmentCategory"]);
            //第一列为类别的名称，第二列开始为Guid
            ReadFromExcel readFromExcel = new ReadFromExcel();
            //按Row获取当前行的所有数据
            int rowCount = readFromExcel.GetRowCount(fileName);
            for (int i = 2; i <= rowCount; i++)
            {
                var columnData = readFromExcel.ReadRowFromExcel((uint)i, fileName);
                //获取第一个数为类别名
                //第二列为类别的Guid,数据必须大于等2才是正确的
                if (columnData.Count >= 4)
                {
                    //var firstData = columnData.First();
                    BaseRepository writeBaseData = new BaseRepository();
                    DepartmentCategory departmentCategory = new DepartmentCategory();
                    departmentCategory.DepartmentCategoryName = columnData.ElementAt(0);
                    departmentCategory.DepartmentCategoryId = Guid.Parse(columnData.ElementAt(1));
                    departmentCategory.Remarks = columnData.ElementAt(2);
                    departmentCategory.Priority = Decimal.Parse(columnData.ElementAt(3));
                    //写入数据库
                    await writeBaseData.AddDepartmentCategory(departmentCategory);

                }
            }
        }

        internal async void InitialIndicatorGroup()
        {
            var fileName = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager
                .AppSettings["IndicatorGroup"]);
            ReadFromExcel readFromExcel = new ReadFromExcel();
            //按Row获取当前行的所有数据
            int rowCount = readFromExcel.GetRowCount(fileName);
            for (int i = 2; i <= rowCount; i++)
            {
                var columnData = readFromExcel.ReadRowFromExcel((uint)i, fileName);
                //获取第一个数为名称
                //数据必须大于等4才是正确的
                if (columnData.Count >= 4)
                {
                    BaseRepository writeBaseData = new BaseRepository();
                    //第二列为来源系统Guid
                    IndicatorGroup indicatorGroup = new IndicatorGroup();
                    indicatorGroup.IndicatorGroupName = columnData.ElementAt(0);
                    indicatorGroup.IndicatorGroupId = Guid.Parse(columnData.ElementAt(1));
                    indicatorGroup.Priority = Decimal.Parse(columnData.ElementAt(2));
                    indicatorGroup.Remarks = columnData.ElementAt(3);
                    //写入数据库
                    await writeBaseData.AddIndicatorGroup(indicatorGroup);
                }
            }
        }

        internal async void InitialIndicatorGroupMapIndicator()
        {
            var fileName = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager
                 .AppSettings["IndicatorGroupMapIndicator"]);
            ReadFromExcel readFromExcel = new ReadFromExcel();
            //按Row获取当前行的所有数据
            int rowCount = readFromExcel.GetRowCount(fileName);
            for (int i = 2; i <= rowCount; i++)
            {
                var columnData = readFromExcel.ReadRowFromExcel((uint)i, fileName);
                //获取第一个数为指标组名称，第二列为指标名称，第三列为优先级
                //数据必须大于等3才是正确的
                if (columnData.Count >= 3)
                {
                    BaseRepository writeBaseData = new BaseRepository();
                    //第二列为来源系统Guid
                    IndicatorGroupMapIndicator indicatorGroupMapIndicator = new IndicatorGroupMapIndicator();
                    string indicatorGroupName = columnData.ElementAt(0);
                    string indicatorName = columnData.ElementAt(1);
                    indicatorGroupMapIndicator.Priority = Decimal.Parse(columnData.ElementAt(2));

                    //写入数据库
                    await writeBaseData.AddIndicatorGroupMapIndicator(indicatorGroupMapIndicator, indicatorGroupName, indicatorName);
                }
            }
        }

        internal async void InitialDataSourceSystem()
        {
            var fileName = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager
                .AppSettings["DataSourceSystem"]);
            ReadFromExcel readFromExcel = new ReadFromExcel();
            //按Row获取当前行的所有数据
            int rowCount = readFromExcel.GetRowCount(fileName);
            for (int i = 2; i <= rowCount; i++)
            {
                var columnData = readFromExcel.ReadRowFromExcel((uint)i, fileName);
                //获取第一个数为来源系统名称
                //数据必须大于等2才是正确的
                if (columnData.Count >= 2)
                {
                    BaseRepository writeBaseData = new BaseRepository();
                    //第二列为来源系统Guid
                    DataSourceSystem dataSourceSystem = new DataSourceSystem();
                    dataSourceSystem.DataSourceSystemName = columnData.ElementAt(0);
                    dataSourceSystem.DataSourceSystemId = Guid.Parse(columnData.ElementAt(1));
                    //写入数据库
                    await writeBaseData.AddDataSourceSystem(dataSourceSystem);
                }
            }
        }

        internal async void InitialDepartment()
        {
            var fileName = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager
               .AppSettings["Department"]);
            //第一列为科室名称，第二列所属科室类别，第三列为科室的Guid
            ReadFromExcel readFromExcel = new ReadFromExcel();
            //按Row获取当前行的所有数据
            int rowCount = readFromExcel.GetRowCount(fileName);
            for (int i = 2; i <= rowCount; i++)
            {
                var columnData = readFromExcel.ReadRowFromExcel((uint)i, fileName);
                //获取第一个数为科室名称
                //数据必须大于等5才是正确的
                if (columnData.Count >= 5)
                {
                    BaseRepository writeBaseData = new BaseRepository();
                    //DepartmentCategory departmentCategory = new DepartmentCategory();
                    //第二列为科室类别
                    //var name = columnData.ElementAt(1);
                    //var departmentCategory = writeBaseData.FindDepartmentCategoryByName(name);
                    //if (departmentCategory == null)
                    //{
                    //    return;
                    //}
                    Department department = new Department();
                    //第一列为科室名称
                    department.DepartmentName = columnData.ElementAt(0);

                    //第二列为科室Guid
                    department.DepartmentId = Guid.Parse(columnData.ElementAt(1));
                    //第三列为科室类别
                    var departmentCategoryName = columnData.ElementAt(2);

                    department.Priority = Decimal.Parse(columnData.ElementAt(3));
                    department.Remarks = columnData.ElementAt(4);
                    //department.DepartmentCategoryId = departmentCategory.DepartmentCategoryId;
                    //添加department
                    await writeBaseData.AddDepartment(department, departmentCategoryName);
                }
            }
        }

        internal async void InitialIndicator()
        {
            var indicatorFile = HttpContext.Current.Server
                .MapPath(System.Configuration.ConfigurationManager.AppSettings["Indicator"]);
            ReadFromExcel readFromExcel = new ReadFromExcel();
            //按Row获取当前行的所有数据
            //先获Row总数，从第二行开始取数据，第一行为标题
            int rowCount = readFromExcel.GetRowCount(indicatorFile);
            for (int i = 2; i <= rowCount; i++)
            {
                var columnData = readFromExcel.ReadRowFromExcel((uint)i, indicatorFile);
                //将该列按照IndicatorItem录入，再插入到数据库
                if (columnData.Count >= 10)
                {
                    IndicatorItem indicatorItem = new IndicatorItem();
                    //按照顺序填充
                    indicatorItem.Name = columnData.ElementAt(0);
                    indicatorItem.GuidId = Guid.Parse(columnData.ElementAt(1));
                    indicatorItem.Unit = columnData.ElementAt(2);
                    indicatorItem.IsAuto = columnData.ElementAt(3);
                    indicatorItem.DataSystem = columnData.ElementAt(4);
                    indicatorItem.Department = columnData.ElementAt(5);
                    indicatorItem.Remarks = columnData.ElementAt(6);
                    indicatorItem.Duration = columnData.ElementAt(7);
                    indicatorItem.DutyDepartment = columnData.ElementAt(8);
                    indicatorItem.Priority = Decimal.Parse(columnData.ElementAt(9));
                    //写入数据库
                    BaseRepository writeBaseData = new BaseRepository();
                   await writeBaseData.AddIndicator(indicatorItem);
                }
            }
        }

        internal async void InitialDepartmentCategoryMapIndicatorGroup()
        {
            var fileName = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager
                 .AppSettings["DepartmentCategoryMapIndicatorGroup"]);
            ReadFromExcel readFromExcel = new ReadFromExcel();
            //按Row获取当前行的所有数据
            int rowCount = readFromExcel.GetRowCount(fileName);
            for (int i = 2; i <= rowCount; i++)
            {
                var columnData = readFromExcel.ReadRowFromExcel((uint)i, fileName);
                //获取第一个数为名称
                //数据必须大于等3才是正确的
                if (columnData.Count >= 3)
                {
                    BaseRepository writeBaseData = new BaseRepository();
                    //第二列为来源系统Guid
                    DepartmentCategoryMapIndicatorGroupItem item = new DepartmentCategoryMapIndicatorGroupItem();
                    item.DepartmentCategoryName = columnData.ElementAt(0);
                    item.IndicatorGroupName = columnData.ElementAt(1);
                    item.Priority = Decimal.Parse(columnData.ElementAt(2));
                    //写入数据库
                    await writeBaseData.AddDepartmentCategoryMapIndicatorGroup(item);
                }
            }
        }

        internal async void InitialIndicatorAlgorithm()
        {
            var fileName = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager
               .AppSettings["IndicatorAlgorithm"]);
            ReadFromExcel readFromExcel = new ReadFromExcel();
            //按Row获取当前行的所有数据
            int rowCount = readFromExcel.GetRowCount(fileName);
            for (int i = 2; i <= rowCount; i++)
            {
                var columnData = readFromExcel.ReadRowFromExcel((uint)i, fileName);
                //数据必须大于等5才是正确的
                //第1列为结果项目的Guid,
                //第2列为第一个项目的Guid
                //第3列为第二个项目的Guid
                //第4列为操作符
                //第5列为备注
                if (columnData.Count >= 5)
                {
                    //var firstData = columnData.First();
                    BaseRepository writeBaseData = new BaseRepository();
                    IndicatorAlgorithm indicatorAlgorithm = new IndicatorAlgorithm();
                    indicatorAlgorithm.ResultId = Guid.Parse(columnData.ElementAt(0));
                    indicatorAlgorithm.FirstOperandID = Guid.Parse(columnData.ElementAt(1));
                    indicatorAlgorithm.SecondOperandID = Guid.Parse(columnData.ElementAt(2));
                    indicatorAlgorithm.OperationMethod = columnData.ElementAt(3);
                    indicatorAlgorithm.Remarks = columnData.ElementAt(4);
                    indicatorAlgorithm.IndicatorAlgorithmsId = System.Guid.NewGuid();
                    await writeBaseData.AddIndicatorAlgorithm(indicatorAlgorithm);

                }
            }
        }

        internal async void InitialDuration()
        {
            var fileName = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager
                 .AppSettings["Duration"]);
            ReadFromExcel readFromExcel = new ReadFromExcel();
            //按Row获取当前行的所有数据
            int rowCount = readFromExcel.GetRowCount(fileName);
            for (int i = 2; i <= rowCount; i++)
            {
                var columnData = readFromExcel.ReadRowFromExcel((uint)i, fileName);
                //获取第一个数为名称
                //数据必须大于等3才是正确的
                if (columnData.Count >= 3)
                {
                    BaseRepository writeBaseData = new BaseRepository();
                    //第二列为来源系统Guid
                    Duration duration = new Duration();
                    duration.DurationName = columnData.ElementAt(0);
                    duration.DurationId = Guid.Parse(columnData.ElementAt(1));
                    duration.Remarks = columnData.ElementAt(2);
                    //写入数据库
                    await writeBaseData.AddDuration(duration);
                }
            }
        }
    }
}