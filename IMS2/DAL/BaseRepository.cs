using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IMS2.Models;
using System.Threading.Tasks;

namespace IMS2.DAL
{
    public class BaseRepository
    {
        public async Task<DepartmentCategory> AddDepartmentCategory(DepartmentCategory departmentCategory)
        {
            DepartmentCategory item = null;
            if (departmentCategory == null || departmentCategory.DepartmentCategoryName == null
                || departmentCategory.DepartmentCategoryId == null)
            {
                return item;
            }
            using (ImsDbContext context = new ImsDbContext())
            {
                //用ID查重
                item = await context.DepartmentCategories.FindAsync(departmentCategory.DepartmentCategoryId);
                if (item == null)
                {
                    //如果为null,说明数据库不存在该项
                    item = departmentCategory;
                    context.DepartmentCategories.Add(item);
                    await context.SaveChangesAsync();
                }
            }
            return item;
        }

        public async Task<DataSourceSystem> AddDataSourceSystem(DataSourceSystem dataSourceSystem)
        {
            DataSourceSystem item = null;
            if (dataSourceSystem == null || dataSourceSystem.DataSourceSystemId == null
                || dataSourceSystem.DataSourceSystemName == null)
            {
                return null;
            }
            //ID查重
            using (ImsDbContext context = new ImsDbContext())
            {
                item = await context.DataSourceSystems.FindAsync(dataSourceSystem.DataSourceSystemId);

                if (item == null)
                {
                    item = dataSourceSystem;
                    context.DataSourceSystems.Add(item);
                    await context.SaveChangesAsync();
                }
            }
            return item;
        }

        public async Task<Duration> AddDuration(Duration duration)
        {
            Duration item = null;
            if (duration == null || duration.DurationId == null || String.IsNullOrEmpty(duration.DurationName))
            {
                return item;
            }
            using (ImsDbContext context = new ImsDbContext())
            {
                //ID
                item = await context.Durations.FindAsync(duration.DurationId);
                if (item == null)
                {
                    item = duration;
                    context.Durations.Add(item);
                    await context.SaveChangesAsync();
                }
            }
            return item;
        }



        public async Task<Department> AddDepartment(Department department)
        {
            Department item = null;
            if (department == null || department.DepartmentId == null || String.IsNullOrEmpty(department.DepartmentName))
            {
                return item;
            }
            using (ImsDbContext context = new ImsDbContext())
            {
                //ID
                item = await context.Departments.FindAsync(department.DepartmentId);
                if (item == null)
                {
                    item = department;
                    context.Departments.Add(item);
                    await context.SaveChangesAsync();
                }
            }
            return item;
        }

        public async Task<IndicatorGroup> AddIndicatorGroup(IndicatorGroup indicatorGroup)
        {
            IndicatorGroup item = null;
            if (indicatorGroup == null || indicatorGroup.IndicatorGroupId == null
                || String.IsNullOrEmpty(indicatorGroup.IndicatorGroupName))
            {
                return item;
            }
            using (ImsDbContext context = new ImsDbContext())
            {
                //ID
                item = await context.IndicatorGroups.FindAsync(indicatorGroup.IndicatorGroupId);
                if (item == null)
                {
                    item = indicatorGroup;
                    context.IndicatorGroups.Add(item);
                    await context.SaveChangesAsync();
                }
            }
            return item;
        }

        public async Task<UserDepartment> AddUserDepartment(UserDepartment userDepartment)
        {
            UserDepartment item = null;
            if(userDepartment == null)
            {
                return item;
            }
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                item = await context.UserDepartments.FindAsync(userDepartment.UserDepartmentId);
                if(item == null)
                {
                    item = userDepartment;
                    context.UserDepartments.Add(item);
                    await context.SaveChangesAsync();
                }
            }
            return item;
        }

        public async Task<IndicatorGroupMapIndicator> AddIndicatorGroupMapIndicator(IndicatorGroupMapIndicator indicatorGroupMapIndicator, string indicatorGroupName, string indicatorName)
        {
            IndicatorGroupMapIndicator item = null;
            if (indicatorGroupMapIndicator == null
                || String.IsNullOrEmpty(indicatorGroupName)
                || String.IsNullOrEmpty(indicatorName))
            {
                return item;
            }
            using (ImsDbContext context = new ImsDbContext())
            {
                try
                {
                    //查找指标组，获取指标
                    var indicatorGroup = context.IndicatorGroups.Where(i => i.IndicatorGroupName == indicatorGroupName)
                                        .SingleOrDefault();
                    var indicator = context.Indicators.Where(i => i.IndicatorName == indicatorName)
                                    .SingleOrDefault();
                    if (indicatorGroup == null || indicator == null)
                    {
                        //若不存在项目组和项目，直接返回
                        return item;
                    }
                    //一个指标组可以有多个不同的指标
                    item = context.IndicatorGroupMapIndicators.Where(i => i.IndicatorGroupId == indicatorGroup.IndicatorGroupId
                                && i.IndicatorId == indicator.IndicatorId).SingleOrDefault();
                    if (item == null)
                    {
                        item = indicatorGroupMapIndicator;
                        item.IndicatorGroupId = indicatorGroup.IndicatorGroupId;
                        item.IndicatorId = indicator.IndicatorId;
                        item.IndicatorGroupMapIndicatorId = System.Guid.NewGuid();

                        context.IndicatorGroupMapIndicators.Add(item);
                        await context.SaveChangesAsync();
                    }
                }
                catch (Exception)
                {
                }
                
            }
            return item;
        }

