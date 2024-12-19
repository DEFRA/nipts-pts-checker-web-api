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
                    BreedAdditionalInfo = travelDocument.Pet.AdditionalInfoMixedBreedOrUnknown,
                    Sex = Enum.GetName(typeof(PetGender), travelDocument.Pet.SexId),
                    DateOfBirth = travelDocument.Pet.DOB,
                    ColourName = !string.IsNullOrEmpty(travelDocument.Pet.OtherColour) ? travelDocument.Pet.OtherColour : travelDocument.Pet.Colour?.Name,
                    SignificantFeatures = travelDocument.Pet.HasUniqueFeature == (int)YesNoOptions.Yes ? travelDocument.Pet.UniqueFeatureDescription : "No",
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


            var petOwnerAddress = travelDocument.Application?.OwnerAddress != null
                ? new
                {
                    AddressLineOne = travelDocument.Application.OwnerAddress.AddressLineOne,
                    AddressLineTwo = travelDocument.Application.OwnerAddress.AddressLineTwo,
                    TownOrCity = travelDocument.Application.OwnerAddress.TownOrCity,
                    County = travelDocument.Application.OwnerAddress.County,
                    PostCode = travelDocument.Application.OwnerAddress.PostCode,
                }
                : null;

            var petOwner =
                new
                {
                    Name = travelDocument.Application?.OwnerNewName,
                    Telephone = travelDocument.Application?.OwnerNewTelephone,
                    Email = travelDocument.Application?.Owner != null ? travelDocument.Application.Owner.Email : null,
                    Address = petOwnerAddress,
                };

            var petDetail =
                new
                {
                    Pet = pet,
                    Application = application,
                    TravelDocument = travelDocumentDetail,
                    PetOwner = petOwner
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
                    BreedAdditionalInfo = application.Pet.AdditionalInfoMixedBreedOrUnknown,
                    Sex = Enum.GetName(typeof(PetGender), application.Pet.SexId),
                    DateOfBirth = application.Pet.DOB,
                    ColourName = !string.IsNullOrEmpty(application.Pet.OtherColour) ? application.Pet.OtherColour : application.Pet.Colour?.Name,
                    SignificantFeatures = application.Pet.HasUniqueFeature == (int)YesNoOptions.Yes ? application.Pet.UniqueFeatureDescription : "No",
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

            var petOwnerAddress = application.OwnerAddress != null
                ? new
                {
                    AddressLineOne = application.OwnerAddress.AddressLineOne,
                    AddressLineTwo = application.OwnerAddress.AddressLineTwo,
                    TownOrCity = application.OwnerAddress.TownOrCity,
                    County = application.OwnerAddress.County,
                    PostCode = application.OwnerAddress.PostCode,                    
                }
                : null;

            var petOwner = 
                new
                {
                    Name = application.OwnerNewName,
                    Telephone = application.OwnerNewTelephone,
                    Email = application.Owner != null ? application.Owner.Email : null,
                    Address = petOwnerAddress,
                };

            var response =
                new
                {
                    Pet = pet,
                    Application = applicationDetail,
                    TravelDocument = travelDocumentDetail,
                    PetOwner = petOwner
                };

            return response;
        }
    }
}
