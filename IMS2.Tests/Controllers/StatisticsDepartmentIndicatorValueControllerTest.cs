using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS2.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IMS2.Models;
using IMS2.PublicOperations.Implements;
using IMS2.ViewModels.StatisticsDepartmentIndicatorValueViews;
using Moq;
using IMS2.RepositoryAsync;

namespace IMS2.Tests.Controllers
{
    [TestClass]
    public class StatisticsDepartmentIndicatorValueControllerTest
    {
        private List<IndicatorAlgorithm> IndicatorAlgorithmList;
        private List<Indicator> IndicatorList;
        private List<DepartmentIndicatorValue> DepartmentIndicatorValueList;
        private List<Department> DepartmentList;
        private List<Duration> DurationList { get; set; }
        private DateTime testTime1 = DateTime.Parse("2017-01-01");
        private DateTime testTime2 = DateTime.Parse("2017-02-01");
        private DateTime testTime3 = DateTime.Parse("2017-03-01");
        private DateTime[] yearTime = new DateTime[12];
        [TestInitialize]
        /// <summary>
        /// 初始化测试数据
        /// </summary>
        public void InitialTestData()
        {
            this.IndicatorAlgorithmList = new List<IndicatorAlgorithm>();
            this.IndicatorList = new List<Indicator>();
            this.DepartmentIndicatorValueList = new List<DepartmentIndicatorValue>();
            this.DepartmentList = new List<Department>();
            this.DurationList = new List<Duration>();
            for (int i = 0; i < 12; i++)
            {
                yearTime[i] = new DateTime(2017, i + 1, 1);
            }
            //初始化跨度
            InitialDuration();
            //初始化科室表
            InitialDepartmentList();
            //初始化项目表
            InitialIndicatorList();
            //初始化值表
            InitialDepartmentIndicatorValueList();

            //初始化算法表
            InitialAlgorithmList();
        }



