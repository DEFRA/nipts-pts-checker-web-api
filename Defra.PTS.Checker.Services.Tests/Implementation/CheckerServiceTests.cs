using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Repositories.Interface;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Defra.PTS.Checker.Tests.Services
{
    [TestFixture]
    public class CheckerServiceTests
    {
        private Mock<IPetRepository>? _petRepositoryMock;
        private Mock<IApplicationRepository>? _applicationRepositoryMock;
        private Mock<ITravelDocumentRepository>? _travelDocumentRepositoryMock;
        private Mock<ILogger<CheckerService>>? _loggerMock;
        private CheckerService? _checkerService;

        [SetUp]
        public void SetUp()
        {
            _petRepositoryMock = new Mock<IPetRepository>();
            _applicationRepositoryMock = new Mock<IApplicationRepository>();
            _travelDocumentRepositoryMock = new Mock<ITravelDocumentRepository>();
            _loggerMock = new Mock<ILogger<CheckerService>>();

            _checkerService = new CheckerService(
                _petRepositoryMock.Object,
                _applicationRepositoryMock.Object,
                _travelDocumentRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Test]
        public async Task CheckMicrochipNumberAsync_PetNotFound_ReturnsPetNotFoundError()
        {
            // Arrange
            string microchipNumber = "1234567890";
            _petRepositoryMock!.Setup(repo => repo.GetByMicrochipNumberAsync(microchipNumber))
                .ReturnsAsync(new List<Pet>());

            // Act
            var result = await _checkerService!.CheckMicrochipNumberAsync(microchipNumber);

            // Assert
            Assert.That(result, Is.Not.Null);
            var error = result!.GetType().GetProperty("error")!.GetValue(result, null);
            Assert.That(error, Is.EqualTo("Pet not found"));
        }

        [Test]
        public async Task CheckMicrochipNumberAsync_PetFound_NoRecentApplication_ReturnsApplicationNotFoundError()
        {
            // Arrange
            string microchipNumber = "1234567890";
            var pets = new List<Pet>
            {
                new Pet { Id = Guid.NewGuid(), Name = "Fido", MicrochipNumber = microchipNumber }
            };
            _petRepositoryMock!.Setup(repo => repo.GetByMicrochipNumberAsync(microchipNumber))
                .ReturnsAsync(pets);

            _applicationRepositoryMock!.Setup(repo => repo.GetApplicationsByPetIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<Application>());

            // Act
            var result = await _checkerService!.CheckMicrochipNumberAsync(microchipNumber);

            // Assert
            Assert.That(result, Is.Not.Null);
            var error = result!.GetType().GetProperty("error")!.GetValue(result, null);
            Assert.That(error, Is.EqualTo("Application not found"));
        }

        [Test]
        public async Task CheckMicrochipNumberAsync_PetFound_ApplicationAndTravelDocumentFound_ReturnsPetDetails()
        {
            // Arrange
            string microchipNumber = "1234567890";
            var petId = Guid.NewGuid();
            var pets = new List<Pet>
            {
                new Pet { Id = petId, Name = "Fido", MicrochipNumber = microchipNumber }
            };
            var applicationAuthorised = new Application
            {
                Id = Guid.NewGuid(),
                PetId = petId,
                ReferenceNumber = "APP123",
                DateAuthorised = DateTime.Now,
                Status = "authorised"
            };
            var travelDocument = new TravelDocument
            {
                Id = Guid.NewGuid(),
                DocumentReferenceNumber = "TD123"
            };

            _petRepositoryMock!.Setup(repo => repo.GetByMicrochipNumberAsync(microchipNumber))
                .ReturnsAsync(pets);

            _applicationRepositoryMock!.Setup(repo => repo.GetApplicationsByPetIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<Application> { applicationAuthorised });

            _travelDocumentRepositoryMock!.Setup(repo => repo.GetTravelDocumentByApplicationIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(travelDocument);

            // Act
            var result = await _checkerService!.CheckMicrochipNumberAsync(microchipNumber);

            // Assert
            Assert.That(result, Is.Not.Null);

            // Extract properties from the result
            var petDetails = result!.GetType().GetProperty("Pet")!.GetValue(result, null);
            var applicationDetails = result.GetType().GetProperty("Application")!.GetValue(result, null);
            var travelDocumentDetails = result.GetType().GetProperty("TravelDocument")!.GetValue(result, null);

            Assert.That(petDetails, Is.Not.Null);
            Assert.That(applicationDetails, Is.Not.Null);
            Assert.That(travelDocumentDetails, Is.Not.Null);

            var petName = petDetails!.GetType().GetProperty("PetName")!.GetValue(petDetails, null);
            var applicationReferenceNumber = applicationDetails!.GetType().GetProperty("ReferenceNumber")!.GetValue(applicationDetails, null);
            var travelDocumentReferenceNumber = travelDocumentDetails!.GetType().GetProperty("TravelDocumentReferenceNumber")!.GetValue(travelDocumentDetails, null);

            Assert.That(petName, Is.EqualTo("Fido"));
            Assert.That(applicationReferenceNumber, Is.EqualTo("APP123"));
            Assert.That(travelDocumentReferenceNumber, Is.EqualTo("TD123"));
        }

        [Test]
        public async Task CheckMicrochipNumberAsync_PetFound_ApplicationFound_NoTravelDocument_ReturnsPetDetailsWithoutTravelDocument()
        {
            // Arrange
            string microchipNumber = "1234567890";
            var petId = Guid.NewGuid();
            var pets = new List<Pet>
            {
                new Pet { Id = petId, Name = "Fido", MicrochipNumber = microchipNumber }
            };
            var applicationAuthorised = new Application
            {
                Id = Guid.NewGuid(),
                PetId = petId,
                ReferenceNumber = "APP123",
                DateAuthorised = DateTime.Now,
                Status = "authorised"
            };

            _petRepositoryMock!.Setup(repo => repo.GetByMicrochipNumberAsync(microchipNumber))
                .ReturnsAsync(pets);

            _applicationRepositoryMock!.Setup(repo => repo.GetApplicationsByPetIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<Application> { applicationAuthorised });

            _travelDocumentRepositoryMock!.Setup(repo => repo.GetTravelDocumentByApplicationIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((TravelDocument?)null);

            // Act
            var result = await _checkerService!.CheckMicrochipNumberAsync(microchipNumber);

            // Assert
            Assert.That(result, Is.Not.Null);

            // Extract properties from the result
            var petDetails = result!.GetType().GetProperty("Pet")!.GetValue(result, null);
            var applicationDetails = result.GetType().GetProperty("Application")!.GetValue(result, null);
            var travelDocumentDetails = result.GetType().GetProperty("TravelDocument")!.GetValue(result, null);

            Assert.That(petDetails, Is.Not.Null);
            Assert.That(applicationDetails, Is.Not.Null);
            Assert.That(travelDocumentDetails, Is.Null);

            var petName = petDetails!.GetType().GetProperty("PetName")!.GetValue(petDetails, null);
            var applicationReferenceNumber = applicationDetails!.GetType().GetProperty("ReferenceNumber")!.GetValue(applicationDetails, null);

            Assert.That(petName, Is.EqualTo("Fido"));
            Assert.That(applicationReferenceNumber, Is.EqualTo("APP123"));
        }
    }
}
