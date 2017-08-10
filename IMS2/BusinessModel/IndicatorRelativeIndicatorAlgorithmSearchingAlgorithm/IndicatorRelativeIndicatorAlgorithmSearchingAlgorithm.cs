using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS2.BusinessModel.IndicatorRelativeIndicatorAlgorithmSearchingAlgorithm
{
    /// <summary>
    /// 指标关联算法查找算法。
    /// </summary>
    /// <remarks>找出与指标关联的算法。</remarks>
    /// <see cref="指标关联算法查找算法"/>
    public class IndicatorRelativeIndicatorAlgorithmSearchingAlgorithm
    {
        /// <summary>
        /// 初始化。
        /// </summary>
        public IndicatorRelativeIndicatorAlgorithmSearchingAlgorithm()
        {

        }





        /// <summary>
        /// 查找“结果ID”集合。
        /// </summary>
        /// <param name="indicatorId">指标ID。</param>
        /// <returns>与“指标ID”关联的“结果ID”集合。</returns>
        /// <remarks>循环查找。</remarks>
        /// <see cref="指标关联算法查找算法"/>
        public List<Guid> Find(Guid indicatorId)
        {
            List<Guid> returnResultIds = new List<Guid>();
            List<Guid> searchResultIds = new List<Guid> { indicatorId };

            var db = new Models.ImsDbContext();

            do
            {
                var list = db.IndicatorAlgorithms.Where(c => searchResultIds.Contains(c.FirstOperandID) || searchResultIds.Contains(c.SecondOperandID)).ToList();

                searchResultIds = list.Select(c => c.ResultId).ToList();

                if (searchResultIds.Count() > 0)
                {
                    returnResultIds = returnResultIds.Union(searchResultIds).ToList();
                }
                else
                    break;
            } while (true);

            return returnResultIds;
        }
    }
}