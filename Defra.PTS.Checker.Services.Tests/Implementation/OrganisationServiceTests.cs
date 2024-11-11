using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Models;
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
    public class OrganisationServiceTests
    {
        private Mock<IRepository<Organisation>> _organisationRepository;
        private Mock<ILogger<OrganisationService>>? _loggerMock;
        private OrganisationService _organisationService;
        public OrganisationServiceTests()
        {
            _organisationRepository = new Mock<IRepository<Organisation>>();
            _loggerMock = new Mock<ILogger<OrganisationService>>();

            _organisationService = new OrganisationService(_loggerMock.Object, _organisationRepository.Object);
        }

        [Test]
        public async Task GetOrganisation_When_Organisation_IsNotNull_ReturnsValidObject()
        {
            var organisation = new Organisation()
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                ActiveFrom = DateTime.Now,
                Location = "NI",
                Name = "Test",
            };
            _organisationRepository.Setup(a => a.Find(It.IsAny<Guid>())).ReturnsAsync(organisation);

            var result = await _organisationService.GetOrganisation(Guid.NewGuid());

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.TypeOf<OrganisationResponseModel>());
            Assert.That(result?.Id, Is.EqualTo(organisation.Id));
            Assert.That(result?.Name, Is.EqualTo("Test"));
            Assert.That(result?.IsActive, Is.True);
        }

        [Test]
        public async Task GetOrganisation_When_Organisation_Null_ReturnsValidObject()
        {
            _organisationRepository.Setup(a => a.Find(It.IsAny<Guid>())).ReturnsAsync((Organisation?)null!);

            var result = await _organisationService.GetOrganisation(Guid.NewGuid());

            Assert.That(result, Is.Null);

        }
    }
}
