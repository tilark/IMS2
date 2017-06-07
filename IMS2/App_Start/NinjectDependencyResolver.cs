using System;
using System.Collections.Generic;
using Ninject;
using IMS2.Controllers;
using IMS2.PublicOperations;
using ExcelEntityOperation;
using IMS2.RepositoryAsync;
using IMS2.BusinessModel.SatisticsValueModel;
using IMS2.BusinessModel.AlgorithmModel;
namespace IMS2.App_Start
{
    public class NinjectDependencyResolver : System.Web.Mvc.IDependencyResolver
    {
        private readonly IKernel kernel;
        public NinjectDependencyResolver()
        {
            this.kernel = new StandardKernel();
            this.AddBindings();
        }

        private void AddBindings()
        {
            
            this.kernel.Bind<IDomainUnitOfWork>().To<DomainUnitOfWork>();
            this.kernel.Bind<IFileUpload>().To<UploadFilesController>();
            this.kernel.Bind<IReadFromExcel>().To<ReadFromExcel>();
            this.kernel.Bind<IAlgorithmOperation>().To<AlgorithmOperationImpl>();
            this.kernel.Bind<ISatisticsValue>().To<SatisticsValue>();
            ////this.kernel.Bind<ITodoRepository>().To<TodoRepository1>().Named("type1");
            ////this.kernel.Bind<ITodoRepository>().To<TodoRepository2>().Named("type2");
            ////this.kernel.Bind<IMessage>().To<Message1>().Named("message1");
        }

        public object GetService(Type serviceType)
        {
            return this.kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return this.kernel.GetAll(serviceType);
        }
    }

}