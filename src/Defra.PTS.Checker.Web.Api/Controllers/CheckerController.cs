using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Models.Enums;
using Defra.PTS.Checker.Models.Search;
using Defra.PTS.Checker.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Defra.PTS.Checker.Web.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CheckerController : ControllerBase
{
    private readonly ITravelDocumentService _travelDocumentService;
    private readonly IApplicationService _applicationService;
    private readonly ICheckerService _checkerService;

    public CheckerController(ITravelDocumentService travelDocumentService, IApplicationService applicationService, ICheckerService checkerService)
    {
        _travelDocumentService = travelDocumentService;
        _applicationService = applicationService;
        _checkerService = checkerService;
    }

    [HttpPost("checkApplicationNumber")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK: Returns the requested application", typeof(ApplicationDetail))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request: Application reference number is not provided or is not valid", typeof(IDictionary<string, string>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found: There is no application matching this reference number")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error: An error has occurred")]
    [SwaggerOperation(
            OperationId = "CheckApplicationNumber",
            Tags = new[] { "Checker" },
            Summary = "Retrieves a specific application by Reference Number",
            Description = "Returns the application details for the specified Application Number"
        )]
    public async Task<IActionResult> CheckApplicationNumber([FromBody] ApplicationNumberCheckRequest request)
    {
        if (string.IsNullOrEmpty(request.ApplicationNumber) || request.ApplicationNumber.Length > 20)
            return BadRequest(ModelState);

        var response = await _applicationService.GetApplicationByReferenceNumber(request.ApplicationNumber);


        if (response == null)
            return new NotFoundObjectResult("Application not found");


        var travelDocument = _travelDocumentService.GetTravelDocumentByApplicationId(response.Id).Result;

        var sexOfPet = (PetGenderType)response.Pet!.SexId;
        var speciesOfPet = (PetSpeciesType)response.Pet.SpeciesId;


        var applicationDetails = new ApplicationDetail
        {
            ReferenceNumber = response.Id,
            DateOfApplication = response.DateOfApplication,
            DocumentReferenceNumber = travelDocument.DocumentReferenceNumber,
            Status = response.Status,
            DateOfIssue = travelDocument.DateOfIssue,
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

        return new OkObjectResult(applicationDetails);
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

    [HttpPost]
    [Route("checkPTDNumber")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK: Returns the approved application", typeof(SearchByPtdNumberResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request: PTD Number is not provided or is not valid", typeof(IDictionary<string, string>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found: There is no application matching this PTD number")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error: An error has occurred")]
    [SwaggerOperation(
            OperationId = "checkPTDNumber",
            Tags = new[] { "Checker" },
            Summary = "Retrieves a specific application by PTD Number",
            Description = "Returns the application details for the specified Pet Travel Document Number"
        )]
    public async Task<IActionResult> GetApplicationByPTDNumber([FromBody, SwaggerRequestBody("The search payload", Required = true)] SearchByPtdNumberRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var document = await _travelDocumentService.GetTravelDocumentByPTDNumber(model.PTDNumber);
        if (document == null)
        {
            return new NotFoundObjectResult("Application not found");
        }

        var pet = document.Pet ?? new Pet();
        var application = document.Application ?? new Application();

        var response = new SearchByPtdNumberResponse
        {
            DocumentReferenceNumber = document.DocumentReferenceNumber ?? string.Empty,
            DateOfIssue = document.DateOfIssue,
            Status = application.Status ?? string.Empty,
            MicrochipNumber = pet.MicrochipNumber ?? string.Empty,
            MicrochippedDate = pet.MicrochippedDate,
            Name = pet.Name ?? string.Empty,
            Breed = pet.Breed?.Name ?? string.Empty,
            DOB = pet.DOB,
            Colour = pet.Colour?.Name ?? string.Empty,
            Sex = (PetGenderType)pet.SexId,
            SpeciesId = (PetSpeciesType)pet.SpeciesId,
            UniqueFeatureDescription = pet.UniqueFeatureDescription ?? string.Empty,
        };

        return new OkObjectResult(response);
    }
}

public class MicrochipCheckRequest
{
    public string? MicrochipNumber { get; set; }
}

public class ApplicationNumberCheckRequest
{
    public string? ApplicationNumber { get; set; }
}


