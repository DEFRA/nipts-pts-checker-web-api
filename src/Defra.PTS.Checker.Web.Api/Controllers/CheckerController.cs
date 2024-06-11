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
