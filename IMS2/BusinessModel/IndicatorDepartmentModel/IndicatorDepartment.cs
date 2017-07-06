using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS2.BusinessModel.IndicatorDepartmentModel
{
    /// <summary>
    /// 
    /// </summary>
    public class IndicatorDepartment
    {
        public Guid IndicatorID { get; set; }

        public List<Guid> DepartmentIDList { get; set; }
    }
}