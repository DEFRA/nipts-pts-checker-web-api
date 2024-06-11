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
        private readonly ITravelDocumentService _travelDocumentService;

        public CheckerController(ITravelDocumentService travelDocumentService)
        {
            _travelDocumentService = travelDocumentService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApplicationDetail), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> CheckApplicationNumber(string referenceNumber)
        {

            if (!referenceNumber.StartsWith("GB") || referenceNumber.Length > 20)
                return BadRequest(ModelState);

            var response = await _travelDocumentService.GetTravelDocumentByReferenceNumber(referenceNumber);

            if (response == null)
                return NotFound();

            var sexOfPet = (PetGenderType)response.Pet!.SexId;
            var speciesOfPet = (PetSpeciesType)response.Pet.SpeciesId;

            var applicationDetails = new ApplicationDetail
            {
                ReferenceNumber = response.ApplicationId,
                DateOfApplication = response.Application?.DateOfApplication,
                DocumentReferenceNumber = response.DocumentReferenceNumber,
                Status = response.Application?.Status,
                DateOfIssue = response.DateOfIssue,
                PetName = response.Pet?.Name,
                DateOfBirthOfPet = response.Pet?.DOB,
                MicrochipNumber = response.Pet?.MicrochipNumber,
                MicrochippedDate = response.Pet?.MicrochippedDate,
                SexOfPet = sexOfPet.ToString(),
                SpeciesOfPet = speciesOfPet.ToString(),
                UniqueFeaturesOfPet = response.Pet?.UniqueFeatureDescription,
                BreedName = response.Pet?.Breed?.Name, 
                ColourOfPet = response.Pet?.Colour?.Name!
        };

            return Ok(applicationDetails);
        }

    }
}
