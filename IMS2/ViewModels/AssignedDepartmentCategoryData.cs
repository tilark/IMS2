using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IMS2.Models;
namespace IMS2.ViewModels
{
    public class AssignedDepartmentCategoryData
    {
        public Guid DepartmentCategoryID { get; set; }
        public string DepartmentCategoryName { get; set; }
        public bool Assigned { get; set; }
        public string Color { get; set; }
    }
}