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
    /// POST /checker/checkPTDNumber
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /checker/checkPTDNumber
    ///     {
    ///        "ptdNumber": "ABCXYZ"
    ///     }
    ///
    /// </remarks>
    [HttpPost]
    [Route("checkPTDNumber")]
    [ProducesResponseType(typeof(VwApplication), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
            OperationId = "checkPTDNumber",
            Tags = new[] { "Checker" },
            Summary = "Retrieves a specific application by PTD Number",
            Description = "Returns the application details for the specified Pet Travel Document Number"
        )]
    public async Task<IActionResult> SearchApplicationByPTDNumber([FromBody] SearchByPTDNumberModel model)
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

            return new OkObjectResult(application);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "checkPTDNumber: An error has occurred");
            return new StatusCodeResult(500);
        }
    }
}
