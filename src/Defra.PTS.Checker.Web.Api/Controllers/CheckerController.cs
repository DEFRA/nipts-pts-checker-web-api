using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Models;
using Defra.PTS.Checker.Models.Enums;
using Defra.PTS.Checker.Services.Interface;
using Defra.Trade.Address.V1.ApiClient.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.CosmosDB.Fluent.Models;

namespace Defra.PTS.Checker.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckerController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly ITravelDocumentService _travelDocumentService;

        public CheckerController(IApplicationService applicationService, ITravelDocumentService travelDocumentService)
        {
            _applicationService = applicationService;
            _travelDocumentService = travelDocumentService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApplicationDetail), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckApplicationNumber(string referenceNumber)
        {
            var response = await _travelDocumentService.GetTravelDocumentByReferenceNumber(referenceNumber);

            if (response == null)
                return NotFound();

            var sexOfPet = (PetGenderType)response.Pet.SexId;
            var speciesOfPet = (PetSpeciesType)response.Pet.SpeciesId;

            var applicationDetails = new ApplicationDetail
            {
                ReferenceNumber = response.ApplicationId,
                DateOfApplication = response?.Application.DateOfApplication,
                DocumentReferenceNumber = response.DocumentReferenceNumber,
                Status = response?.Application.Status,
                DateOfIssue = response?.DateOfIssue,
                PetName = response.Pet?.Name,
                DateOfBirthOfPet = response.Pet?.DOB,
                MicrochipNumber = response.Pet?.MicrochipNumber,
                MicrochippedDate = response.Pet?.MicrochippedDate,
                SexOfPet = sexOfPet.ToString(),
                SpeciesOfPet = speciesOfPet.ToString(),
                UniqueFeaturesOfPet = response.Pet?.UniqueFeatureDescription,
                BreedName = response.Pet?.Breed?.Name, 
                ColourOfPet = response.Pet?.Colour?.Name
        };

            return Ok(applicationDetails);
        }

    }
}
