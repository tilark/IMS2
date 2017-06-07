using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IMS2.BusinessModel.AlgorithmModel;
using IMS2.BusinessModel.SatisticsValueModel;
using IMS2.ViewModels.StatisticsDepartmentIndicatorValueViews;
namespace IMS2.Tests.BusinessModel.SatisticsModels
{


    [TestClass]
    public class TestSatisticsValue
    {
        [TestInitialize]
        public void Initial()
        {
            MockUnitOfWork.InitialDataBase();
        }

        [TestMethod]
        public async Task TestGetSatisticsValue()
        {
            var unitOfWork = MockUnitOfWork.SetupUnitOfWork();
            var satistics = new SatisticsValue(new AlgorithmOperationImpl(), unitOfWork.Object);
            //Act

            //获得基本操作数的值

            //    //Act

            //    //获得基本操作数的值

            var test1 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = MockUnitOfWork.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = MockUnitOfWork.DurationList.Find(a => a.DurationName == "月").DurationId,
                IndicatorID = MockUnitOfWork.IndicatorList.Find(a => a.IndicatorName == "Y").IndicatorId,
                Time = MockUnitOfWork.yearTime[0]
            };
            var test2 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = MockUnitOfWork.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = MockUnitOfWork.DurationList.Find(a => a.DurationName == "月").DurationId,
                IndicatorID = MockUnitOfWork.IndicatorList.Find(a => a.IndicatorName == "Z").IndicatorId,
                Time = MockUnitOfWork.yearTime[0]
            };
            //测试基本跨度月的当月计算值X=Y/Z
            var test3 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = MockUnitOfWork.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = MockUnitOfWork.DurationList.Find(a => a.DurationName == "月").DurationId,
                IndicatorID = MockUnitOfWork.IndicatorList.Find(a => a.IndicatorName == "X").IndicatorId,
                Time = MockUnitOfWork.yearTime[0]
            };
            //测试季的Y的总值
            var test4 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = MockUnitOfWork.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = MockUnitOfWork.DurationList.Find(a => a.DurationName == "季").DurationId,
                IndicatorID = MockUnitOfWork.IndicatorList.Find(a => a.IndicatorName == "Y").IndicatorId,
                Time = MockUnitOfWork.yearTime[0]
            };
            //测试季的X的总值 X=Y/Z
            var test5 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = MockUnitOfWork.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = MockUnitOfWork.DurationList.Find(a => a.DurationName == "季").DurationId,
                IndicatorID = MockUnitOfWork.IndicatorList.Find(a => a.IndicatorName == "X").IndicatorId,
                Time = MockUnitOfWork.yearTime[0]
            };
            //测试单月的乘法 A = Y*Z
            var test6 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = MockUnitOfWork.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = MockUnitOfWork.DurationList.Find(a => a.DurationName == "月").DurationId,
                IndicatorID = MockUnitOfWork.IndicatorList.Find(a => a.IndicatorName == "A").IndicatorId,
                Time = MockUnitOfWork.yearTime[0]
            };
            //测试第一季度的乘法 A = Y*Z
            var test7 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = MockUnitOfWork.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = MockUnitOfWork.DurationList.Find(a => a.DurationName == "季").DurationId,
                IndicatorID = MockUnitOfWork.IndicatorList.Find(a => a.IndicatorName == "A").IndicatorId,
                Time = MockUnitOfWork.yearTime[0]
            };

            //测试上半年的乘法 A = Y * Z
            var test8 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = MockUnitOfWork.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = MockUnitOfWork.DurationList.Find(a => a.DurationName == "半年").DurationId,
                IndicatorID = MockUnitOfWork.IndicatorList.Find(a => a.IndicatorName == "A").IndicatorId,
                Time = MockUnitOfWork.yearTime[0]
            };
            //测试全年的乘法 A = Y * Z
            var test9 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = MockUnitOfWork.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = MockUnitOfWork.DurationList.Find(a => a.DurationName == "年").DurationId,
                IndicatorID = MockUnitOfWork.IndicatorList.Find(a => a.IndicatorName == "A").IndicatorId,
                Time = MockUnitOfWork.yearTime[0]
            };

            //测试全年的除法 X = Y/Z
            var test10 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = MockUnitOfWork.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = MockUnitOfWork.DurationList.Find(a => a.DurationName == "年").DurationId,
                IndicatorID = MockUnitOfWork.IndicatorList.Find(a => a.IndicatorName == "X").IndicatorId,
                Time = MockUnitOfWork.yearTime[0]
            };

            //求基本月的B B=A/Z
            var test11 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = MockUnitOfWork.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = MockUnitOfWork.DurationList.Find(a => a.DurationName == "月").DurationId,
                IndicatorID = MockUnitOfWork.IndicatorList.Find(a => a.IndicatorName == "B").IndicatorId,
                Time = MockUnitOfWork.yearTime[0]
            };

            //求基本月的C C=A * Z
            var test12 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = MockUnitOfWork.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = MockUnitOfWork.DurationList.Find(a => a.DurationName == "月").DurationId,
                IndicatorID = MockUnitOfWork.IndicatorList.Find(a => a.IndicatorName == "C").IndicatorId,
                Time = MockUnitOfWork.yearTime[0]
            };

            //求第二季度B B=A/Z
            var test13 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = MockUnitOfWork.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = MockUnitOfWork.DurationList.Find(a => a.DurationName == "季").DurationId,
                IndicatorID = MockUnitOfWork.IndicatorList.Find(a => a.IndicatorName == "B").IndicatorId,
                Time = MockUnitOfWork.yearTime[3]
            };
            //求第二季度C C=A*Z
            var test14 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = MockUnitOfWork.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = MockUnitOfWork.DurationList.Find(a => a.DurationName == "季").DurationId,
                IndicatorID = MockUnitOfWork.IndicatorList.Find(a => a.IndicatorName == "C").IndicatorId,
                Time = MockUnitOfWork.yearTime[3]
            };
            //求下半年B B=A/Z
            var test15 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = MockUnitOfWork.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = MockUnitOfWork.DurationList.Find(a => a.DurationName == "半年").DurationId,
                IndicatorID = MockUnitOfWork.IndicatorList.Find(a => a.IndicatorName == "B").IndicatorId,
                Time = MockUnitOfWork.yearTime[6]
            };
            //求下半年C C=A*Z
            var test16 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = MockUnitOfWork.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = MockUnitOfWork.DurationList.Find(a => a.DurationName == "半年").DurationId,
                IndicatorID = MockUnitOfWork.IndicatorList.Find(a => a.IndicatorName == "C").IndicatorId,
                Time = MockUnitOfWork.yearTime[6]
            };
            var value1 = await satistics.GetSatisticsValue(test1);
            var value2 = await satistics.GetSatisticsValue(test2);
            var value3 = await satistics.GetSatisticsValue(test3);
            var value4 = await satistics.GetSatisticsValue(test4);
            var value5 = await satistics.GetSatisticsValue(test5);
            var value6 = await satistics.GetSatisticsValue(test6);
            var value7 = await satistics.GetSatisticsValue(test7);
            var value8 = await satistics.GetSatisticsValue(test8);
            var value9 = await satistics.GetSatisticsValue(test9);
            var value10 = await satistics.GetSatisticsValue(test10);

            var value11 = await satistics.GetSatisticsValue(test11);
            var value12 = await satistics.GetSatisticsValue(test12);
            var value13 = await satistics.GetSatisticsValue(test13);
            var value14 = await satistics.GetSatisticsValue(test14);
            var value15 = await satistics.GetSatisticsValue(test15);
            var value16 = await satistics.GetSatisticsValue(test16);
            //Assert
            Assert.AreEqual(value1.HasValue ? value1.Value : 0M, 3.2M);
            Assert.AreEqual(value2.HasValue ? value2.Value : 0M, 3.2M);
            Assert.AreEqual(value3.Value, 3.2M / 3.2M);
            Assert.AreEqual(value4.Value, 12.6M);
            Assert.AreEqual(value5.Value, (3.2M + 4.2M + 5.2M) / (3.2M + 4.2M + 5.2M));
            Assert.AreEqual(value6.Value, 3.2M * 3.2M);
            Assert.AreEqual(value7.Value, 3.2M * 3.2M + 4.2M * 4.2M + 5.2M * 5.2M);
            Assert.AreEqual(value8.Value, 3.2M * 3.2M + 4.2M * 4.2M + 5.2M * 5.2M + 6.2M * 6.2M + 7.2M * 7.2M + 8.2M * 8.2M);
            Assert.AreEqual(value9.Value, 1051.28M);
            Assert.AreEqual(value10.Value, 1M);
            Assert.AreEqual(value11.Value, 3.2M);
            Assert.AreEqual(value12.Value, 32.768M);
            Assert.AreEqual(Decimal.Round(value13.Value, 2), 7.29M);
            Assert.AreEqual(Decimal.Round(value14.Value, 2), 1162.94M);
            Assert.AreEqual(Decimal.Round(value15.Value, 2), 11.95M);
            Assert.AreEqual(Decimal.Round(value16.Value, 2), 10223.93M);

        }
    }
}
