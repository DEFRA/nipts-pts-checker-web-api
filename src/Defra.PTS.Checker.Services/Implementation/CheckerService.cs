using Defra.PTS.Checker.Services.Enums;
using Defra.PTS.Checker.Repositories.Interface;
using Defra.PTS.Checker.Services.Interface;
using Microsoft.Extensions.Logging;

namespace Defra.PTS.Checker.Services.Implementation
{
    public class CheckerService : ICheckerService
    {
        private readonly IPetRepository _petRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly ITravelDocumentRepository _travelDocumentRepository;

        public CheckerService(IPetRepository petRepository, IApplicationRepository applicationRepository, ITravelDocumentRepository travelDocumentRepository)
        {
            _petRepository = petRepository;
            _applicationRepository = applicationRepository;
            _travelDocumentRepository = travelDocumentRepository;
        }

        public async Task<object> CheckMicrochipNumberAsync(string microchipNumber)
        {
            var pets = await _petRepository.GetByMicrochipNumberAsync(microchipNumber);
            if (!pets.Any())
            {
                return null;
            }

            var petDetails = new List<object>();

            foreach (var pet in pets)
            {
                var mostRecentApplication = _applicationRepository.GetMostRecentApplication(pet.Id);
                if (mostRecentApplication == null)
                {
                    continue;
                }

                var travelDocument = await _travelDocumentRepository.GetTravelDocumentByApplicationIdAsync(mostRecentApplication.Id);
                var travelDocumentDetail = travelDocument != null
                    ? new
                    {
                        TravelDocumentId = travelDocument.Id,
                        TravelDocumentReferenceNumber = travelDocument.DocumentReferenceNumber,
                        TravelDocumentCreatedOn = travelDocument.CreatedOn,
                        TravelDocumentValidityStartDate = travelDocument.ValidityStartDate,
                        TravelDocumentValidityEndDate = travelDocument.ValidityEndDate,
                        TravelDocumentStatusId = travelDocument.StatusId
                    }
                    : null;

                var petDetail = new
                {
                    Pet = new
                    {
                        PetId = pet.Id,
                        PetName = pet.Name,
                        Species = Enum.GetName(typeof(PetSpecies), pet.SpeciesId),
                        BreedName = pet.Breed?.Name,
                        Sex = Enum.GetName(typeof(PetGender), pet.SexId),
                        DateOfBirth = pet.DOB,
                        ColourName = pet.Colour?.Name,
                        SignificantFeatures = pet.UniqueFeatureDescription,
                        pet.MicrochipNumber,
                        pet.MicrochippedDate
                    },
                    Application = new
                    {
                        mostRecentApplication.Id,
                        mostRecentApplication.ReferenceNumber,
                        mostRecentApplication.DateOfApplication,
                        mostRecentApplication.Status,
                        mostRecentApplication.DateAuthorised,
                        mostRecentApplication.DateRejected,
                        mostRecentApplication.DateRevoked
                    },
                    TravelDocument = travelDocumentDetail
                };

                petDetails.Add(petDetail);
            }

            return petDetails.Any() ? petDetails : null;
        }

    }
}