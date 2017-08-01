using IMS2.BusinessModel.SatisticsValueModel;
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

        Task<decimal?> GetSatisticsValue(Guid indicatorID, Guid durationId, Guid departmentId, DateTime time);
    }
}
