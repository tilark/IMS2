using Microsoft.VisualStudio.TestTools.UnitTesting;
using IMS2.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMS2;
using System.Web.Mvc;
using IMS2.Models;
using IMS2.ViewModels;

namespace IMS2.Controllers.Tests
{
    [TestClass()]
    public class ProvidingDepartmentIndicatorControllerTests
    {
        [TestMethod()]
        public async Task SearchIndicatorTest()
        {
            // Arrange
            var controller = new ProvidingDepartmentIndicatorController();

            var searchTime = new DateTime(2016, 5, 1);
            var id = Guid.Parse("dccf942d-8ee8-4671-bf14-1ac826af431d");
            // Act
            ViewResult result = (await controller.SearchIndicator(searchTime, id)) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ViewBag.SearchTime);
        }
    }
}