        public async Task<Department> AddDepartment(Department department, string departmentCategoryName)
        {
            Department item = null;
            if (department == null || department.DepartmentId == null
                || String.IsNullOrEmpty(department.DepartmentName)
                || String.IsNullOrEmpty(departmentCategoryName))
            {
                return item;
            }
            using (ImsDbContext context = new ImsDbContext())
            {
                //ID
                item = await context.Departments.FindAsync(department.DepartmentId);
                if (item == null)
                {
                    var departmentCategory = context.DepartmentCategories
                                            .Where(d => d.DepartmentCategoryName == departmentCategoryName)
                                            .SingleOrDefault();
                    if (departmentCategory == null)
                    {
                        return item;
                    }
                    item = department;
                    item.DepartmentCategoryId = departmentCategory.DepartmentCategoryId;
                    context.Departments.Add(item);
                    await context.SaveChangesAsync();
                }
            }
            return item;
        }

        public async Task<Indicator> AddIndicator(IndicatorItem indicatorItem)
        {
            Indicator item = null;

            if (indicatorItem == null || indicatorItem.GuidId == null
                || indicatorItem.Name == null)
            {
                return null;
            }
            using (ImsDbContext context = new ImsDbContext())
            {
               
                try
                {
                    //ID查重
                    item = await context.Indicators.FindAsync(indicatorItem.GuidId);
                    if (item != null)
                    {
                        return item;
                    }
                    item = new Indicator();
                    //根据IsAuto从不同表中查找ID
                    if (indicatorItem.IsAuto == "是")
                    {
                        item.IsAutoGetData = true;
                        var dataSystem = context.DataSourceSystems.Where(d => d.DataSourceSystemName == indicatorItem.DataSystem).Single();
                        //若不存在，会引发异常,
                        //需获取DataSystem的ID值
                        item.DataSourceSystemId = dataSystem.DataSourceSystemId;
                        item.ProvidingDepartmentId = null;
                    }
                    else
                    {
                        item.IsAutoGetData = false;
                        var department = context.Departments.Where(d => d.DepartmentName == indicatorItem.Department).Single();

                        //如果科室不存在，说明有问题，引发异常
                        //获取department的ID
                        item.ProvidingDepartmentId = department.DepartmentId;
                        item.DataSourceSystemId = null;
                    }
                    //指标可没有责任科室
                    var dutyDepartment = context.Departments.Where(d => d.DepartmentName == indicatorItem.DutyDepartment).FirstOrDefault();
                    var duration = context.Durations.Where(d => d.DurationName == indicatorItem.Duration).Single();

                    //继续赋其他值
                    item.IndicatorName = indicatorItem.Name;
                    item.Unit = indicatorItem.Unit;
                    item.DutyDepartmentId = dutyDepartment?.DepartmentId;
                    item.IndicatorId = indicatorItem.GuidId;
                    item.Remarks = indicatorItem.Remarks;
                    item.Priority = indicatorItem.Priority;
                    item.DurationId = duration.DurationId;
                    context.Indicators.Add(item);
                    await context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    //处理异常
                }
            }
            return item;
        }

        public async Task<DepartmentCategoryMapIndicatorGroup> AddDepartmentCategoryMapIndicatorGroup(DepartmentCategoryMapIndicatorGroupItem mapItem)
        {
            DepartmentCategoryMapIndicatorGroup item = null;

            if (mapItem == null || String.IsNullOrEmpty(mapItem.DepartmentCategoryName)
                || String.IsNullOrEmpty(mapItem.IndicatorGroupName))
            {
                return null;
            }
            using (ImsDbContext context = new ImsDbContext())
            {
                try
                {
                    item = new DepartmentCategoryMapIndicatorGroup();
                    var departmentCategory = context.DepartmentCategories.Where(d => d.DepartmentCategoryName == mapItem.DepartmentCategoryName)
                        .Single();
                    var indicatorGroup = context.IndicatorGroups.Where(i => i.IndicatorGroupName == mapItem.IndicatorGroupName)
                        .Single();
                    item.DepartmentCategoryId = departmentCategory.DepartmentCategoryId;
                    item.IndicatorGroupId = indicatorGroup.IndicatorGroupId;
                    item.Priority = mapItem.Priority;
                    item.DepartmentCategoryMapIndicatorGroupId = System.Guid.NewGuid();
                    context.DepartmentCategoryMapIndicatorGroups.Add(item);
                    await context.SaveChangesAsync();
                }
                catch (Exception)
                {

                }
            }
            return item;
        }

        public async Task<IndicatorAlgorithm> AddIndicatorAlgorithm(IndicatorAlgorithm indicatorAlgorithm)
        {
            IndicatorAlgorithm item = null;
            if (indicatorAlgorithm == null || indicatorAlgorithm.ResultId == null
                || indicatorAlgorithm.FirstOperandID == null || indicatorAlgorithm.SecondOperandID == null
                || String.IsNullOrEmpty(indicatorAlgorithm.OperationMethod))
            {
                return null;
            }
            using (ImsDbContext context = new ImsDbContext())
            {
                try
                {
                    //ResultOperation查找，如果能找到，不添加
                    item = context.IndicatorAlgorithms.Where(i => i.ResultId == indicatorAlgorithm.ResultId).SingleOrDefault();
                    if (item == null)
                    {
                        //如果仍然没有，则可以添加到数据库
                        item = indicatorAlgorithm;
                        context.IndicatorAlgorithms.Add(item);
                        await context.SaveChangesAsync();
                    }
                }
                catch (Exception)
                {
                }
            }
            return item;
        }
    }
}