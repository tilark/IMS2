using Microsoft.VisualStudio.TestTools.UnitTesting;
using IMS2.BusinessModel.IndicatorDepartmentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS2.Tests;

namespace IMS2.BusinessModel.IndicatorDepartmentModel.Tests
{
    [TestClass()]
    public class IndicatorDepartmentImplTests
    {
        [TestInitialize]
        public void Initial()
        {
            MockUnitOfWork.InitialDataBase();
        }
        [TestMethod()]
        public void IndicatorDepartmentImplTest()
        {
            var unitOfWork = MockUnitOfWork.SetupUnitOfWork();
            Assert.Fail();
        }
    }
}