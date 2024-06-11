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
    public class ApplicationServiceTests
    {
        private Mock<IApplicationRepository>? _applicationRepositoryMock;
        private Mock<ILogger<ApplicationService>>? _loggerMock;
        private ApplicationService? _applicationService;

        [SetUp]
        public void SetUp()
        {
            _applicationRepositoryMock = new Mock<IApplicationRepository>();
            _loggerMock = new Mock<ILogger<ApplicationService>>();
            _applicationService = new ApplicationService(_loggerMock.Object, _applicationRepositoryMock.Object);
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

            _applicationRepositoryMock.Setup(repo => repo.GetApplicationById(It.IsAny<Guid>())).ReturnsAsync(application);

            // Act
            var result = await _applicationService.GetApplicationById(guid);

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
    }
}
