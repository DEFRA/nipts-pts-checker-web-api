using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Repositories.Implementation;
using Defra.PTS.Checker.Repositories.Interface;
using Defra.PTS.Checker.Services.Enums;
using Defra.PTS.Checker.Services.Implementation;
using Defra.PTS.Checker.Services.Interface;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Defra.PTS.Checker.Services.Tests.Implementation
{
    [TestFixture]
    public class ApplicationServiceTests
    {
        private Mock<IApplicationRepository>? _applicationRepositoryMock;
        private Mock<ITravelDocumentService>? _travelDocumentServiceMock;
        private Mock<ILogger<ApplicationService>>? _loggerMock;
        private ApplicationService? _applicationService;

        [SetUp]
        public void SetUp()
        {
            _applicationRepositoryMock = new Mock<IApplicationRepository>();
            _loggerMock = new Mock<ILogger<ApplicationService>>();
            _travelDocumentServiceMock = new Mock<ITravelDocumentService>();
            _applicationService = new ApplicationService(_loggerMock.Object, _applicationRepositoryMock.Object, _travelDocumentServiceMock.Object);
        }

        [Test]
        public async Task GetApplicationById_ReturnsApplication()
        {
            // Arrange

            var guid = Guid.Parse("F567CDDA-DC72-4865-C18A-08DC12AE079D");
            var date = DateTime.Now;

            var pet = new Pet
            {
                Name = "Kitsu"
            };

            var owner = new Owner
            {
                FullName = "Dean",
                CharityName = "Special Effect"
            };

            var user = new User
            {
                FirstName = "tester"
            };

            var address = new Address
            {
                AddressLineOne = "1 Test Lane"
            };

            var application = new Application
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

            _applicationRepositoryMock!.Setup(repo => repo.GetApplicationById(It.IsAny<Guid>())).ReturnsAsync(application);

            // Act
            var result = await _applicationService!.GetApplicationById(guid);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(guid));
            Assert.That(result.PetId, Is.EqualTo(guid));
            Assert.That(result.DynamicId, Is.EqualTo(guid));
            Assert.That(result.OwnerAddressId, Is.EqualTo(guid));
            Assert.That(result.OwnerId, Is.EqualTo(guid));
            Assert.That(result.UserId, Is.EqualTo(guid));
            Assert.That(result.UserId, Is.EqualTo(guid));
            Assert.That(result.IsConsentAgreed, Is.EqualTo(true));
            Assert.That(result.IsDeclarationSigned, Is.EqualTo(true));
            Assert.That(result.IsPrivacyPolicyAgreed, Is.EqualTo(true));
            Assert.That(result.CreatedBy, Is.EqualTo(guid));
            Assert.That(result.DateAuthorised, Is.EqualTo(date));
            Assert.That(result.DateOfApplication, Is.EqualTo(date));
            Assert.That(result.DateRejected, Is.EqualTo(date));
            Assert.That(result.DateRevoked, Is.EqualTo(date));
            Assert.That(result.OwnerAddress, Is.EqualTo(address));
        }


        [Test]
        public async Task GetApplicationByPTDNumber_ReturnsNull_WhenTravelDocumentNotFound()
        {
            // Arrange
            string ptdNumber = "PTD123";
            _travelDocumentServiceMock.Setup(repo => repo.GetTravelDocumentByPTDNumber(ptdNumber))
                           .ReturnsAsync((TravelDocument)null);

            // Act
            var result = await _applicationService.GetApplicationByPTDNumber(ptdNumber);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetApplicationByPTDNumber_ReturnsCorrectData_WhenTravelDocumentFound()
        {
            // Arrange
            string ptdNumber = "PTD123";
            var travelDocument = new TravelDocument
            {
                Id = Guid.NewGuid(),
                DocumentReferenceNumber = "REF123",
                DateOfIssue = new DateTime(2023, 1, 1),
                ValidityStartDate = new DateTime(2023, 1, 1),
                ValidityEndDate = new DateTime(2024, 1, 1),
                StatusId = 1,
                Pet = new Pet
                {
                    Id = Guid.NewGuid(),
                    Name = "Buddy",
                    SpeciesId = (int)PetSpecies.Dog,
                    Breed = new Breed { Name = "Labrador" },
                    SexId = (int)PetGender.Male,
                    DOB = new DateTime(2020, 1, 1),
                    Colour = new Colour { Name = "Black" },
                    UniqueFeatureDescription = "Scar on ear",
                    MicrochipNumber = "1234567890",
                    MicrochippedDate = new DateTime(2020, 1, 10)
                },
                Application = new Application
                {
                    Id = Guid.NewGuid(),
                    ReferenceNumber = "APP123",
                    DateOfApplication = new DateTime(2022, 1, 1),
                    Status = "Approved",
                    DateAuthorised = new DateTime(2022, 2, 1),
                    DateRejected = null,
                    DateRevoked = null
                }
            };

            _travelDocumentServiceMock.Setup(repo => repo.GetTravelDocumentByPTDNumber(ptdNumber))
                           .ReturnsAsync(travelDocument);

            // Act
            var result = await _applicationService.GetApplicationByPTDNumber(ptdNumber);

            // Assert
            Assert.That(result, Is.Not.Null);
            var parsedJson = System.Text.Json.JsonSerializer.Serialize(result);

            // Parse JSON string
            using JsonDocument doc = JsonDocument.Parse(parsedJson);
            JsonElement root = doc.RootElement;

            string str = string.Empty;
            // Extract and assert TravelDocument details
            Assert.That(travelDocument.Id, Is.EqualTo(Guid.Parse(root.GetProperty("TravelDocument").GetProperty("TravelDocumentId").GetString())));
            Assert.That(travelDocument.DocumentReferenceNumber, Is.EqualTo(root.GetProperty("TravelDocument").GetProperty("TravelDocumentReferenceNumber").GetString()));
            Assert.That(travelDocument.DateOfIssue, Is.EqualTo(root.GetProperty("TravelDocument").GetProperty("TravelDocumentDateOfIssue").GetDateTime()));
            Assert.That(travelDocument.ValidityStartDate, Is.EqualTo(root.GetProperty("TravelDocument").GetProperty("TravelDocumentValidityStartDate").GetDateTime()));
            Assert.That(travelDocument.ValidityEndDate, Is.EqualTo(root.GetProperty("TravelDocument").GetProperty("TravelDocumentValidityEndDate").GetDateTime()));
            Assert.That(travelDocument.StatusId, Is.EqualTo(root.GetProperty("TravelDocument").GetProperty("TravelDocumentStatusId").GetInt32()));

            //// Extract and assert Pet details
            Assert.That(travelDocument.Pet.Id, Is.EqualTo(Guid.Parse(root.GetProperty("Pet").GetProperty("PetId").GetString())));
            Assert.That(travelDocument.Pet.Name, Is.EqualTo(root.GetProperty("Pet").GetProperty("PetName").GetString()));
            Assert.That(Enum.GetName(typeof(PetSpecies), travelDocument.Pet.SpeciesId), Is.EqualTo(root.GetProperty("Pet").GetProperty("Species").GetString()));
            Assert.That(travelDocument.Pet.Breed.Name, Is.EqualTo(root.GetProperty("Pet").GetProperty("BreedName").GetString()));
            Assert.That(Enum.GetName(typeof(PetGender), travelDocument.Pet.SexId), Is.EqualTo(root.GetProperty("Pet").GetProperty("Sex").GetString()));
            Assert.That(travelDocument.Pet.DOB, Is.EqualTo(root.GetProperty("Pet").GetProperty("DateOfBirth").GetDateTime()));
            Assert.That(travelDocument.Pet.Colour.Name, Is.EqualTo(root.GetProperty("Pet").GetProperty("ColourName").GetString()));
            Assert.That(travelDocument.Pet.UniqueFeatureDescription, Is.EqualTo(root.GetProperty("Pet").GetProperty("SignificantFeatures").GetString()));
            Assert.That(travelDocument.Pet.MicrochipNumber, Is.EqualTo(root.GetProperty("Pet").GetProperty("MicrochipNumber").GetString()));
            Assert.That(travelDocument.Pet.MicrochippedDate, Is.EqualTo(root.GetProperty("Pet").GetProperty("MicrochippedDate").GetDateTime()));

            //// Extract and assert Application details
            Assert.That(travelDocument.Application.Id, Is.EqualTo(Guid.Parse(root.GetProperty("Application").GetProperty("ApplicationId").GetString())));
            Assert.That(travelDocument.Application.ReferenceNumber, Is.EqualTo(root.GetProperty("Application").GetProperty("ReferenceNumber").GetString()));
            Assert.That(travelDocument.Application.DateOfApplication, Is.EqualTo(root.GetProperty("Application").GetProperty("DateOfApplication").GetDateTime()));
            Assert.That(travelDocument.Application.Status, Is.EqualTo(root.GetProperty("Application").GetProperty("Status").GetString()));
            Assert.That(travelDocument.Application.DateAuthorised, Is.EqualTo(root.GetProperty("Application").GetProperty("DateAuthorised").GetDateTime()));
            Assert.That(travelDocument.Application.DateRejected, Is.EqualTo(root.GetProperty("Application").GetProperty("DateRejected").GetString()));
            Assert.That(travelDocument.Application.DateRevoked, Is.EqualTo(root.GetProperty("Application").GetProperty("DateRevoked").GetString()));
        }

    }
}
