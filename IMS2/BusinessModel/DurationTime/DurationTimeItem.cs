using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS2.BusinessModel.DurationTime
{
    /// <summary>
    /// 时段时间项。
    /// </summary>
    /// <remarks>记录“时段ID”与“时间”的一个组合。可用于同时涉及到“时段”与“时间”的情景中，如虚拟值算法。</remarks>
    /// <see cref="时段时间组合算法"/>
    public class DurationTimeItem
    {
        /// <summary>
        /// 初始化。
        /// </summary>
        public DurationTimeItem()
        {

        }





        /// <summary>
        /// 时段ID。
        /// </summary>
        public Guid DurationId { get; set; }

        /// <summary>
        /// 时间。
        /// </summary>
        public DateTime Time { get; set; }
    }
}