using Microsoft.VisualStudio.TestTools.UnitTesting;
using IMS2.BusinessModel.SatisticsValueModel;
using IMS2.BusinessModel.AlgorithmModel;
using IMS2.Controllers;
using IMS2.BusinessModel.IndicatorDepartmentModel;
using IMS2.Tests;
using System.Collections.Generic;
using System;
using Moq;
using System.Threading.Tasks;
using IMS2.ViewModels.StatisticsDepartmentIndicatorValueViews;
using IMS2.Models;

namespace IMS2.Controllers.Tests
{
    [TestClass()]
    public class StatisticsDepartmentIndicatorValueControllerTests
    {
        [TestInitialize]
        public void Initial2()
        {
            MockUnitOfWork.InitialDataBase();
        }

        /// <summary>
        /// 页面端输入
        /// </summary>
        [TestMethod]
        public void TestGetDepartmentIndicatorDurationValueFromUI()
        {
            //页面端只输入指标提供科室和时间和时段，计算出所有的科室指标值，写入新值表中
        }
        [TestMethod]
        public void CreateDepartmentIndicatorDurationValue()
        {
            var unitOfWork = MockUnitOfWork.SetupUnitOfWork();
            var controller = new StatisticsDepartmentIndicatorValueController(unitOfWork.Object, new SatisticsValue(new AlgorithmOperationImpl(), unitOfWork.Object), new IndicatorDepartmentImpl(unitOfWork.Object));
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
        /// <summary>
        /// 测试获取指标科室组，针对每个指标、科室，跨度，时间获取相对应的值。
        /// </summary>
        /// <returns></returns>
        [TestMethod()]
        public async Task _CaculateAlgorithmResultTest()
        {
            var unitOfWork = MockUnitOfWork.SetupUnitOfWork();
            var mockIndicatorDepartment = new Mock<IIndicatorDepartment>();

            mockIndicatorDepartment.Setup(a => a.GetAlgorithmIndicatorDepartment()).Returns(GetTestIndicatorDepartment());
            var controller = new StatisticsDepartmentIndicatorValueController(unitOfWork.Object, new SatisticsValue(new AlgorithmOperationImpl(), unitOfWork.Object), mockIndicatorDepartment.Object);

            var testValue = new DepartmentIndicatorDurationVirtualValueEdit
            {
                DurationId = CachedKeyEntry.SeasonDurationID,
                Time = new DateTime(2017, 1, 1)
            };
            var resultAction = await controller._CaculateAlgorithmResult(testValue);

            MockUnitOfWork.mockDepartmentIndicatorDurationVirtualValue.Verify(m => m.Add(It.IsAny<DepartmentIndicatorDurationVirtualValue>()), Times.Exactly(2));
            unitOfWork.Verify(m => m.SaveChangesClientWinAsync(), Times.AtLeastOnce());
            //Assert.Fail();
        }

        private async Task<List<IndicatorDepartment>> GetTestIndicatorDepartment()
        {
            List<Guid> departmentIDList = new List<Guid>();
            var result = new List<IndicatorDepartment>();
            await Task.Run(() =>
            {
                foreach (var item in MockUnitOfWork.DepartmentList)
                {
                    var id = item.DepartmentId;
                    departmentIDList.Add(id);
                }
                result = new List<IndicatorDepartment> {
                new IndicatorDepartment{ IndicatorID = MockUnitOfWork.IndicatorList.Find(a => a.IndicatorName == "Y").IndicatorId,
                  DepartmentIDList = departmentIDList}
            };
            });        
           
            return result;         

        }
    }
}