        [TestMethod]
        public void GetDepartmentIndicatorTimeValueTest()
        {
            //Arrange
            var unitOfWork = new Mock<IDomainUnitOfWork>();
            var controller = new StatisticsDepartmentIndicatorValueController(new AlgorithmOperationImpl(), unitOfWork.Object);
            controller.DepartmentIndicatorValueList = this.DepartmentIndicatorValueList;
            controller.DepartmentList = this.DepartmentList;
            controller.IndicatorAlgorithmList = this.IndicatorAlgorithmList;
            controller.IndicatorList = this.IndicatorList;
            controller.DurationList = this.DurationList;
            //Act

            //获得基本操作数的值

            var test1 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = this.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = this.DurationList.Find(a => a.DurationName == "月").DurationId,
                IndicatorID = this.IndicatorList.Find(a => a.IndicatorName == "Y").IndicatorId,
                Time = yearTime[0]
            };
            var test2 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = this.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = this.DurationList.Find(a => a.DurationName == "月").DurationId,
                IndicatorID = this.IndicatorList.Find(a => a.IndicatorName == "Z").IndicatorId,
                Time = yearTime[0]
            };
            //测试基本跨度月的当月计算值X=Y/Z
            var test3 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = this.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = this.DurationList.Find(a => a.DurationName == "月").DurationId,
                IndicatorID = this.IndicatorList.Find(a => a.IndicatorName == "X").IndicatorId,
                Time = yearTime[0]
            };
            //测试季的Y的总值
            var test4 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = this.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = this.DurationList.Find(a => a.DurationName == "季").DurationId,
                IndicatorID = this.IndicatorList.Find(a => a.IndicatorName == "Y").IndicatorId,
                Time = yearTime[0]
            };
            //测试季的X的总值 X=Y/Z
            var test5 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = this.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = this.DurationList.Find(a => a.DurationName == "季").DurationId,
                IndicatorID = this.IndicatorList.Find(a => a.IndicatorName == "X").IndicatorId,
                Time = yearTime[0]
            };
            //测试单月的乘法 A = Y*Z
            var test6 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = this.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = this.DurationList.Find(a => a.DurationName == "月").DurationId,
                IndicatorID = this.IndicatorList.Find(a => a.IndicatorName == "A").IndicatorId,
                Time = yearTime[0]
            };
            //测试第一季度的乘法 A = Y*Z
            var test7 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = this.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = this.DurationList.Find(a => a.DurationName == "季").DurationId,
                IndicatorID = this.IndicatorList.Find(a => a.IndicatorName == "A").IndicatorId,
                Time = yearTime[0]
            };

            //测试上半年的乘法 A = Y * Z
            var test8 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = this.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = this.DurationList.Find(a => a.DurationName == "半年").DurationId,
                IndicatorID = this.IndicatorList.Find(a => a.IndicatorName == "A").IndicatorId,
                Time = yearTime[0]
            };
            //测试全年的乘法 A = Y * Z
            var test9 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = this.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = this.DurationList.Find(a => a.DurationName == "年").DurationId,
                IndicatorID = this.IndicatorList.Find(a => a.IndicatorName == "A").IndicatorId,
                Time = yearTime[0]
            };

            //测试全年的除法 X = Y/Z
            var test10 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = this.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = this.DurationList.Find(a => a.DurationName == "年").DurationId,
                IndicatorID = this.IndicatorList.Find(a => a.IndicatorName == "X").IndicatorId,
                Time = yearTime[0]
            };

            //求基本月的B B=A/Z
            var test11 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = this.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = this.DurationList.Find(a => a.DurationName == "月").DurationId,
                IndicatorID = this.IndicatorList.Find(a => a.IndicatorName == "B").IndicatorId,
                Time = yearTime[0]
            };

            //求基本月的C C=A * Z
            var test12 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = this.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = this.DurationList.Find(a => a.DurationName == "月").DurationId,
                IndicatorID = this.IndicatorList.Find(a => a.IndicatorName == "C").IndicatorId,
                Time = yearTime[0]
            };

            //求第二季度B B=A/Z
            var test13 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = this.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = this.DurationList.Find(a => a.DurationName == "季").DurationId,
                IndicatorID = this.IndicatorList.Find(a => a.IndicatorName == "B").IndicatorId,
                Time = yearTime[3]
            };
            //求第二季度C C=A*Z
            var test14 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = this.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = this.DurationList.Find(a => a.DurationName == "季").DurationId,
                IndicatorID = this.IndicatorList.Find(a => a.IndicatorName == "C").IndicatorId,
                Time = yearTime[3]
            };
            //求下半年B B=A/Z
            var test15 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = this.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = this.DurationList.Find(a => a.DurationName == "半年").DurationId,
                IndicatorID = this.IndicatorList.Find(a => a.IndicatorName == "B").IndicatorId,
                Time = yearTime[6]
            };
            //求下半年C C=A*Z
            var test16 = new DepartmentIndicatorDurationTime
            {
                DepartmentId = this.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = this.DurationList.Find(a => a.DurationName == "半年").DurationId,
                IndicatorID = this.IndicatorList.Find(a => a.IndicatorName == "C").IndicatorId,
                Time = yearTime[6]
            };

            var value1 = controller.GetDepartmentIndicatorTimeValue(test1);
            var value2 = controller.GetDepartmentIndicatorTimeValue(test2);
            var value3 = controller.GetDepartmentIndicatorTimeValue(test3);
            var value4 = controller.GetDepartmentIndicatorTimeValue(test4);
            var value5 = controller.GetDepartmentIndicatorTimeValue(test5);
            var value6 = controller.GetDepartmentIndicatorTimeValue(test6);
            var value7 = controller.GetDepartmentIndicatorTimeValue(test7);
            var value8 = controller.GetDepartmentIndicatorTimeValue(test8);
            var value9 = controller.GetDepartmentIndicatorTimeValue(test9);
            var value10 = controller.GetDepartmentIndicatorTimeValue(test10);

            var value11 = controller.GetDepartmentIndicatorTimeValue(test11);
            var value12 = controller.GetDepartmentIndicatorTimeValue(test12);
            var value13 = controller.GetDepartmentIndicatorTimeValue(test13);
            var value14 = controller.GetDepartmentIndicatorTimeValue(test14);
            var value15 = controller.GetDepartmentIndicatorTimeValue(test15);
            var value16 = controller.GetDepartmentIndicatorTimeValue(test16);
            //Assert
            Assert.AreEqual(value1.Value, 3.2M);
            Assert.AreEqual(value2.Value, 3.2M);
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
            Assert.AreEqual(Decimal.Round(value15.Value , 2), 11.95M);
            Assert.AreEqual(Decimal.Round(value16.Value , 2), 10223.93M);



        }

        #region 初始化数据


        private void InitialDepartmentList()
        {
            var department1 = new Department
            {
                DepartmentId = System.Guid.NewGuid(),
                DepartmentName = "科室1"
            };
            this.DepartmentList.Add(department1);

            var department2 = new Department
            {
                DepartmentId = System.Guid.NewGuid(),
                DepartmentName = "科室2"
            };
            this.DepartmentList.Add(department2);
        }

        private void InitialDepartmentIndicatorValueList()
        {
            for (int i = 0; i < 12; i++)
            {
                var Y = new DepartmentIndicatorValue
                {
                    IndicatorId = this.IndicatorList.Find(a => a.IndicatorName == "Y").IndicatorId,
                    DepartmentId = this.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                    Time = yearTime[i],
                    Value = 3.2M + Decimal.Parse(i.ToString()),
                    IsLocked = true
                };
                this.DepartmentIndicatorValueList.Add(Y);
            }
            for (int j = 0; j < 12; j++)
            {
                var Z = new DepartmentIndicatorValue
                {
                    IndicatorId = this.IndicatorList.Find(a => a.IndicatorName == "Z").IndicatorId,
                    DepartmentId = this.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                    Time = yearTime[j],
                    Value = 3.2M + Decimal.Parse(j.ToString()),
                    IsLocked = true
                };
                this.DepartmentIndicatorValueList.Add(Z);
            }
        }

        private void InitialIndicatorList()
        {
            var indicator1 = new Indicator
            {
                IndicatorId = System.Guid.NewGuid(),
                DutyDepartmentId = this.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = this.DurationList.Find(a => a.DurationName == "月").DurationId,
                IndicatorName = "X"
            };
            this.IndicatorList.Add(indicator1);
            var indicator2 = new Indicator
            {
                IndicatorId = System.Guid.NewGuid(),
                DutyDepartmentId = this.DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = this.DurationList.Find(a => a.DurationName == "月").DurationId,
                IndicatorName = "Y"
            };
            this.IndicatorList.Add(indicator2);

            var indicator3 = new Indicator
            {
                IndicatorId = System.Guid.NewGuid(),
                DutyDepartmentId = System.Guid.NewGuid(),
                DurationId = this.DurationList.Find(a => a.DurationName == "月").DurationId,
                IndicatorName = "Z"
            };
            this.IndicatorList.Add(indicator3);

            var indicator4 = new Indicator
            {
                IndicatorId = System.Guid.NewGuid(),
                DutyDepartmentId = System.Guid.NewGuid(),
                DurationId = this.DurationList.Find(a => a.DurationName == "月").DurationId,
                IndicatorName = "A"
            };
            this.IndicatorList.Add(indicator4);

            var indicator5 = new Indicator
            {
                IndicatorId = System.Guid.NewGuid(),
                DutyDepartmentId = System.Guid.NewGuid(),
                DurationId = this.DurationList.Find(a => a.DurationName == "月").DurationId,
                IndicatorName = "B"
            };
            this.IndicatorList.Add(indicator5);

            var indicator6 = new Indicator
            {
                IndicatorId = System.Guid.NewGuid(),
                DutyDepartmentId = System.Guid.NewGuid(),
                DurationId = this.DurationList.Find(a => a.DurationName == "月").DurationId,
                IndicatorName = "C"
            };
            this.IndicatorList.Add(indicator6);
        }

        private void InitialAlgorithmList()
        {
            var data1 = new IndicatorAlgorithm
            {
                ResultId = this.IndicatorList.Find(a => a.IndicatorName == "X").IndicatorId,
                FirstOperandID = this.IndicatorList.Find(a => a.IndicatorName == "Y").IndicatorId,
                SecondOperandID = this.IndicatorList.Find(a => a.IndicatorName == "Z").IndicatorId,
                OperationMethod = "division"
            };
            this.IndicatorAlgorithmList.Add(data1);

            var data2 = new IndicatorAlgorithm
            {
                ResultId = this.IndicatorList.Find(a => a.IndicatorName == "A").IndicatorId,
                FirstOperandID = this.IndicatorList.Find(a => a.IndicatorName == "Y").IndicatorId,
                SecondOperandID = this.IndicatorList.Find(a => a.IndicatorName == "Z").IndicatorId,
                OperationMethod = "multiplication"
            };
            this.IndicatorAlgorithmList.Add(data2);

            var data3 = new IndicatorAlgorithm
            {
                ResultId = this.IndicatorList.Find(a => a.IndicatorName == "B").IndicatorId,
                FirstOperandID = this.IndicatorList.Find(a => a.IndicatorName == "A").IndicatorId,
                SecondOperandID = this.IndicatorList.Find(a => a.IndicatorName == "Z").IndicatorId,
                OperationMethod = "division"
            };
            this.IndicatorAlgorithmList.Add(data3);

            var data4 = new IndicatorAlgorithm
            {
                ResultId = this.IndicatorList.Find(a => a.IndicatorName == "C").IndicatorId,
                FirstOperandID = this.IndicatorList.Find(a => a.IndicatorName == "A").IndicatorId,
                SecondOperandID = this.IndicatorList.Find(a => a.IndicatorName == "Z").IndicatorId,
                OperationMethod = "multiplication"
            };
            this.IndicatorAlgorithmList.Add(data4);

        }
        private void InitialDuration()
        {
            var yearDuration = new Duration
            {
                DurationId = System.Guid.NewGuid(),
                DurationName = "年",
                Level = 4,
                Remarks = "年"
            };
            this.DurationList.Add(yearDuration);

            var halfYearDuration = new Duration
            {
                DurationId = System.Guid.NewGuid(),
                DurationName = "半年",
                Level = 3,
                Remarks = "半年"
            };
            this.DurationList.Add(halfYearDuration);

            var seasonDuration = new Duration
            {
                DurationId = System.Guid.NewGuid(),
                DurationName = "季",
                Level = 2,
                Remarks = "季"
            };
            this.DurationList.Add(seasonDuration);
            var monthDuration = new Duration
            {
                DurationId = System.Guid.NewGuid(),
                DurationName = "月",
                Level = 1,
                Remarks = "月"
            };
            this.DurationList.Add(monthDuration);
        }

        #endregion
    }
}
