﻿using Defra.PTS.Checker.Models;
using Defra.PTS.Checker.Services.Implementation;
using NUnit.Framework;
using Polly;

namespace Defra.PTS.Checker.Services.Tests.Implementation
{
    [TestFixture]
    public class CheckSummaryServiceTests
    {
        private readonly CheckSummaryService _service = new (DataHelper.GetDbContext());

        [Test]
        public async Task SaveCheckSummary_ReturnsIdOnSave()
        {
            // Arrange
            var model = new CheckOutcomeModel
            {
                CheckerId = null,
                CheckOutcome = "Pass",
                ApplicationId = new Guid("FF0DF803-8033-4CF8-B877-AB69BEFE63D2"),
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
