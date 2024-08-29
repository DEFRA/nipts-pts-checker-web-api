using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Repositories.Implementation;
using Defra.PTS.Checker.Repositories.Interface;
using Defra.PTS.Checker.Services.Enums;
using Defra.PTS.Checker.Services.Implementation;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Defra.PTS.Checker.Services.Tests.Implementation
{
    [TestFixture]
    public class TravelDocumentServiceTests
    {
        private Mock<ITravelDocumentRepository>? _mockRepository;
        private TravelDocumentService? _service;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new Mock<ITravelDocumentRepository>();
            _service = new TravelDocumentService(_mockRepository.Object);
        }

        [Test]
        public async Task GetTravelDocumentByPTDNumber_ReturnsTravelDocument_WhenDocumentExists()
        {
            // Arrange
            string ptdNumber = "PTD123";
            var expectedTravelDocument = new TravelDocument
            {
                Id = Guid.NewGuid(),
                DocumentReferenceNumber = "REF123",
                DateOfIssue = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
                ValidityStartDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
                ValidityEndDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
                StatusId = 1,
                Pet = new Pet { Id = Guid.NewGuid(), Name = "Buddy" },
                Application = new Application { Id = Guid.NewGuid(), ReferenceNumber = "APP123" }
            };

            _mockRepository!.Setup(repo => repo.GetTravelDocumentByPTDNumber(ptdNumber))
                           .ReturnsAsync(expectedTravelDocument);

            // Act
            var result = await _service!.GetTravelDocumentByPTDNumber(ptdNumber);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(expectedTravelDocument, Is.EqualTo(result));
        }


        [Test]
        public async Task GetTravelDocumentByApplicationId_ReturnsTravelDocument_WhenDocumentExists()
        {
            // Arrange
            Guid applicationId = Guid.NewGuid();
            var expectedTravelDocument = new TravelDocument
            {
                Id = Guid.NewGuid(),
                DocumentReferenceNumber = "REF123",
                DateOfIssue = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
                ValidityStartDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
                ValidityEndDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
                StatusId = 1,
                Pet = new Pet { Id = Guid.NewGuid(), Name = "Buddy" },
                Application = new Application { Id = Guid.NewGuid(), ReferenceNumber = "APP123" }
            };

            _mockRepository!.Setup(repo => repo.GetTravelDocumentByApplicationIdAsync(applicationId))
                           .ReturnsAsync(expectedTravelDocument);

            // Act
            var result = await _service!.GetTravelDocumentByApplicationId(applicationId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(expectedTravelDocument, Is.EqualTo(result));
        }

    }
}
