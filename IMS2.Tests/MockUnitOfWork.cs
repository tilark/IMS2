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
        public static List<DepartmentIndicatorDurationVirtualValue> DepartmentIndicatorDurationVirtualValueList { get; set; }
        public static List<IndicatorGroup> IndicatorGroupList { get; set; }
        public static List<IndicatorGroupMapIndicator> IndicatorGroupMapIndicatorList { get; set; }
        public static List<DepartmentCategoryMapIndicatorGroup> DepartmentCategoryMapIndicatorGroupList { get; set; }
        public static List<DepartmentCategory> DepartmentCategoryList { get; set; }

        public static Mock<DbSet<DepartmentIndicatorDurationVirtualValue>> mockDepartmentIndicatorDurationVirtualValue { get; set; }

        public static DateTime[] yearTime = new DateTime[12];

        public static void InitialDataBase()
        {
            IndicatorAlgorithmList = new List<IndicatorAlgorithm>();
            IndicatorList = new List<Indicator>();
            DepartmentIndicatorValueList = new List<DepartmentIndicatorValue>();
            DepartmentList = new List<Department>();
            DurationList = new List<Duration>();
            DepartmentIndicatorDurationVirtualValueList = new List<DepartmentIndicatorDurationVirtualValue>();

            IndicatorGroupList = new List<IndicatorGroup>();
            IndicatorGroupMapIndicatorList = new List<IndicatorGroupMapIndicator>();
            DepartmentCategoryMapIndicatorGroupList = new List<DepartmentCategoryMapIndicatorGroup>();
            DepartmentCategoryList = new List<DepartmentCategory>();
            for (int i = 0; i < 12; i++)
            {
                yearTime[i] = new DateTime(2017, i + 1, 1);
            }

            InitialBaseDate();       
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
            //var mockDepartmentIndicatorDurationValue = SetupMockDbSet<DepartmentIndicatorDurationVirtualValue>(DepartmentIndicatorDurationVirtualValueList);
            mockDepartmentIndicatorDurationVirtualValue = SetupMockDbSet<DepartmentIndicatorDurationVirtualValue>(DepartmentIndicatorDurationVirtualValueList);
            unitOfWork.Setup(m => m.Db.Set<Indicator>()).Returns(mockIndicator.Object);
            unitOfWork.Setup(m => m.Db.Set<DepartmentIndicatorValue>()).Returns(mockDepartmentIndicatorValue.Object);
            unitOfWork.Setup(m => m.Db.Set<Department>()).Returns(mockDepartment.Object);
            unitOfWork.Setup(m => m.Db.Set<Duration>()).Returns(mockDuration.Object);
            unitOfWork.Setup(m => m.Db.Set<IndicatorAlgorithm>()).Returns(mockIndicatorAlgorithm.Object);
            unitOfWork.Setup(m => m.Db.Set<DepartmentIndicatorDurationVirtualValue>()).Returns(mockDepartmentIndicatorDurationVirtualValue.Object);

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
        private static void InitialBaseDate()
        {
            #region 初始化跨度表
            DurationList = new List<Duration>
            {
                new Duration
            {
                DurationId = CachedKeyEntry.YearDurationID,
                DurationName = "年",
                Level = 4,
                Remarks = "年"
            },
                new Duration
            {
                DurationId = CachedKeyEntry.HalftYearDurationID,
                DurationName = "半年",
                Level = 3,
                Remarks = "半年"
            },
                new Duration
            {
                DurationId = CachedKeyEntry.SeasonDurationID,
                DurationName = "季",
                Level = 2,
                Remarks = "季"
            },
                new Duration
            {
                DurationId = CachedKeyEntry.MonthDurationID,
                DurationName = "月",
                Level = 1,
                Remarks = "月"
            }
            };
            #endregion

            #region 初始化科室组
            var departmentCategoryList = new DepartmentCategory[]
          {
                new DepartmentCategory{ DepartmentCategoryId = Guid.ParseExact("{D26F0FE1-CF92-47E7-8080-E75E8C2D32D6}", "B"),  DepartmentCategoryName = "科室组1"},
                new DepartmentCategory{ DepartmentCategoryId = Guid.ParseExact("{DFAE2B17-5A5C-4A57-8D28-F7653DB8DB8D}", "B"),  DepartmentCategoryName = "科室组1"},
                 new DepartmentCategory{ DepartmentCategoryId = Guid.ParseExact("{E13B85A2-CE26-4E99-9B95-4B0691E3A4C7}", "B"),  DepartmentCategoryName = "科室组1"},
                new DepartmentCategory{ DepartmentCategoryId = Guid.ParseExact("{A36DFD9E-3F48-48D3-BA86-E97BCD66C2EA}", "B"),  DepartmentCategoryName = "科室组1"}
          };
            foreach (var item in departmentCategoryList)
            {
                DepartmentCategoryList.Add(item);
            }
            #endregion
            #region 初始化科室
            var departmentList = new Department[]
         {
                new Department{ DepartmentId = Guid.ParseExact("{5A806F21-8F31-4C88-B5FD-1FCFF2411508}", "B"), DepartmentName = "科室1"},
                new Department{DepartmentId = Guid.ParseExact("{195FD439-74EA-4D4E-8DC4-290191B79F7E}","B"), DepartmentName = "科室2"},
                new Department{DepartmentId = Guid.ParseExact("{36A1012E-5CF0-47DE-B42C-F7C8AF49547E}","B"), DepartmentName = "科室3"},
                new Department{DepartmentId = Guid.ParseExact("{6BD49534-0AF4-4CA8-B4D5-DC0C1735CA0E}","B"), DepartmentName = "科室4"}
         };
            foreach (var item in departmentList)
            {
                DepartmentList.Add(item);
            }
            #endregion

            #region 初始化指标组
            var indicatorGroupList = new IndicatorGroup[]
        {
                new IndicatorGroup{ IndicatorGroupId = Guid.ParseExact("{A841EA09-A61A-4289-B996-582F290D7DA3}", "B"), IndicatorGroupName = "指标组1" },
                 new IndicatorGroup{ IndicatorGroupId = Guid.ParseExact("{DE737640-3380-47FA-96B2-C2B2CAFA8290}", "B"), IndicatorGroupName = "指标组2"},
                  new IndicatorGroup{ IndicatorGroupId = Guid.ParseExact("{6C0CC7F6-AEDD-42A7-A824-1D4F3DDE8D37}", "B"), IndicatorGroupName = "指标组3"}
        };
            foreach (var item in indicatorGroupList)
            {
                IndicatorGroupList.Add(item);
            }
            #endregion 

            #region 初始化指标
            var indicatorList = new Indicator[]
          {
              new Indicator
            {
                IndicatorId = Guid.ParseExact("{0C326BC3-6D95-4E63-BBBB-EABC0B0AE344}", "B"),
                DutyDepartmentId = DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = CachedKeyEntry.MonthDurationID,
                IndicatorName = "X"
            },
              new Indicator
            {
                IndicatorId = Guid.ParseExact("{B2BDE3D8-BDAE-4549-A867-01981369A50C}", "B"),
                DutyDepartmentId = DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = CachedKeyEntry.MonthDurationID,
                IndicatorName = "Y"
            },
              new Indicator
            {
                IndicatorId = Guid.ParseExact("{B4D4968E-EF53-43BC-AA0C-FDA749ED2061}", "B"),
                DutyDepartmentId = DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = CachedKeyEntry.MonthDurationID,
                IndicatorName = "Z"
            },
              new Indicator
            {
                IndicatorId = Guid.ParseExact("{963376CF-032D-436E-9B45-466C4610B50F}", "B"),
                DutyDepartmentId = DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = CachedKeyEntry.MonthDurationID,
                IndicatorName = "A"
            },
               new Indicator
            {
                IndicatorId = Guid.ParseExact("{5AD61CEB-4F99-4C81-8FC6-CD603C8A695E}", "B"),
                DutyDepartmentId = DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = CachedKeyEntry.MonthDurationID,
                IndicatorName = "B"
            },
               new Indicator
            {
                IndicatorId = Guid.ParseExact("{B9733E7D-87DD-4B04-956C-C22C24E93D00}", "B"),
                DutyDepartmentId = DepartmentList.Find(a => a.DepartmentName == "科室1").DepartmentId,
                DurationId = CachedKeyEntry.MonthDurationID,
                IndicatorName = "C"
            }
          };
            foreach (var item in indicatorList)
            {
                IndicatorList.Add(item);
            }
            #endregion

            #region 初始化指标组指标
        //    var indicatorGroupMapIndicatorList = new IndicatorGroupMapIndicator[]
        //{
        //        new IndicatorGroupMapIndicator{ IndicatorGroupMapIndicatorId = Guid.ParseExact("{48666021-D65E-48D5-8FD8-DE96A2BA2E7F}","B"), IndicatorGroupId = IndicatorGroupList.Find(a => a.IndicatorGroupName== "指标组1").IndicatorGroupId, IndicatorId= IndicatorList.Find(a => a.IndicatorName== "指标1").IndicatorId }
        //};
        //    foreach (var item in indicatorGroupMapIndicatorList)
        //    {
        //        IndicatorGroupMapIndicatorList.Add(item);
        //    }
            #endregion

            #region 初始化科室指标值表
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

            //添加科室2的Y、Z值
            for (int i = 0; i < 12; i++)
            {
                var Y = new DepartmentIndicatorValue
                {
                    IndicatorId = IndicatorList.Find(a => a.IndicatorName == "Y").IndicatorId,
                    DepartmentId = DepartmentList.Find(a => a.DepartmentName == "科室2").DepartmentId,
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
                    DepartmentId = DepartmentList.Find(a => a.DepartmentName == "科室2").DepartmentId,
                    Time = yearTime[j],
                    Value = 3.2M + Decimal.Parse(j.ToString()),
                    IsLocked = true
                };
                DepartmentIndicatorValueList.Add(Z);
            }
            #endregion

            #region 初始化算法表
            IndicatorAlgorithmList = new List<IndicatorAlgorithm>
            {
                new IndicatorAlgorithm
            {
                ResultId = IndicatorList.Find(a => a.IndicatorName == "X").IndicatorId,
                FirstOperandID = IndicatorList.Find(a => a.IndicatorName == "Y").IndicatorId,
                SecondOperandID = IndicatorList.Find(a => a.IndicatorName == "Z").IndicatorId,
                OperationMethod = "division"
            },
                new IndicatorAlgorithm
            {
                ResultId = IndicatorList.Find(a => a.IndicatorName == "A").IndicatorId,
                FirstOperandID = IndicatorList.Find(a => a.IndicatorName == "Y").IndicatorId,
                SecondOperandID = IndicatorList.Find(a => a.IndicatorName == "Z").IndicatorId,
                OperationMethod = "multiplication"
            },
                new IndicatorAlgorithm
            {
                ResultId = IndicatorList.Find(a => a.IndicatorName == "B").IndicatorId,
                FirstOperandID = IndicatorList.Find(a => a.IndicatorName == "A").IndicatorId,
                SecondOperandID = IndicatorList.Find(a => a.IndicatorName == "Z").IndicatorId,
                OperationMethod = "division"
            },
                new IndicatorAlgorithm
            {
                ResultId = IndicatorList.Find(a => a.IndicatorName == "C").IndicatorId,
                FirstOperandID = IndicatorList.Find(a => a.IndicatorName == "A").IndicatorId,
                SecondOperandID = IndicatorList.Find(a => a.IndicatorName == "Z").IndicatorId,
                OperationMethod = "multiplication"
            }
            };

            #endregion
        }
        #endregion
    }
}
