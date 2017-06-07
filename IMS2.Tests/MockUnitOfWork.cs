using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using IMS2.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using IMS2.RepositoryAsync;
using IMS2.PublicOperations.Implements;

namespace IMS2.Tests
{
    public class MockUnitOfWork
    {
        public static List<IndicatorAlgorithm> IndicatorAlgorithmList;
        public static List<Indicator> IndicatorList;
        public static List<DepartmentIndicatorValue> DepartmentIndicatorValueList;
        public static List<Department> DepartmentList;
        public static List<Duration> DurationList { get; set; }
        public static List<DepartmentIndicatorDurationVirtualValue> DepartmentIndicatorDurationValueList { get; set; }
        public static DateTime[] yearTime = new DateTime[12];

        public static void InitialDataBase()
        {
            IndicatorAlgorithmList = new List<IndicatorAlgorithm>();
            IndicatorList = new List<Indicator>();
            DepartmentIndicatorValueList = new List<DepartmentIndicatorValue>();
            DepartmentList = new List<Department>();
            DurationList = new List<Duration>();
            DepartmentIndicatorDurationValueList = new List<DepartmentIndicatorDurationVirtualValue>();
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

        #region MockSet     

        public static Mock<IDomainUnitOfWork> SetupUnitOfWork()
        {
            var unitOfWork = new Mock<IDomainUnitOfWork>();
            var mockIndicator = SetupMockDbSet<Indicator>(IndicatorList);
            var mockDepartmentIndicatorValue = SetupMockDbSet<DepartmentIndicatorValue>(DepartmentIndicatorValueList);
            var mockDepartment = SetupMockDbSet<Department>(DepartmentList);
            var mockDuration = SetupMockDbSet<Duration>(DurationList);
            var mockIndicatorAlgorithm = SetupMockDbSet<IndicatorAlgorithm>(IndicatorAlgorithmList);
            var mockDepartmentIndicatorDurationValue = SetupMockDbSet<DepartmentIndicatorDurationVirtualValue>(DepartmentIndicatorDurationValueList);

            unitOfWork.Setup(m => m.Db.Set<Indicator>()).Returns(mockIndicator.Object);
            unitOfWork.Setup(m => m.Db.Set<DepartmentIndicatorValue>()).Returns(mockDepartmentIndicatorValue.Object);
            unitOfWork.Setup(m => m.Db.Set<Department>()).Returns(mockDepartment.Object);
            unitOfWork.Setup(m => m.Db.Set<Duration>()).Returns(mockDuration.Object);
            unitOfWork.Setup(m => m.Db.Set<IndicatorAlgorithm>()).Returns(mockIndicatorAlgorithm.Object);
            unitOfWork.Setup(m => m.Db.Set<DepartmentIndicatorDurationVirtualValue>()).Returns(mockDepartmentIndicatorDurationValue.Object);

            return unitOfWork;
        }

        public static Mock<DbSet<T>> SetupMockDbSet<T>(List<T> dataList) where T : class
        {
            var data = dataList.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IDbAsyncEnumerable<T>>()
               .Setup(m => m.GetAsyncEnumerator())
               .Returns(new TestDbAsyncEnumerator<T>(data.GetEnumerator()));

            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<T>(data.Provider));

            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return mockSet;
        }
        #endregion

        #region 初始化数据


        private static void InitialDepartmentList()
        {
            var department1 = new Department
            {
                DepartmentId = System.Guid.NewGuid(),
                DepartmentName = "科室1"
            };
            DepartmentList.Add(department1);

            var department2 = new Department
            {
                DepartmentId = System.Guid.NewGuid(),
                DepartmentName = "科室2"
            };
            DepartmentList.Add(department2);
        }

        private static void InitialDepartmentIndicatorValueList()
        {
            for (int i = 0; i < 12; i++)
            {
                var Y = new DepartmentIndicatorValue
                {
                    IndicatorId = IndicatorList.Find(a => a.IndicatorName == "Y").IndicatorId,
                    DepartmentId = DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                    Time = yearTime[i],
                    Value = 3.2M + Decimal.Parse(i.ToString()),
                    IsLocked = true
                };
                DepartmentIndicatorValueList.Add(Y);
            }
            for (int j = 0; j < 12; j++)
            {
                var Z = new DepartmentIndicatorValue
                {
                    IndicatorId = IndicatorList.Find(a => a.IndicatorName == "Z").IndicatorId,
                    DepartmentId = DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                    Time = yearTime[j],
                    Value = 3.2M + Decimal.Parse(j.ToString()),
                    IsLocked = true
                };
                DepartmentIndicatorValueList.Add(Z);
            }
        }

