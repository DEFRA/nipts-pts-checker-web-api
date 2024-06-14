using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Repositories.Interface;
using Defra.PTS.Checker.Services.Enums;
using Defra.PTS.Checker.Services.Interface;
using Microsoft.Extensions.Logging;

public class CheckerService : ICheckerService
{
    private readonly IPetRepository _petRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly ITravelDocumentRepository _travelDocumentRepository;
    private readonly ILogger<CheckerService> _logger;

    public CheckerService(
        IPetRepository petRepository,
        IApplicationRepository applicationRepository,
        ITravelDocumentRepository travelDocumentRepository,
        ILogger<CheckerService> logger)
    {
        _petRepository = petRepository;
        _applicationRepository = applicationRepository;
        _travelDocumentRepository = travelDocumentRepository;
        _logger = logger;
    }

    public async Task<object?> CheckMicrochipNumberAsync(string microchipNumber)
    {
        var pets = await _petRepository.GetByMicrochipNumberAsync(microchipNumber);
        if (!pets.Any())
        {
            return null;
        }

        foreach (var pet in pets)
        {
            var mostRelevantApplication = await GetMostRelevantApplication(pet.Id);
            if (mostRelevantApplication != null)
            {
                var travelDocument = await _travelDocumentRepository.GetTravelDocumentByApplicationIdAsync(mostRelevantApplication.Id);
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
                        mostRelevantApplication.Id,
                        mostRelevantApplication.ReferenceNumber,
                        mostRelevantApplication.DateOfApplication,
                        mostRelevantApplication.Status,
                        mostRelevantApplication.DateAuthorised,
                        mostRelevantApplication.DateRejected,
                        mostRelevantApplication.DateRevoked
                    },
                    TravelDocument = travelDocumentDetail
                };

                return petDetail;
            }
        }

        return null;
    }

    private async Task<Application?> GetMostRelevantApplication(Guid petId)
    {
        var applications = await _applicationRepository.GetApplicationsByPetIdAsync(petId);

        if (applications.Any())
        {
            _logger.LogInformation("Applications found for pet ID: {PetId}. Details: {Applications}", petId, applications);
        }
        else
        {
            _logger.LogWarning("No applications found for pet ID: {PetId}", petId);
            return null;
        }

        // Separate ordering for DateAuthorised and DateRevoked
        var authorisedOrRevokedApplication = applications
            .Where(a => a.Status == "Authorised" || a.Status == "Revoked")
            .OrderByDescending(a => a.DateAuthorised)
            .ThenByDescending(a => a.DateRevoked)
            .FirstOrDefault();

        if (authorisedOrRevokedApplication != null)
        {
            _logger.LogInformation("Most relevant authorised or revoked application for pet ID: {PetId} is: {Application}", petId, authorisedOrRevokedApplication);
            return authorisedOrRevokedApplication;
        }

        var awaitingVerificationApplication = applications
            .Where(a => a.Status == "Awaiting Verification")
            .OrderByDescending(a => a.CreatedOn)
            .FirstOrDefault();

        if (awaitingVerificationApplication != null)
        {
            _logger.LogInformation("Most relevant awaiting verification application for pet ID: {PetId} is: {Application}", petId, awaitingVerificationApplication);
            return awaitingVerificationApplication;
        }

        var rejectedApplication = applications
            .Where(a => a.Status == "Rejected")
            .OrderByDescending(a => a.DateRejected)
            .FirstOrDefault();

        if (rejectedApplication != null)
        {
            _logger.LogInformation("Most relevant rejected application for pet ID: {PetId} is: {Application}", petId, rejectedApplication);
        }

        return rejectedApplication;
    }

}
