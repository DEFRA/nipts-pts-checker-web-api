using Defra.PTS.Checker.Entities;
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
    public class ColourServiceTests
    {
        private Mock<IRepository<Colour>>? _colourRepositoryMock;
        private Mock<ILogger<ColourService>>? _loggerMock;
        private ColourService? _colourService;

        [SetUp]
        public void SetUp()
        {
            _colourRepositoryMock = new Mock<IRepository<Colour>>();
            _loggerMock = new Mock<ILogger<ColourService>>();
            _colourService = new ColourService(_loggerMock.Object, _colourRepositoryMock.Object);
        }

        [Test]
        public async Task GetAllColour_ReturnsListOfColourResponses()
        {
            // Arrange
            var colours = new List<Colour>
            {
                new Colour { Id = 1, Name = "Brown" },
                new Colour { Id = 2, Name = "White" }
            };

            _colourRepositoryMock!.Setup(repo => repo.GetAll()).Returns(colours);

            // Act
            var result = await _colourService!.GetAllColours();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Exactly(2).Items);
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Brown"));
            Assert.That(result.Last().Id, Is.EqualTo(2));
            Assert.That(result.Last().Name, Is.EqualTo("White"));
        }
    }
}
