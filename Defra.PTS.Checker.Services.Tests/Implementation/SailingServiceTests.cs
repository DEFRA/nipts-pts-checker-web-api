using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Repositories.Interface;
using Defra.PTS.Checker.Services.Implementation;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Defra.PTS.Checker.Services.Tests.Implementation
{
    [TestFixture]
    public class SailingServiceTests
    {
        private Mock<IRepository<Route>> _sailingRepositoryMock;
        private Mock<ILogger<SailingService>> _loggerMock;
        private SailingService _sailingService;

        [SetUp]
        public void SetUp()
        {
            _sailingRepositoryMock = new Mock<IRepository<Route>>();
            _loggerMock = new Mock<ILogger<SailingService>>();
            _sailingService = new SailingService(_loggerMock.Object, _sailingRepositoryMock.Object);
        }

        [Test]
        public async Task GetAllSailingRoutes_ReturnsListOfRouteResponses()
        {
            // Arrange
            var routes = new List<Route>
            {
                new Route { Id = 1, RouteName = "Route A" },
                new Route { Id = 2, RouteName = "Route B" }
            };

            _sailingRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(routes);

            // Act
            var result = await _sailingService.GetAllSailingRoutes();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Exactly(2).Items);
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().RouteName, Is.EqualTo("Route A"));
            Assert.That(result.Last().Id, Is.EqualTo(2));
            Assert.That(result.Last().RouteName, Is.EqualTo("Route B"));
        }

        [Test]
        public async Task GetAllSailingRoutes_ReturnsEmptyList_WhenNoRoutes()
        {
            // Arrange
            var routes = new List<Route>();
            _sailingRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(routes);

            // Act
            var result = await _sailingService.GetAllSailingRoutes();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }
    }
}
