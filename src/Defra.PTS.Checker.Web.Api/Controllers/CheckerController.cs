using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Models.Search;
using Defra.PTS.Checker.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Defra.PTS.Checker.Web.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CheckerController : ControllerBase
{
    private readonly IApplicationService _applicationService;
    private readonly ILogger<CheckerController> _logger;

    public CheckerController(IApplicationService applicationService, ILogger<CheckerController> logger)
    {
        _applicationService = applicationService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a specific application by PTD Number
    /// </summary>
    /// <param name="model">The request</param>
    /// <returns>An approved application</returns>
    /// <remarks>
    /// Returns the application details for the specified Pet Travel Document Number
    /// 
    /// Sample request:
    ///
    ///     POST /checker/checkPTDNumber
    ///     {
    ///        "ptdNumber": "GB826CD186E"
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Ok: Returns the approved application</response>
    /// <response code="400">Bad Request: PTD Number is not provided or is not valid</response>
    /// <response code="404">Not Found: There is no application matching this PTD number</response>
    /// <response code="500">Internal Server Error: An error has occurred</response>
    [HttpPost]
    [Route("checkPTDNumber")]
    [ProducesResponseType(typeof(SearchByPTDNumberResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
            OperationId = "checkPTDNumber",
            Tags = new[] { "Checker" }
        )]
    public async Task<IActionResult> SearchApplicationByPTDNumber([FromBody, SwaggerRequestBody("The PTD Numberd", Required = true)] SearchByPTDNumberRequest model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var application = await _applicationService.GetApplicationByPTDNumber(model.PTDNumber);
            if (application == null)
            {
                return new NotFoundObjectResult("PTD Number not found");
            }

            var response = new SearchByPTDNumberResponse
            {
            };

            return new OkObjectResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "checkPTDNumber: An error has occurred");
            return new StatusCodeResult(500);
        }
    }
}
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
