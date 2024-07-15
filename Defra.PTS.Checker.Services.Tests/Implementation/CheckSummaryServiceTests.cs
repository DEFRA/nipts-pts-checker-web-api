using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Models;
using Defra.PTS.Checker.Repositories.Interface;
using Defra.PTS.Checker.Services.Implementation;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Defra.PTS.Checker.Services.Tests.Implementation
{
    [TestFixture]
    public class CheckSummaryServiceTests
    {
        private Mock<IRepository<Route>> _sailingRepositoryMock;
        private Mock<ILogger<CheckSummaryService>> _loggerMock;
        private CheckSummaryService _service;

        [SetUp]
        public void SetUp()
        {
            var context = DataHelper.GetDbContext();

            _loggerMock = new Mock<ILogger<CheckSummaryService>>();
            _service = new CheckSummaryService(context, _loggerMock.Object);
        }

        [Test]
        public async Task SaveCheckSummary_ReturnsIdOnSave()
        {
            // Arrange
            var model = new CheckOutcomeModel
            {
                CheckerId = null,
                CheckOutcome = "Pass",
                PTDNumber = "GB826CD186E",
                RouteId = 1,
                SailingTime = DateTime.UtcNow,
            };

            // Act
            var result = await _service.SaveCheckSummary(model);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CheckSummaryId, Is.Not.Null);
        }
    }
}
