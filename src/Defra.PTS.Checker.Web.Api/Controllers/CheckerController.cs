using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Models.Enums;
using Defra.PTS.Checker.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Defra.PTS.Checker.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckerController : ControllerBase
    {
        private readonly ITravelDocumentService _travelDocumentService;
        private readonly ICheckerService _checkerService;

        public CheckerController(ITravelDocumentService travelDocumentService, ICheckerService checkerService)
        {
            _travelDocumentService = travelDocumentService;
            _checkerService = checkerService;
        }

        [HttpPost]
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

        [HttpPost("checkMicrochipNumber")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CheckMicrochipNumber([FromBody] MicrochipCheckRequest request)
        {
            if (string.IsNullOrEmpty(request.MicrochipNumber))
            {
                return BadRequest(new { error = "Microchip number is required" });
            }

            try
            {
                var response = await _checkerService.CheckMicrochipNumberAsync(request.MicrochipNumber);

                if (response == null)
                {
                    return NotFound(new { error = "Application or travel document not found" });
                }


                return Ok(System.Text.Json.JsonSerializer.Serialize(response));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred during processing", details = ex.Message });
            }
        }
    }

    public class MicrochipCheckRequest
    {
        public string? MicrochipNumber { get; set; }
    }
}

