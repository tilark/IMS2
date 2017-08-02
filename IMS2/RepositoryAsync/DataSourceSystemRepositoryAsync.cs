using IMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS2.RepositoryAsync
{
    public class DataSourceSystemRepositoryAsync : DomainRepositoryAsync<DataSourceSystem>
    {
        public DataSourceSystemRepositoryAsync(IDomainUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}