        private static void InitialIndicatorList()
        {
            var indicator1 = new Indicator
            {
                IndicatorId = Guid.ParseExact("{0C326BC3-6D95-4E63-BBBB-EABC0B0AE344}","B"),
                DutyDepartmentId = DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = CachedKeyEntry.MonthDurationID,
                IndicatorName = "X"
            };
            IndicatorList.Add(indicator1);
            var indicator2 = new Indicator
            {
                IndicatorId = Guid.ParseExact("{B2BDE3D8-BDAE-4549-A867-01981369A50C}","B"),
                DutyDepartmentId = DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = CachedKeyEntry.MonthDurationID,
                IndicatorName = "Y"
            };
            IndicatorList.Add(indicator2);

            var indicator3 = new Indicator
            {
                IndicatorId = Guid.ParseExact("{B4D4968E-EF53-43BC-AA0C-FDA749ED2061}", "B"),
                DutyDepartmentId = DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = CachedKeyEntry.MonthDurationID,
                IndicatorName = "Z"
            };
            IndicatorList.Add(indicator3);

            var indicator4 = new Indicator
            {
                IndicatorId = Guid.ParseExact("{963376CF-032D-436E-9B45-466C4610B50F}", "B"),
                DutyDepartmentId = DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = CachedKeyEntry.MonthDurationID,
                IndicatorName = "A"
            };
            IndicatorList.Add(indicator4);

            var indicator5 = new Indicator
            {
                IndicatorId = Guid.ParseExact("{5AD61CEB-4F99-4C81-8FC6-CD603C8A695E}", "B"),
                DutyDepartmentId = DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = CachedKeyEntry.MonthDurationID,
                IndicatorName = "B"
            };
            IndicatorList.Add(indicator5);

            var indicator6 = new Indicator
            {
                IndicatorId = Guid.ParseExact("{B9733E7D-87DD-4B04-956C-C22C24E93D00}", "B"),
                DutyDepartmentId = DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = CachedKeyEntry.MonthDurationID,
                IndicatorName = "C"
            };
            IndicatorList.Add(indicator6);
        }

        private static void InitialAlgorithmList()
        {
            var data1 = new IndicatorAlgorithm
            {
                ResultId = IndicatorList.Find(a => a.IndicatorName == "X").IndicatorId,
                FirstOperandID = IndicatorList.Find(a => a.IndicatorName == "Y").IndicatorId,
                SecondOperandID = IndicatorList.Find(a => a.IndicatorName == "Z").IndicatorId,
                OperationMethod = "division"
            };
            IndicatorAlgorithmList.Add(data1);

            var data2 = new IndicatorAlgorithm
            {
                ResultId = IndicatorList.Find(a => a.IndicatorName == "A").IndicatorId,
                FirstOperandID = IndicatorList.Find(a => a.IndicatorName == "Y").IndicatorId,
                SecondOperandID = IndicatorList.Find(a => a.IndicatorName == "Z").IndicatorId,
                OperationMethod = "multiplication"
            };
            IndicatorAlgorithmList.Add(data2);

            var data3 = new IndicatorAlgorithm
            {
                ResultId = IndicatorList.Find(a => a.IndicatorName == "B").IndicatorId,
                FirstOperandID = IndicatorList.Find(a => a.IndicatorName == "A").IndicatorId,
                SecondOperandID = IndicatorList.Find(a => a.IndicatorName == "Z").IndicatorId,
                OperationMethod = "division"
            };
            IndicatorAlgorithmList.Add(data3);

            var data4 = new IndicatorAlgorithm
            {
                ResultId = IndicatorList.Find(a => a.IndicatorName == "C").IndicatorId,
                FirstOperandID = IndicatorList.Find(a => a.IndicatorName == "A").IndicatorId,
                SecondOperandID = IndicatorList.Find(a => a.IndicatorName == "Z").IndicatorId,
                OperationMethod = "multiplication"
            };
            IndicatorAlgorithmList.Add(data4);

        }
        private static void InitialDuration()
        {
            var yearDuration = new Duration
            {
                DurationId = CachedKeyEntry.YearDurationID,
                DurationName = "年",
                Level = 4,
                Remarks = "年"
            };
            DurationList.Add(yearDuration);

            var halfYearDuration = new Duration
            {
                DurationId = CachedKeyEntry.HalftYearDurationID,
                DurationName = "半年",
                Level = 3,
                Remarks = "半年"
            };
            DurationList.Add(halfYearDuration);

            var seasonDuration = new Duration
            {
                DurationId = CachedKeyEntry.SeasonDurationID,
                DurationName = "季",
                Level = 2,
                Remarks = "季"
            };
            DurationList.Add(seasonDuration);
            var monthDuration = new Duration
            {
                DurationId = CachedKeyEntry.MonthDurationID,
                DurationName = "月",
                Level = 1,
                Remarks = "月"
            };
            DurationList.Add(monthDuration);
        }

        #endregion
    }
}
