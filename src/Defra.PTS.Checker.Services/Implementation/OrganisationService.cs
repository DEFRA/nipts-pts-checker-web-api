using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Models;
using Defra.PTS.Checker.Repositories.Interface;
using Defra.PTS.Checker.Services.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.PTS.Checker.Services.Implementation
{
    public class OrganisationService : IOrganisationService
    {

        private readonly IRepository<Organisation> _organisationRepository;
        private readonly ILogger<OrganisationService> _log;
        public OrganisationService(ILogger<OrganisationService> log, IRepository<Organisation> organisationRepository)
        {
            _log = log;
            _organisationRepository = organisationRepository;
        }


        public async Task<OrganisationResponseModel> GetOrganisation(Guid organisationId)
        {
            var organisation = await _organisationRepository.Find(organisationId);
            if (organisation == null)
            {
                return null;
            }

            return new OrganisationResponseModel
            {
                Id = organisation.Id,
                Name = organisation.Name,
                Location = organisation.Location,
                ActiveFrom = organisation.ActiveFrom,
                ActiveTo = organisation.ActiveTo,
                ExternalId = organisation.ExternalId,
                IsActive = organisation.IsActive, // Handle nullable boolean explicitly
            };
        }
    }
}
