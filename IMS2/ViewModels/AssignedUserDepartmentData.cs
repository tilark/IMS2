using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS2.ViewModels
{
    public class AssignedUserDepartmentData
    {
        public Guid UserDepartmentId { get; set; }
        public string UserDepartmentName { get; set; }
        public bool Assigned { get; set; }
    }
}