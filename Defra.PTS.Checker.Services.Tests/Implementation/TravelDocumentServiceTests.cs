using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Repositories.Implementation;
using Defra.PTS.Checker.Repositories.Interface;
using Defra.PTS.Checker.Services.Implementation;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.PTS.Checker.Services.Tests.Implementation
{
    [TestFixture]
    public class TravelDocumentServiceTests
    {
        private Mock<ITravelDocumentRepository>? _travelDocumentRepositoryMock;
        private TravelDocumentService? _travelDocumentService;

        [SetUp]
        public void SetUp()
        {
            _travelDocumentRepositoryMock = new Mock<ITravelDocumentRepository>();
            _travelDocumentService = new TravelDocumentService(_travelDocumentRepositoryMock.Object);
        }

        [Test]
        public async Task GetTravelDocumentByReferenceNumber_ReturnsDocument()
        {
            // Arrange
            var guid = Guid.Parse("F567CDDA-DC72-4865-C18A-08DC12AE079D");
            var date = DateTime.Now;
            var qr = new byte[] { 1 };

            var pet = new Pet
            {
                Name = "Kitsu"
            };

            var owner = new Owner
            {
                FullName = "Dean",
                CharityName = "Special Effect"
            };

            var application = new Application
            {
                Id = guid
            };


            var travelDocument = new TravelDocument
            {
                Id = guid,
                DocumentReferenceNumber = "GB123",
                IssuingAuthorityId = 2,
                CreatedBy = guid,
                DateOfIssue = date,
                CreatedOn = date,
                UpdatedOn = date,
                ApplicationId = guid,
                PetId = guid,
                DocumentSignedBy = "test",
                IsLifeTime = true,
                UpdatedBy = guid,
                OwnerId = guid,
                StatusId = 3,
                ValidityEndDate = date,
                ValidityStartDate = date,
                QrCode = qr,
                Application = application, 
                Owner = owner,
                Pet = pet
            };

            _travelDocumentRepositoryMock!.Setup(repo => repo.GetTravelDocumentByPTDNumber(It.IsAny<string>())).ReturnsAsync(travelDocument);

            // Act
            var result = await _travelDocumentService!.GetTravelDocumentByPTDNumber("GB123");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(guid));
            Assert.That(result.PetId, Is.EqualTo(guid));
            Assert.That(result.ApplicationId, Is.EqualTo(guid));
            Assert.That(result.OwnerId, Is.EqualTo(guid));
            Assert.That(result.StatusId, Is.EqualTo(3));
            Assert.That(result.IssuingAuthorityId, Is.EqualTo(2));
            Assert.That(result.DocumentReferenceNumber, Is.EqualTo("GB123"));
            Assert.That(result.Owner, Is.EqualTo(owner));
            Assert.That(result.Pet, Is.EqualTo(pet));
            Assert.That(result.Application, Is.EqualTo(application));
            Assert.That(result.DocumentSignedBy, Is.EqualTo("test"));
            Assert.That(result.CreatedBy, Is.EqualTo(guid));
            Assert.That(result.DateOfIssue, Is.EqualTo(date));
            Assert.That(result.CreatedOn, Is.EqualTo(date));
            Assert.That(result.UpdatedOn, Is.EqualTo(date));
            Assert.That(result.IsLifeTime, Is.EqualTo(true));
            Assert.That(result.UpdatedBy, Is.EqualTo(guid));
            Assert.That(result.ValidityEndDate, Is.EqualTo(date));
            Assert.That(result.ValidityStartDate, Is.EqualTo(date));
            Assert.That(result.QrCode, Is.EqualTo(qr));
        }
        [Test]
        public async Task GetTravelDocumentByPTDNumber_ReturnsDocument()
        {
            // Arrange
            var guid = Guid.Parse("F567CDDA-DC72-4865-C18A-08DC12AE079D");
            var date = DateTime.Now;
            var qr = new byte[] { 1 };

            var pet = new Pet
            {
                Name = "Kitsu"
            };

            var owner = new Owner
            {
                FullName = "Dean",
                CharityName = "Special Effect"
            };

            var application = new Application
            {
                Id = guid
            };


            var travelDocument = new TravelDocument
            {
                Id = guid,
                DocumentReferenceNumber = "GB123",
                IssuingAuthorityId = 2,
                CreatedBy = guid,
                DateOfIssue = date,
                CreatedOn = date,
                UpdatedOn = date,
                ApplicationId = guid,
                PetId = guid,
                DocumentSignedBy = "test",
                IsLifeTime = true,
                UpdatedBy = guid,
                OwnerId = guid,
                StatusId = 3,
                ValidityEndDate = date,
                ValidityStartDate = date,
                QrCode = qr,
                Application = application,
                Owner = owner,
                Pet = pet
            };

            _travelDocumentRepositoryMock!.Setup(repo => repo.GetTravelDocumentByPTDNumber(It.IsAny<string>())).ReturnsAsync(travelDocument);

            // Act
            var result = await _travelDocumentService!.GetTravelDocumentByPTDNumber("GB123");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(guid));
            Assert.That(result.PetId, Is.EqualTo(guid));
            Assert.That(result.ApplicationId, Is.EqualTo(guid));
            Assert.That(result.OwnerId, Is.EqualTo(guid));
            Assert.That(result.StatusId, Is.EqualTo(3));
            Assert.That(result.IssuingAuthorityId, Is.EqualTo(2));
            Assert.That(result.DocumentReferenceNumber, Is.EqualTo("GB123"));
            Assert.That(result.Owner, Is.EqualTo(owner));
            Assert.That(result.Pet, Is.EqualTo(pet));
            Assert.That(result.Application, Is.EqualTo(application));
            Assert.That(result.DocumentSignedBy, Is.EqualTo("test"));
            Assert.That(result.CreatedBy, Is.EqualTo(guid));
            Assert.That(result.DateOfIssue, Is.EqualTo(date));
            Assert.That(result.CreatedOn, Is.EqualTo(date));
            Assert.That(result.UpdatedOn, Is.EqualTo(date));
            Assert.That(result.IsLifeTime, Is.EqualTo(true));
            Assert.That(result.UpdatedBy, Is.EqualTo(guid));
            Assert.That(result.ValidityEndDate, Is.EqualTo(date));
            Assert.That(result.ValidityStartDate, Is.EqualTo(date));
            Assert.That(result.QrCode, Is.EqualTo(qr));
        }
    }
}
