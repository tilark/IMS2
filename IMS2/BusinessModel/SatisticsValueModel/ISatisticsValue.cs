using IMS2.ViewModels.StatisticsDepartmentIndicatorValueViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS2.BusinessModel.SatisticsValueModel
{
    /// <summary>
    /// 获得聚合、统计值的接口
    /// </summary>
    public interface ISatisticsValue
    {
       Task<decimal?> GetSatisticsValue(DepartmentIndicatorDurationTime departmentIndicatorDurationTime);
    }
}
