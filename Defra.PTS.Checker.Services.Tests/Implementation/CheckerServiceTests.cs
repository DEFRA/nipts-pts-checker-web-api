using Defra.PTS.Checker.Entities;
using models = Defra.PTS.Checker.Models;
using Defra.PTS.Checker.Repositories.Interface;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Defra.PTS.Checker.Services.Implementation;

namespace Defra.PTS.Checker.Tests.Services
{
    [TestFixture]
    public class CheckerServiceTests
    {
        private Mock<IPetRepository>? _petRepositoryMock;
        private Mock<IApplicationRepository>? _applicationRepositoryMock;
        private Mock<ITravelDocumentRepository>? _travelDocumentRepositoryMock;
        private Mock<ICheckerRepository>? _checkerRepositoryMock;
        private Mock<ILogger<CheckerService>>? _loggerMock;
        private CheckerService? _checkerService;

        [SetUp]
        public void SetUp()
        {
            _petRepositoryMock = new Mock<IPetRepository>();
            _applicationRepositoryMock = new Mock<IApplicationRepository>();
            _travelDocumentRepositoryMock = new Mock<ITravelDocumentRepository>();
            _checkerRepositoryMock = new Mock<ICheckerRepository>();
            _loggerMock = new Mock<ILogger<CheckerService>>();

            _checkerService = new CheckerService(
                _petRepositoryMock.Object,
                _applicationRepositoryMock.Object,
                _travelDocumentRepositoryMock.Object,
                _checkerRepositoryMock.Object,
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

        [Test]
        public async Task CheckMicrochipNumberAsync_PetFound_MultipleApplications_ReturnsMostRelevantApplication()
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

            var applicationRevoked = new Application
            {
                Id = Guid.NewGuid(),
                PetId = petId,
                ReferenceNumber = "APP124",
                DateRevoked = DateTime.Now.AddDays(-1),
                Status = "revoked"
            };

            var applicationAwaitingVerification = new Application
            {
                Id = Guid.NewGuid(),
                PetId = petId,
                ReferenceNumber = "APP125",
                CreatedOn = DateTime.Now.AddDays(-2),
                Status = "awaiting verification"
            };

            _petRepositoryMock!.Setup(repo => repo.GetByMicrochipNumberAsync(microchipNumber))
                .ReturnsAsync(pets);

            _applicationRepositoryMock!.Setup(repo => repo.GetApplicationsByPetIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<Application> { applicationRevoked, applicationAwaitingVerification, applicationAuthorised });

            _travelDocumentRepositoryMock!.Setup(repo => repo.GetTravelDocumentByApplicationIdAsync(applicationAuthorised.Id))
                .ReturnsAsync(new TravelDocument
                {
                    Id = Guid.NewGuid(),
                    DocumentReferenceNumber = "TD123"
                });

            // Act
            var result = await _checkerService!.CheckMicrochipNumberAsync(microchipNumber);

            // Assert
            Assert.That(result, Is.Not.Null);

            // Extract

        }

        [Test]
        public async Task CheckerUser_AddChecker()
        {
            // Arrange
            var checkerDto = new models.CheckerDto
            {
                Id = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "Test",
                RoleId = null
            };

            _checkerRepositoryMock!.Setup(repo => repo.Add(It.IsAny<Entities.Checker>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _checkerService!.SaveChecker(checkerDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(checkerDto.Id));
        }

        [Test]
        public async Task CheckerUser_UpdateChecker()
        {
            // Arrange
            var checkerDto = new models.CheckerDto
            {
                Id = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "Test",
                RoleId = null
            };

            var checkerEntity = new Entities.Checker
            {
                Id = checkerDto.Id,
                FirstName = checkerDto.FirstName,
                LastName = checkerDto.LastName,
                FullName = $"{checkerDto.FirstName} {checkerDto.LastName}",
                RoleId = null
            };

            _checkerRepositoryMock!.Setup(repo => repo.Find(It.IsAny<Guid>()))
                .ReturnsAsync(checkerEntity);

            _checkerRepositoryMock!.Setup(repo => repo.Update(It.IsAny<Entities.Checker>()));

            // Act
            var result = await _checkerService!.SaveChecker(checkerDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(checkerDto.Id));
        }
    }
}
