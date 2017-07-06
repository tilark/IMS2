using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS2.ViewModels.StatisticsDepartmentIndicatorValueViews
{
    /// <summary>
    /// 从数据库取数，出错的情况有以下：
    /// 1、原数据库中无该指标数或者该数没有审核(IsLocked)，将返回Null
    /// 2、输入的Duration的跨度低于指标本身的跨度，如指标为季指标，但输入的DurationID为月
    /// 3、除0错误等其他的基本运算错误
    /// </summary>
    public class SatisticsIndicatorHandleMessage
    {
        public Guid IndicatorID { get; set; }
        public Guid DepartmentID { get; set; }  
        public Guid DurationID { get; set; }
        public DateTime Time { get; set; }
        public string ErrorMessage { get; set; }

        public long Count { get; set; }
        public bool Successed { get; set; }
    }
}