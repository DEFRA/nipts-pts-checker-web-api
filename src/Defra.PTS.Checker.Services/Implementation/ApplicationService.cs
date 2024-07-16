using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Repositories.Implementation;
using Defra.PTS.Checker.Repositories.Interface;
using Defra.PTS.Checker.Services.Enums;
using Defra.PTS.Checker.Services.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.PTS.Checker.Services.Implementation
{
    public class ApplicationService : IApplicationService
    {
        private readonly ILogger<ApplicationService> _log;
        private readonly IApplicationRepository _applicationRepository;
        private readonly ITravelDocumentService _travelDocumentService;

        public ApplicationService(ILogger<ApplicationService> log
            , IApplicationRepository applicationRepository
            , ITravelDocumentService travelDocumentService)
        {
            _log = log; 
            _applicationRepository = applicationRepository;
            _travelDocumentService = travelDocumentService;
        }

        public Task<Application> GetApplicationById(Guid id)
        {
            _log.LogInformation("Running inside method {0}", "GetApplicationById");
            var application = _applicationRepository.GetApplicationById(id);
            return application;
        }

        public async Task<object?> GetApplicationByPTDNumber(string ptdNumber)
        {
            var travelDocument = await _travelDocumentService.GetTravelDocumentByPTDNumber(ptdNumber);
            if (travelDocument == null)
            {
                return null;
            }

            var travelDocumentDetail =
                new
                {
                    TravelDocumentId = travelDocument.Id,
                    TravelDocumentReferenceNumber = travelDocument.DocumentReferenceNumber,
                    TravelDocumentDateOfIssue = travelDocument.DateOfIssue,
                    TravelDocumentValidityStartDate = travelDocument.ValidityStartDate,
                    TravelDocumentValidityEndDate = travelDocument.ValidityEndDate,
                    TravelDocumentStatusId = travelDocument.StatusId
                };

            var pet = travelDocument.Pet != null
                ? new
                {
                    PetId = travelDocument.Pet.Id,
                    PetName = travelDocument.Pet.Name,
                    Species = Enum.GetName(typeof(PetSpecies), travelDocument.Pet.SpeciesId),
                    BreedName = travelDocument.Pet.Breed?.Name,
                    Sex = Enum.GetName(typeof(PetGender), travelDocument.Pet.SexId),
                    DateOfBirth = travelDocument.Pet.DOB,
                    ColourName = travelDocument.Pet.Colour?.Name,
                    SignificantFeatures = travelDocument.Pet.UniqueFeatureDescription,
                    travelDocument.Pet.MicrochipNumber,
                    travelDocument.Pet.MicrochippedDate
                }
                : null;

            var application = travelDocument.Application != null
                ? new
                {
                    ApplicationId = travelDocument.Application.Id,
                    travelDocument.Application.ReferenceNumber,
                    travelDocument.Application.DateOfApplication,
                    travelDocument.Application.Status,
                    travelDocument.Application.DateAuthorised,
                    travelDocument.Application.DateRejected,
                    travelDocument.Application.DateRevoked
                }
                 : null;

            var petDetail =
                new
                {
                    Pet = pet,
                    Application = application,
                    TravelDocument = travelDocumentDetail
                };

            return petDetail;
        }


        public async Task<object?> GetApplicationByReferenceNumber(string referenceNumber)
        {
            _log.LogInformation("Running inside method {0}", "GetApplicationByReferenceNumber");
            var application = await _applicationRepository.GetApplicationByReferenceNumber(referenceNumber);
            if (application == null)
            {
                return null;
            }

            var travelDocument = await _travelDocumentService.GetTravelDocumentByApplicationId(application.Id);

            var travelDocumentDetail = travelDocument != null ?
                new
                {
                    TravelDocumentId = travelDocument.Id,
                    TravelDocumentReferenceNumber = travelDocument.DocumentReferenceNumber,
                    TravelDocumentDateOfIssue = travelDocument.DateOfIssue,
                    TravelDocumentValidityStartDate = travelDocument.ValidityStartDate,
                    TravelDocumentValidityEndDate = travelDocument.ValidityEndDate,
                    TravelDocumentStatusId = travelDocument.StatusId
                }
                : null;

            var pet = application.Pet != null
                ? new
                {
                    PetId = application.Pet.Id,
                    PetName = application.Pet.Name,
                    Species = Enum.GetName(typeof(PetSpecies), application.Pet.SpeciesId),
                    BreedName = application.Pet.Breed?.Name,
                    Sex = Enum.GetName(typeof(PetGender), application.Pet.SexId),
                    DateOfBirth = application.Pet.DOB,
                    ColourName = application.Pet.Colour?.Name,
                    SignificantFeatures = application.Pet.UniqueFeatureDescription,
                    application.Pet.MicrochipNumber,
                    application.Pet.MicrochippedDate
                }
                : null;

            var applicationDetail =
                new
                {
                    ApplicationId = application.Id,
                    application.ReferenceNumber,
                    application.DateOfApplication,
                    application.Status,
                    application.DateAuthorised,
                    application.DateRejected,
                    application.DateRevoked
                };

            var response =
                new
                {
                    Pet = pet,
                    Application = applicationDetail,
                    TravelDocument = travelDocumentDetail
                };

            return response;
        }
    }
}
