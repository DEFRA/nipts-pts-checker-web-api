using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Models;
using Defra.PTS.Checker.Models.Constants;
using Defra.PTS.Checker.Models.Search;
using Defra.PTS.Checker.Services.Interface;
using Defra.PTS.Checker.Web.Api.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;

namespace Defra.PTS.Checker.Web.Api.Tests.Controllers
{
    [TestFixture]
    public class CheckerControllerTests
    {
        private Mock<ITravelDocumentService>? _travelDocumentServiceMock;
        private Mock<IApplicationService>? _applicationServiceMock;
        private Mock<ICheckerService>? _checkerServiceMock;
        private CheckerController? _controller;

        [SetUp]
        public void SetUp()
        {
            _travelDocumentServiceMock = new Mock<ITravelDocumentService>();
            _applicationServiceMock = new Mock<IApplicationService>();
            _checkerServiceMock = new Mock<ICheckerService>();
            _controller = new CheckerController(_travelDocumentServiceMock.Object, _applicationServiceMock.Object, _checkerServiceMock.Object);
        }

        [Test]
        public async Task GetApplicationDetailsById_ReturnsOkResult()
        {
            // Arrange
            var request = new ApplicationNumberCheckRequest
            {
                ApplicationNumber = "UHXU1"
            };

            var application = GetApplication();

            var travelDocument = GetTravelDocument();

            _applicationServiceMock!.Setup(service => service.GetApplicationByReferenceNumber(It.IsAny<string>())).ReturnsAsync(application);
            _travelDocumentServiceMock!.Setup(service => service.GetTravelDocumentByApplicationId(It.IsAny<Guid>())).ReturnsAsync(travelDocument);


            // Act
            var result = await _controller!.CheckApplicationNumber(request);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task GetApplicationDetailsById_ValidIdButNoApplication_ReturnsNotFoundResult()
        {
            // Arrange
            var request = new ApplicationNumberCheckRequest 
            { 
                ApplicationNumber = "UZHR2" 
            };

            _applicationServiceMock!.Setup(service => service.GetApplicationByReferenceNumber(It.IsAny<string>()))!.ReturnsAsync((Entities.Application)null!);

            // Act
            var result = await _controller!.CheckApplicationNumber(request);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult!.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task GetApplicationDetailsById_InvalidId_ReturnsBadRequestResult()
        {
            // Arrange
            var request = new ApplicationNumberCheckRequest
            {
                ApplicationNumber = ""
            };

            // Act
            var result = await _controller!.CheckApplicationNumber(request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult!.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task GetApplicationByPTDNumber_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var travelDocument = GetTravelDocument();

            _travelDocumentServiceMock!.Setup(service => service.GetTravelDocumentByPTDNumber(It.IsAny<string>())).ReturnsAsync(travelDocument);

            var request = new SearchByPtdNumberRequest
            {
                PTDNumber = $"{ApiConstants.PTDNumberPrefix}ABCXYZ123",
            };

            // Act
            var result = await _controller!.GetApplicationByPTDNumber(request);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var objectResult = result as OkObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }

        [Test]
        public async Task GetApplicationByPTDNumber_ValidRequestButNoApplication_ReturnsNotFoundResult()
        {
            // Arrange
            var request = new SearchByPtdNumberRequest
            {
                PTDNumber = $"{ApiConstants.PTDNumberPrefix}ABCXYZ123",
            };

            _travelDocumentServiceMock!.Setup(service => service.GetTravelDocumentByPTDNumber(It.IsAny<string>()))!.ReturnsAsync((TravelDocument)null!);

            // Act
            var result = await _controller!.GetApplicationByPTDNumber(request);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());

            var objectResult = result as NotFoundObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        }


        [Test]
        public async Task GetApplicationByPTDNumber_InvalidRequest_ReturnsBadRequestResult()
        {
            // Arrange
            var request = new SearchByPtdNumberRequest
            {
                 PTDNumber = string.Empty,
            };

            // Act
            _controller!.ModelState.AddModelError("PTDNumber", "PTDNumber is required");
            var result = await _controller!.GetApplicationByPTDNumber(request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());

            var objectResult = result as BadRequestObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public void GetApplicationByPTDNumber_Exception_ReturnsInternalServerError()
        {
            // Arrange
            var request = new SearchByPtdNumberRequest
            {
                PTDNumber = $"{ApiConstants.PTDNumberPrefix}ABCXYZ123",
            };

            _travelDocumentServiceMock!.Setup(service => service.GetTravelDocumentByPTDNumber(request.PTDNumber))
                .ThrowsAsync(new Exception("Mock Exception"));

            // Assert
            Assert.ThrowsAsync<Exception>(async () => await _controller!.GetApplicationByPTDNumber(request));
        }


        [Test]
        public async Task CheckMicrochipNumber_MicrochipNumberIsNullOrEmpty_ReturnsBadRequest()
        {
            // Arrange
            var request = new MicrochipCheckRequest { MicrochipNumber = string.Empty };

            // Act
            var result = await _controller!.CheckMicrochipNumber(request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult!.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public async Task CheckMicrochipNumber_ServiceReturnsNull_ReturnsNotFound()
        {
            // Arrange
            var request = new MicrochipCheckRequest { MicrochipNumber = "1234567890" };
            _checkerServiceMock!.Setup(service => service.CheckMicrochipNumberAsync(request!.MicrochipNumber)).ReturnsAsync((object?)null!);

            // Act
            var result = await _controller!.CheckMicrochipNumber(request);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult!.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        }

        [Test]
        public async Task CheckMicrochipNumber_ServiceReturnsResponse_ReturnsOk()
        {
            // Arrange
            var request = new MicrochipCheckRequest { MicrochipNumber = "1234567890" };
            var response = new { PetDetails = "Details" };
            _checkerServiceMock!.Setup(service => service.CheckMicrochipNumberAsync(request.MicrochipNumber))
                .ReturnsAsync(response);

            // Act
            var result = await _controller!.CheckMicrochipNumber(request);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var OkObjectResult = result as OkObjectResult;
            Assert.That(OkObjectResult, Is.Not.Null);
            Assert.That(OkObjectResult!.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }

        [Test]
        public async Task CheckMicrochipNumber_ServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var request = new MicrochipCheckRequest { MicrochipNumber = "1234567890" };
            _checkerServiceMock!.Setup(service => service.CheckMicrochipNumberAsync(request.MicrochipNumber))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller!.CheckMicrochipNumber(request);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var ObjectResult = result as ObjectResult;
            Assert.That(ObjectResult!.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        }

        private static TravelDocument GetTravelDocument()
        {
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

            return travelDocument;
        }

        private static Entities.Application GetApplication()
        {
            var guid = Guid.Parse("F567CDDA-DC72-4865-C18A-08DC12AE079D");
            var date = DateTime.Now;

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

            return application;
        }
    }
}
