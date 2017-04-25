using IMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS2.RepositoryAsync
{
    public class DepartmentIndicatorValueRepositoryAsync : DomainRepositoryAsync<DepartmentIndicatorValue>
    {
        public DepartmentIndicatorValueRepositoryAsync(IDomainUnitOfWork unit)
            : base(unit)
        {
        }
    }
}