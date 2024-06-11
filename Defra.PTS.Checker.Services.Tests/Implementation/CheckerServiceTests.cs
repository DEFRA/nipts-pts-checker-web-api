using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Repositories.Interface;
using Defra.PTS.Checker.Services.Implementation;
using Moq;
using NUnit.Framework;

namespace Defra.PTS.Checker.Tests.Services
{
    [TestFixture]
    public class CheckerServiceTests
    {
        private Mock<IPetRepository> _petRepositoryMock;
        private Mock<IApplicationRepository> _applicationRepositoryMock;
        private Mock<ITravelDocumentRepository> _travelDocumentRepositoryMock;
        private CheckerService _checkerService;

        [SetUp]
        public void SetUp()
        {
            _petRepositoryMock = new Mock<IPetRepository>();
            _applicationRepositoryMock = new Mock<IApplicationRepository>();
            _travelDocumentRepositoryMock = new Mock<ITravelDocumentRepository>();

            _checkerService = new CheckerService(
                _petRepositoryMock.Object,
                _applicationRepositoryMock.Object,
                _travelDocumentRepositoryMock.Object
            );
        }

        [Test]
        public async Task CheckMicrochipNumberAsync_PetNotFound_ReturnsNull()
        {
            // Arrange
            string microchipNumber = "1234567890";
            _petRepositoryMock.Setup(repo => repo.GetByMicrochipNumberAsync(microchipNumber))
                .ReturnsAsync(new List<Pet>());

            // Act
            var result = await _checkerService.CheckMicrochipNumberAsync(microchipNumber);

            // Assert
            Assert.That(result, Is.Null);
            
        }

        [Test]
        public async Task CheckMicrochipNumberAsync_PetFound_NoRecentApplication_ReturnsNull()
        {
            // Arrange
            string microchipNumber = "1234567890";
            var pets = new List<Pet>
            {
                new Pet { Id = Guid.NewGuid(), Name = "Fido", MicrochipNumber = microchipNumber }
            };
            _petRepositoryMock.Setup(repo => repo.GetByMicrochipNumberAsync(microchipNumber))
                .ReturnsAsync(pets);

            _applicationRepositoryMock.Setup(repo => repo.GetMostRecentApplication(It.IsAny<Guid>()))
                .Returns((Application)null);

            // Act
            var result = await _checkerService.CheckMicrochipNumberAsync(microchipNumber);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CheckMicrochipNumberAsync_PetFound_ApplicationAndTravelDocumentFound_ReturnsPetDetails()
        {
            // Arrange
            string microchipNumber = "1234567890";
            var pets = new List<Pet>
            {
                new Pet { Id = Guid.NewGuid(), Name = "Fido", MicrochipNumber = microchipNumber }
            };
            var application = new Application { Id = Guid.NewGuid(), PetId = Guid.NewGuid(), ReferenceNumber = "APP123" };
            var travelDocument = new TravelDocument { Id = Guid.NewGuid(), DocumentReferenceNumber = "TD123" };

            _petRepositoryMock.Setup(repo => repo.GetByMicrochipNumberAsync(microchipNumber))
                .ReturnsAsync(pets);

            _applicationRepositoryMock.Setup(repo => repo.GetMostRecentApplication(It.IsAny<Guid>()))
                .Returns(application);

            _travelDocumentRepositoryMock.Setup(repo => repo.GetTravelDocumentByApplicationIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(travelDocument);

            // Act
            var result = await _checkerService.CheckMicrochipNumberAsync(microchipNumber);

            // Assert
            Assert.That(result, Is.Not.Null);            
            var petDetails = result as List<object>;
            Assert.That(petDetails, Is.Not.Null);
            Assert.That(petDetails, Is.Not.Null);
            Assert.That(petDetails?.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task CheckMicrochipNumberAsync_PetFound_ApplicationFound_NoTravelDocument_ReturnsPetDetailsWithoutTravelDocument()
        {
            // Arrange
            string microchipNumber = "1234567890";
            var pets = new List<Pet>
            {
                new Pet { Id = Guid.NewGuid(), Name = "Fido", MicrochipNumber = microchipNumber }
            };
            var application = new Application { Id = Guid.NewGuid(), PetId = Guid.NewGuid(), ReferenceNumber = "APP123" };

            _petRepositoryMock.Setup(repo => repo.GetByMicrochipNumberAsync(microchipNumber))
                .ReturnsAsync(pets);

            _applicationRepositoryMock.Setup(repo => repo.GetMostRecentApplication(It.IsAny<Guid>()))
                .Returns(application);

            _travelDocumentRepositoryMock.Setup(repo => repo.GetTravelDocumentByApplicationIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((TravelDocument)null);

            // Act
            var result = await _checkerService.CheckMicrochipNumberAsync(microchipNumber);

            // Assert
            // Assert
            Assert.That(result, Is.Not.Null);
            var petDetails = result as List<object>;
            Assert.That(petDetails, Is.Not.Null);
            Assert.That(petDetails, Is.Not.Null);
            Assert.That(petDetails?.Count, Is.EqualTo(1));
        }
    }
}
