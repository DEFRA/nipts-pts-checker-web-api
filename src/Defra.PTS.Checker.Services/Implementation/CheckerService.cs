using Defra.PTS.Checker.Entities;
using models = Defra.PTS.Checker.Models;
using Defra.PTS.Checker.Repositories.Interface;
using Defra.PTS.Checker.Services.Enums;
using Defra.PTS.Checker.Services.Interface;
using Microsoft.Extensions.Logging;

public class CheckerService : ICheckerService
{
    private readonly IPetRepository _petRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly ITravelDocumentRepository _travelDocumentRepository;
    private readonly ICheckerRepository _checkerRepository;
    private readonly ILogger<CheckerService> _logger;

    public CheckerService(
        IPetRepository petRepository,
        IApplicationRepository applicationRepository,
        ITravelDocumentRepository travelDocumentRepository,
        ICheckerRepository checkerRepository,
        ILogger<CheckerService> logger)
    {
        _petRepository = petRepository;
        _applicationRepository = applicationRepository;
        _travelDocumentRepository = travelDocumentRepository;
        _checkerRepository = checkerRepository;
        _logger = logger;
    }

    public async Task<Guid> SaveChecker(models.CheckerDto checkerDto)
    {
        var entity = await _checkerRepository.Find(checkerDto.Id);
        if (entity == null)
        {
            entity = new Checker
            {
                Id = checkerDto.Id,
                FirstName = checkerDto.FirstName,
                LastName = checkerDto.LastName,
                FullName = $"{checkerDto.FirstName} {checkerDto.LastName}",
                RoleId = checkerDto.RoleId,
            };

            await _checkerRepository.Add(entity);
        }
        else
        {
            entity.FirstName = checkerDto.FirstName;
            entity.LastName = checkerDto.LastName;
            entity.FullName = $"{checkerDto.FirstName} {checkerDto.LastName}";
            entity.RoleId = checkerDto.RoleId;

            _checkerRepository.Update(entity);
        }

        await _checkerRepository.SaveChanges();

        return entity.Id;
    }

    public async Task<object?> CheckMicrochipNumberAsync(string microchipNumber)
    {
        try
        {
            var pets = await _petRepository.GetByMicrochipNumberAsync(microchipNumber);
            if (!pets.Any())
            {
                _logger.LogInformation("No pets found with microchip number: {MicrochipNumber}", microchipNumber);
                return new { error = "Pet not found" };
            }

            var allApplications = new List<Application>();

            foreach (var pet in pets)
            {
                _logger.LogInformation("Processing pet with ID: {PetId}", pet.Id);

                var applications = await _applicationRepository.GetApplicationsByPetIdAsync(pet.Id);
                if (applications.Any())
                {
                    allApplications.AddRange(applications);
                }
            }

            if (allApplications.Any())
            {
                var mostRelevantApplication = GetMostRelevantApplication(allApplications);
                if (mostRelevantApplication != null)
                {
                    var pet = pets.First(p => p.Id == mostRelevantApplication.PetId);
                    var travelDocument = await _travelDocumentRepository.GetTravelDocumentByApplicationIdAsync(mostRelevantApplication.Id);
                    var travelDocumentDetail = travelDocument != null
                        ? new
                        {
                            TravelDocumentId = travelDocument.Id,
                            TravelDocumentReferenceNumber = travelDocument.DocumentReferenceNumber,
                            TravelDocumentDateOfIssue = travelDocument.DateOfIssue,
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
                            ApplicationId = mostRelevantApplication.Id,
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

            _logger.LogInformation("No relevant applications found for the pets with microchip number: {MicrochipNumber}", microchipNumber);
            return new { error = "Application not found" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while checking microchip number: {MicrochipNumber}", microchipNumber);
            return new { error = "An unexpected error occurred. Please try again later." };
        }
    }

    private static Application? GetMostRelevantApplication(IEnumerable<Application> applications)
    {
        var statusPriority = new Dictionary<string, int>
        {
            { "authorised", 1 },
            { "revoked", 2 },
            { "awaiting verification", 3 },
            { "rejected", 4 }
        };

        return applications
            .OrderBy(a => statusPriority.ContainsKey(a.Status!.ToLower()) ? statusPriority[a.Status.ToLower()] : int.MaxValue)
            .ThenByDescending(a => a.Status!.ToLower() switch
            {
                "authorised" => a.DateAuthorised,
                "revoked" => a.DateRevoked,
                "awaiting verification" => a.CreatedOn,
                "rejected" => a.DateRejected,
                _ => DateTime.MinValue
            })
            .FirstOrDefault();
    }
}
