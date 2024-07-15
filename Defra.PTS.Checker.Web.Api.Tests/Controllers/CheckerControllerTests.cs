using entities = Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Models;
using Defra.PTS.Checker.Models.Constants;
using Defra.PTS.Checker.Models.Search;
using Defra.PTS.Checker.Services.Interface;
using Defra.PTS.Checker.Web.Api.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using Defra.PTS.Checker.Entities;
using Azure;

namespace Defra.PTS.Checker.Web.Api.Tests.Controllers
{
    [TestFixture]
    public class CheckerControllerTests
    {
        private Mock<IApplicationService>? _applicationServiceMock;
        private Mock<ICheckerService>? _checkerServiceMock;
        private Mock<ICheckSummaryService>? _checkSummaryServiceMock;
        private CheckerController? _controller;

        [SetUp]
        public void SetUp()
        {
            _applicationServiceMock = new Mock<IApplicationService>();
            _checkerServiceMock = new Mock<ICheckerService>();
            _checkSummaryServiceMock = new Mock<ICheckSummaryService>();
            _controller = new CheckerController(_applicationServiceMock.Object, _checkerServiceMock.Object, _checkSummaryServiceMock.Object);
        }

        [Test]
        public async Task GetApplicationDetailsById_ReturnsOkResult()
        {
            // Arrange
            var request = new SearchByApplicationNumberRequest
            {
                ApplicationNumber = "UHXU1"
            };
            var response = new { ApplicationDetails = "Details" };

            _applicationServiceMock!.Setup(service => service.GetApplicationByReferenceNumber(It.IsAny<string>())).ReturnsAsync(response);

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
            var request = new SearchByApplicationNumberRequest
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

        [TestCase("", "Application number is required.")]
        [TestCase("123456789012345678901", "Application number cannot exceed 20 characters.")]
        public async Task GetApplicationDetailsById_InvalidId_ReturnsBadRequestResult(string applicationNumber, string expectedErrorMessage)
        {
            // Arrange
            var request = new SearchByApplicationNumberRequest
            {
                ApplicationNumber = applicationNumber
            };

            // Act
            var result = await _controller!.CheckApplicationNumber(request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult!.StatusCode, Is.EqualTo(400));

            // Extract error from BadRequest result
            var error = badRequestResult.Value;
            Assert.That(error, Is.Not.Null);

            // Convert the error to a dynamic object for easy access
            var errorObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(error));
            Assert.That(errorObj, Is.Not.Null);
            Assert.That(errorObj!.ContainsKey("error"));
            Assert.That(errorObj["error"], Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public async Task GetApplicationByPTDNumber_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var response = new { ApplicationDetails = "Details" };

            _applicationServiceMock!.Setup(service => service.GetApplicationByPTDNumber(It.IsAny<string>())).ReturnsAsync(response);

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

            _applicationServiceMock!.Setup(service => service.GetApplicationByPTDNumber(It.IsAny<string>()))!.ReturnsAsync(null);

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

            _applicationServiceMock!.Setup(service => service.GetApplicationByPTDNumber(request.PTDNumber))
                .ThrowsAsync(new Exception("Mock Exception"));

            // Assert
            Assert.ThrowsAsync<Exception>(async () => await _controller!.GetApplicationByPTDNumber(request));
        }


        [Test]
        public async Task CheckMicrochipNumber_MicrochipNumberIsNullOrEmpty_ReturnsBadRequest()
        {
            // Arrange
            var request = new SearchByMicrochipNumberRequest { MicrochipNumber = string.Empty };

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
            var request = new SearchByMicrochipNumberRequest { MicrochipNumber = "1234567890" };
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
            var request = new SearchByMicrochipNumberRequest { MicrochipNumber = "1234567890" };
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
            var request = new SearchByMicrochipNumberRequest { MicrochipNumber = "1234567890" };
            _checkerServiceMock!.Setup(service => service.CheckMicrochipNumberAsync(request.MicrochipNumber))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller!.CheckMicrochipNumber(request);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var ObjectResult = result as ObjectResult;
            Assert.That(ObjectResult!.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        }

        [Test]
        public async Task SaveCheckOutcomed_ReturnsOkResult()
        {
            // Arrange
            var request = new CheckOutcomeModel
            {
                CheckerId = null,
                CheckOutcome = "Pass",
                PTDNumber = "GB826CD186E",
                RouteId = 1,
                SailingTime = DateTime.UtcNow,
            };

            var response = new CheckOutcomeResponseModel  { CheckSummaryId = Guid.NewGuid() };
            
            _applicationServiceMock!.Setup(service => service.GetApplicationByPTDNumber(It.IsAny<string>()))!.ReturnsAsync(new { ApplicationDetails = "Details" });
            _checkSummaryServiceMock!.Setup(service => service.SaveCheckSummary(It.IsAny<CheckOutcomeModel>())).ReturnsAsync(response);

            // Act
            var result = await _controller!.SaveCheckOutcome(request);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task SaveCheckOutcomed_ValidRequestButNoApplication_ReturnsNotFoundResult()
        {
            // Arrange
            var request = new CheckOutcomeModel
            {
                CheckerId = null,
                CheckOutcome = "Pass",
                PTDNumber = "GB826CD186E",
                RouteId = 1,
                SailingTime = DateTime.UtcNow,
            };

            var response = new CheckOutcomeResponseModel { CheckSummaryId = Guid.NewGuid() };

            _applicationServiceMock!.Setup(service => service.GetApplicationByPTDNumber(It.IsAny<string>()))!.ReturnsAsync(default(object));
            _checkSummaryServiceMock!.Setup(service => service.SaveCheckSummary(It.IsAny<CheckOutcomeModel>())).ReturnsAsync(response);

            // Act
            var result = await _controller!.SaveCheckOutcome(request);
            
            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());

            var objectResult = result as NotFoundObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        }

        [Test]
        public async Task SaveCheckOutcomed_InvalidRequest_ReturnsBadRequestResult()
        {
            // Arrange
            var request = new CheckOutcomeModel
            {
                CheckerId = null,
                CheckOutcome = "Pass",
                PTDNumber = string.Empty,
                RouteId = 1,
                SailingTime = DateTime.UtcNow,
            };

            // Act
            _controller!.ModelState.AddModelError("PTDNumber", "PTDNumber is required");
            var result = await _controller!.SaveCheckOutcome(request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());

            var objectResult = result as BadRequestObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }
    }
}
