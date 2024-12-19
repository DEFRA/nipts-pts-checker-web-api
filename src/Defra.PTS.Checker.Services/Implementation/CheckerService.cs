using Defra.PTS.Checker.Entities;
using Models = Defra.PTS.Checker.Models;
using Defra.PTS.Checker.Repositories.Interface;
using Defra.PTS.Checker.Services.Enums;
using Defra.PTS.Checker.Services.Interface;
using Microsoft.Extensions.Logging;

namespace Defra.PTS.Checker.Services.Implementation;

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

    public async Task<Guid> SaveChecker(Models.CheckerDto checkerDto)
    {
        var entity = await _checkerRepository.Find(checkerDto.Id);
        if (entity == null)
        {
            entity = new Entities.Checker
            {
                Id = checkerDto.Id,
                FirstName = checkerDto.FirstName,
                LastName = checkerDto.LastName,
                FullName = $"{checkerDto.FirstName} {checkerDto.LastName}",
                RoleId = checkerDto.RoleId,
                OrganisationId = checkerDto.OrganisationId,
        };

            await _checkerRepository.Add(entity);
        }
        else
        {
            entity.FirstName = checkerDto.FirstName;
            entity.LastName = checkerDto.LastName;
            entity.FullName = $"{checkerDto.FirstName} {checkerDto.LastName}";
            entity.RoleId = checkerDto.RoleId;
            entity.OrganisationId = checkerDto.OrganisationId;

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

            foreach (var id in pets.Select(x => x.Id))
            {
                _logger.LogInformation("Processing pet with ID: {PetId}", id);

                var applications = await _applicationRepository.GetApplicationsByPetIdAsync(id);
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

                    var petOwnerAddress = mostRelevantApplication?.OwnerAddress != null
                        ? new
                        {
                            AddressLineOne = mostRelevantApplication.OwnerAddress.AddressLineOne,
                            AddressLineTwo = mostRelevantApplication.OwnerAddress.AddressLineTwo,
                            TownOrCity = mostRelevantApplication.OwnerAddress.TownOrCity,
                            County = mostRelevantApplication.OwnerAddress.County,
                            PostCode = mostRelevantApplication.OwnerAddress.PostCode,
                        }
                        : null;

                    var petOwner =
                        new
                        {
                            Name = mostRelevantApplication?.OwnerNewName,
                            Telephone = mostRelevantApplication?.OwnerNewTelephone,
                            Email = mostRelevantApplication?.Owner != null ? mostRelevantApplication.Owner.Email : null,
                            Address = petOwnerAddress,
                        };

                    var petDetail = new
                    {
                        Pet = new
                        {
                            PetId = pet.Id,
                            PetName = pet.Name,
                            Species = Enum.GetName(typeof(PetSpecies), pet.SpeciesId),
                            BreedName = pet.Breed?.Name,
                            BreedAdditionalInfo = pet.AdditionalInfoMixedBreedOrUnknown,
                            Sex = Enum.GetName(typeof(PetGender), pet.SexId),
                            DateOfBirth = pet.DOB,
                            ColourName = getColourByPet(pet),
                            SignificantFeatures = getSignificantFeaturesByPet(pet),
                            pet.MicrochipNumber,
                            pet.MicrochippedDate
                        },
                        Application = new
                        {
                            ApplicationId = mostRelevantApplication?.Id,
                            mostRelevantApplication?.ReferenceNumber,
                            mostRelevantApplication?.DateOfApplication,
                            mostRelevantApplication?.Status,
                            mostRelevantApplication?.DateAuthorised,
                            mostRelevantApplication?.DateRejected,
                            mostRelevantApplication?.DateRevoked
                        },
                        TravelDocument = travelDocumentDetail,
                        PetOwner = petOwner
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


    private static string? getColourByPet(Pet pet)
    {
        return !string.IsNullOrEmpty(pet.OtherColour) ? pet.OtherColour : pet.Colour?.Name;
    }

    private static string? getSignificantFeaturesByPet(Pet pet)
    {
        return pet.HasUniqueFeature == (int)YesNoOptions.Yes ? pet.UniqueFeatureDescription : "No";
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

    public async Task<bool> CheckerMicrochipNumberExistWithPtd(string microchipNumber)
    {
        try
        {
            var pets = await _petRepository.GetByMicrochipNumberAsync(microchipNumber);
            if (!pets.Any())
            {
                _logger.LogInformation("No pets found with microchip number: {MicrochipNumber}", microchipNumber);
                return false;
            }

            var hasTravelDocument = await Task.WhenAny(
                pets.Select(async pet =>
                {
                    _logger.LogInformation("Processing pet with ID: {PetId}", pet.Id);

                    var applications = await _applicationRepository.GetApplicationsByPetIdAsync(pet.Id);

                    return await Task.WhenAny(
                        applications.Select(async application =>
                        {
                            var travelDocument = await _travelDocumentRepository.GetTravelDocumentByApplicationIdAsync(application.Id);
                            return travelDocument != null;
                        })
                    ).Result;
                })
            ).Result;

            if (hasTravelDocument)
            {
                return true;
            }

            // No travel documents found for any of the pet's applications
            _logger.LogInformation("No travel documents found for pets with microchip number: {MicrochipNumber}", microchipNumber);

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while checking microchip number: {MicrochipNumber}", microchipNumber);
            return false;
        }
    }

}
