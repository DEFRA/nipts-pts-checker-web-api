using Defra.PTS.Checker.Entities;
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
    public class CheckerControllerTests
    {
        private Mock<ITravelDocumentService>? _travelDocumentServiceMock;
        private Mock<IApplicationService>? _applicationServiceMock;
        private CheckerController? _controller;

        [SetUp]
        public void SetUp()
        {
            _travelDocumentServiceMock = new Mock<ITravelDocumentService>();
            _applicationServiceMock = new Mock<IApplicationService>();
            _controller = new CheckerController(_applicationServiceMock.Object, _travelDocumentServiceMock.Object);
        }

        [Test]
        public async Task GetApplicationDetailsById_ReturnsOkResult()
        {
            // Arrange
            var guid = Guid.Parse("F567CDDA-DC72-4865-C18A-08DC12AE079D");
            var date = DateTime.Now;
            var qr = new byte[] { 1 };

            var pet = new Pet
            {
                Name = "Kitsu"
            };

            var owner = new Entities.Owner
            {
                FullName = "Dean",
                CharityName = "Special Effect"
            };

            var user = new Entities.User
            {
                FirstName = "tester"
            };

            var address = new Entities.Address
            {
                AddressLineOne = "1 Test Lane"
            };

            var application = new Entities.Application
            {
                Id = guid,
                PetId = guid,
                DynamicId = guid,
                OwnerAddressId = guid,
                OwnerId = guid,
                UserId = guid,
                Owner = owner,
                User = user,
                Pet = pet,
                CreatedOn = date,
                DateAuthorised = date,
                DateOfApplication = date,
                DateRejected = date,
                DateRevoked = date,
                UpdatedOn = date,
                Status = "In Test",
                CreatedBy = guid,
                UpdatedBy = guid,
                OwnerNewName = "Newman",
                IsConsentAgreed = true,
                IsDeclarationSigned = true,
                IsPrivacyPolicyAgreed = true,
                OwnerNewTelephone = "123",
                ReferenceNumber = "GB123",
                OwnerAddress = address
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

            _travelDocumentServiceMock.Setup(service => service.GetTravelDocumentByReferenceNumber(It.IsAny<string>())).ReturnsAsync(travelDocument);
            _applicationServiceMock.Setup(service => service.GetApplicationById(It.IsAny<Guid>())).ReturnsAsync(application);

            // Act
            var result = await _controller.CheckApplicationNumber("GB123");

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task GetApplicationDetailsById_ValidIdButNoApplication_ReturnsNotFoundResult()
        {
            // Arrange
            _travelDocumentServiceMock.Setup(service => service.GetTravelDocumentByReferenceNumber(It.IsAny<string>())).ReturnsAsync((TravelDocument)null);

            // Act
            var result = await _controller.CheckApplicationNumber("GB123");

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
            var notFoundResult = result as NotFoundResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task GetApplicationDetailsById_InvalidId_ReturnsBadRequestResult()
        {
            // Arrange
            // Act
            var result = await _controller.CheckApplicationNumber("123");

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        }
    }
}
