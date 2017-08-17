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
        //protected override DepartmentIndicatorValueSubject CreateInstance(IContext context)
        //{
        //    var departmentIndicatorValueSubject = new DepartmentIndicatorValueSubject(context.Kernel.Get<IDomainUnitOfWork>());
        //    var unitOfWork = context.Kernel.Get<IDomainUnitOfWork>();
        //    SatisticsValueUseDbContext satisticsValue = new SatisticsValueUseDbContext(context.Kernel.Get<IAlgorithmOperation>());
        //    var virtualValueObject = new VirtualValueObserver(unitOfWork, satisticsValue);
        //    departmentIndicatorValueSubject.Attach(virtualValueObject);
        //    return departmentIndicatorValueSubject;
        //}

        protected override DepartmentIndicatorValueSubject CreateInstance(IContext context)
        {
            var departmentIndicatorValueSubject = new DepartmentIndicatorValueSubject();
            SatisticsValueUseDbContext satisticsValue = new SatisticsValueUseDbContext(context.Kernel.Get<IAlgorithmOperation>());
            var virtualValueObject = new VirtualValueObserver( satisticsValue);
            departmentIndicatorValueSubject.Attach(virtualValueObject);
            return departmentIndicatorValueSubject;
        }
    }
}