using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IMS2.Models;

namespace IMS2.RepositoryAsync
{
    public class DepartmentRepositoryAsync : DomainRepositoryAsync<Department>
    {
        public DepartmentRepositoryAsync(IDomainUnitOfWork unit)
            : base(unit)
        {
        }
    }
}