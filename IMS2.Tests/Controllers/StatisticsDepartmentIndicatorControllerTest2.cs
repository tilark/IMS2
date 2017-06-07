using Microsoft.VisualStudio.TestTools.UnitTesting;
using IMS2.BusinessModel.SatisticsValueModel;
using IMS2.BusinessModel.AlgorithmModel;
using IMS2.Controllers;
using IMS2.BusinessModel.SatisticsValueModel;

namespace IMS2.Tests.Controllers
{
    [TestClass]
    public class StatisticsDepartmentIndicatorControllerTest2
    {     

        [TestInitialize]
        public void Initial2()
        {
            MockUnitOfWork.InitialDataBase();
        }     

      
        [TestMethod]
        public void CreateDepartmentIndicatorDurationValue()
        {
            var unitOfWork = MockUnitOfWork.SetupUnitOfWork();
            var controller = new StatisticsDepartmentIndicatorValueController(unitOfWork.Object, new SatisticsValue(new AlgorithmOperationImpl(), unitOfWork.Object));
            //测试创建Y的基本月的数据
            var test1 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = MockUnitOfWork.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = MockUnitOfWork.DurationList.Find(a => a.DurationName == "月").DurationId,
                IndicatorID = MockUnitOfWork.IndicatorList.Find(a => a.IndicatorName == "Y").IndicatorId,
                Time = MockUnitOfWork.yearTime[0]
            };

            //Act
            //var result = controller.Edit(test1);

            ////Assert
            //Assert.AreEqual(MockUnitOfWork.DepartmentIndicatorDurationValueList.Count, 1);
            //Assert.AreEqual(MockUnitOfWork.DepartmentIndicatorDurationValueList[0].Value, 3.2M);
        }
       
       
    }
}
