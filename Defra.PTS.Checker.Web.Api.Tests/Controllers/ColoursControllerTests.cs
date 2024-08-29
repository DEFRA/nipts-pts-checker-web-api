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
    public class ColoursControllerTests
    {
        private Mock<IColourService>? _coloursServiceMock;
        private ColoursController? _controller;

        [SetUp]
        public void SetUp()
        {
            _coloursServiceMock = new Mock<IColourService>();
            _controller = new ColoursController(_coloursServiceMock.Object);
        }

        [Test]
        public async Task GetAllColours_ReturnsOkResult_WithListOfColoursRoutes()
        {
            // Arrange
            var colours = new List<Colour>
            {
                new Colour { Id = 1, Name = "Brown" },
                new Colour { Id = 2, Name = "White" }
            };

            _coloursServiceMock!.Setup(s => s.GetAllColours()).ReturnsAsync(colours);

            // Act
            var result = await _controller!.GetAllColours();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(colours));
        }
    }
}
