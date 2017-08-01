using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IMS2.Models;

namespace IMS2.RepositoryAsync
{

    public class DepartmentIndicatorDurationVirtualValueRepositoryAsync : DomainRepositoryAsync<DepartmentIndicatorDurationVirtualValue>
    {
        public DepartmentIndicatorDurationVirtualValueRepositoryAsync(IDomainUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}