using Defra.PTS.Checker.Models;
using Defra.PTS.Checker.Services.Interface;
using Defra.PTS.Checker.Web.Api.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.PTS.Checker.Web.Api.Tests.Controllers
{
    [TestFixture]
    public class SailingControllerTests
    {
        private Mock<ISailingService> _sailingServiceMock;
        private SailingController _controller;

        [SetUp]
        public void SetUp()
        {
            _sailingServiceMock = new Mock<ISailingService>();
            _controller = new SailingController(_sailingServiceMock.Object);
        }

        [Test]
        public async Task GetAllSailingRoutes_ReturnsOkResult_WithListOfRoutes()
        {
            // Arrange
            var routes = new List<RouteResponse>
            {
                new RouteResponse { Id = 1, RouteName = "Route A" },
                new RouteResponse { Id = 2, RouteName = "Route B" }
            };

            _sailingServiceMock.Setup(s => s.GetAllSailingRoutes()).ReturnsAsync(routes);

            // Act
            var result = await _controller.GetAllSailingRoutes();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(routes));
        }

        [Test]
        public async Task GetAllSailingRoutes_ReturnsNotFound_WhenNoRoutes()
        {
            // Arrange
            _sailingServiceMock.Setup(s => s.GetAllSailingRoutes()).ReturnsAsync((IEnumerable<RouteResponse>)null);

            // Act
            var result = await _controller.GetAllSailingRoutes();

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
            var notFoundResult = result as NotFoundResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
        }
    }
}
