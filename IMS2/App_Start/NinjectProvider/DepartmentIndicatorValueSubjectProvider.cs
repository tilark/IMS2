using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject;
using Ninject.Activation;
using IMS2.BusinessModel.ObserverMode.Dad;
using IMS2.RepositoryAsync;
using IMS2.BusinessModel.SatisticsValueModel;

namespace IMS2.App_Start.NinjectProvider
{
    public class DepartmentIndicatorValueSubjectProvider : Provider<DepartmentIndicatorValueSubject>
    {
        protected override DepartmentIndicatorValueSubject CreateInstance(IContext context)
        {
            var departmentIndicatorValueSubject = new DepartmentIndicatorValueSubject(context.Kernel.Get<DomainUnitOfWork>());
            var virtualValueObject = new VirtualValueObserver(context.Kernel.Get<DomainUnitOfWork>(), context.Kernel.Get<SatisticsValue>());
            departmentIndicatorValueSubject.Attach(virtualValueObject);
            return departmentIndicatorValueSubject;
        }
    }
}