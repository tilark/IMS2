using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject;
using Ninject.Activation;
using IMS2.BusinessModel.ObserverMode.Dad;
using IMS2.RepositoryAsync;
using IMS2.BusinessModel.SatisticsValueModel;
using IMS2.BusinessModel.AlgorithmModel;

namespace IMS2.App_Start.NinjectProvider
{
    public class DepartmentIndicatorValueSubjectProvider : Provider<DepartmentIndicatorValueSubject>
    {
        protected override DepartmentIndicatorValueSubject CreateInstance(IContext context)
        {
            var departmentIndicatorValueSubject = new DepartmentIndicatorValueSubject(context.Kernel.Get<IDomainUnitOfWork>());
            SatisticsValue satisticsValue = new SatisticsValue(context.Kernel.Get<IAlgorithmOperation>(), context.Kernel.Get<IDomainUnitOfWork>());
            var virtualValueObject = new VirtualValueObserver(context.Kernel.Get<IDomainUnitOfWork>(), satisticsValue);
            departmentIndicatorValueSubject.Attach(virtualValueObject);
            return departmentIndicatorValueSubject;
        }
    }
}