using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS2.BusinessModel.IndicatorDepartmentModel
{
    public interface IIndicatorDepartment
    {
        /// <summary>
        /// 获得算法表中ResultID的Indicator与Department对应的列表
        /// </summary>
        /// <returns></returns>
        Task<List<IndicatorDepartment>> GetAlgorithmIndicatorDepartment();
    }
}
