using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Models;
using Defra.PTS.Checker.Models.Constants;
using Defra.PTS.Checker.Models.SchemaFilters;
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
    private readonly ICheckerService _checkerService;
    private readonly ICheckSummaryService _checkSummaryService;
    private readonly IOrganisationService _organisationService;

    public CheckerController(IApplicationService applicationService, ICheckerService checkerService, ICheckSummaryService checkSummaryService, IOrganisationService organisationService)
    {
        _applicationService = applicationService;
        _checkerService = checkerService;
        _checkSummaryService = checkSummaryService;
        _organisationService = organisationService;
    }

    [HttpPost("checkApplicationNumber")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK: Returns the requested application", typeof(SearchResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request: Application reference number is not provided or is not valid", typeof(IDictionary<string, string>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found: There is no application matching this reference number")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error: An error has occurred")]
    [SwaggerOperation(
            OperationId = "checkApplicationNumber",
            Tags = new[] { "Checker" },
            Summary = "Retrieves a specific application by Reference Number",

            Description = "Returns the application details for the specified Application Number"
        )]
    public async Task<IActionResult> CheckApplicationNumber([FromBody] SearchByApplicationNumberRequest request)
    {
        if (string.IsNullOrEmpty(request.ApplicationNumber))
        {
            return BadRequest(new { error = "Application number is required." });
        }

        if (request.ApplicationNumber.Length > 20)
        {
            return BadRequest(new { error = "Application number cannot exceed 20 characters." });
        }

        var response = await _applicationService.GetApplicationByReferenceNumber(request.ApplicationNumber);

        if (response == null)
            return new NotFoundObjectResult(ApiConstants.ApplicationNotFound);

        return Ok(response);
    }

    [HttpPost("checkMicrochipNumber")]
    [ProducesResponseType(typeof(SearchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CheckMicrochipNumber([FromBody] SearchByMicrochipNumberRequest request)
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

            var errorProperty = response.GetType().GetProperty("error")?.GetValue(response, null) as string;
            if (!string.IsNullOrEmpty(errorProperty))
            {
                if (errorProperty == "Pet not found" || errorProperty == ApiConstants.ApplicationNotFound)
                {
                    return NotFound(new { error = errorProperty });
                }
                else
                {
                    return StatusCode(500, new { error = "An unexpected error occurred. Please try again later." });
                }
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred during processing", details = ex.Message });
        }
    }


    [HttpGet]
    [Route("checkPTDNumber")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK: Returns the approved application", typeof(SearchResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request: PTD Number is not provided or is not valid", typeof(IDictionary<string, string>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found: There is no application matching this PTD number")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error: An error has occurred")]
    [SwaggerOperation(
        OperationId = "checkPTDNumber",
        Tags = new[] { "Checker" },
        Summary = "Retrieves a specific application by PTD Number",
        Description = "Returns the application details for the specified Pet Travel Document Number"
    )]
    public async Task<IActionResult> GetApplicationByPTDNumber([FromQuery, SwaggerRequestBody("The search payload", Required = true)] string ptdNumber)
    {
        if (string.IsNullOrWhiteSpace(ptdNumber))
        {
            return BadRequest(new { error = "PTD number is required" });
        }

        var application = await _applicationService.GetApplicationByPTDNumber(ptdNumber);
        if (application == null)
        {
            return new NotFoundObjectResult(ApiConstants.ApplicationNotFound);
        }

        return Ok(application);
    }

    [HttpPost]
    [Route("checkPTDNumber")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK: Returns the approved application", typeof(SearchResponse))]
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

        var application = await _applicationService.GetApplicationByPTDNumber(model.PTDNumber);
        if (application == null)
        {
            return new NotFoundObjectResult(ApiConstants.ApplicationNotFound);
        }

        return Ok(application);
    }

    [HttpPost]
    [Route("CheckOutcome")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK: Returns check summary response", typeof(CheckOutcomeResponseModel))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request: Request is not valid", typeof(IDictionary<string, string>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found: There is no application matching this PTD number")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error: An error has occurred")]
    [SwaggerOperation(
            OperationId = "CheckOutcome",
            Tags = new[] { "Checker" },
            Summary = "Saves checkout",
            Description = "Saves check outcome of pass for a pet travel document"
        )]
    public async Task<IActionResult> SaveCheckOutcome([FromBody, SwaggerRequestBody("The check outcome payload", Required = true)] CheckOutcomeModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var application = await _applicationService.GetApplicationById(model.ApplicationId);
        if (application == null)
        {
            return new NotFoundObjectResult(ApiConstants.ApplicationNotFound);
        }

        var response = await _checkSummaryService.SaveCheckSummary(model);
        return Ok(response);
    }



    [HttpPost]
    [Route("ReportNonCompliance")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK: Returns NonCompliance summary response", typeof(NonComplianceResponseModel))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request: Request is not valid", typeof(IDictionary<string, string>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found: There is no application matching this PTD number")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error: An error has occurred")]
    [SwaggerOperation(
           OperationId = "ReportNonCompliance",
           Tags = new[] { "Checker" },
           Summary = "Saves NonCompliance",
           Description = "Saves NonCompliance for a pet travel document"
       )]
    public async Task<IActionResult> ReportNonCompliance([FromBody, SwaggerRequestBody("The NonCompliance payload", Required = true)] NonComplianceModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var application = await _applicationService.GetApplicationById(model.ApplicationId);
        if (application == null)
        {
            return new NotFoundObjectResult(ApiConstants.ApplicationNotFound);
        }

        var response = await _checkSummaryService.SaveCheckSummary(model);
        return Ok(response);
    }

    [HttpPost]
    [Route("checkerUser")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK: Returns the Id of checker", typeof(Guid))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request: Invalid request", typeof(IDictionary<string, string>))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error: An error has occurred")]
    [SwaggerOperation(
        OperationId = "checkerUser",
        Tags = new[] { "Checker" },
        Summary = "Adds or updates a checker user",
        Description = "Adds or updates a checker user"
    )]
    public async Task<IActionResult> SaveCheckerUser([FromBody, SwaggerRequestBody("The checker user payload", Required = true)] CheckerDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var checkerId = await _checkerService.SaveChecker(model);

        return Ok(checkerId);
    }

    [HttpPost]
    [Route("checkMicrochipNumberExistWithPtd")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK: Returns the bool", typeof(bool))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request: Invalid request", typeof(IDictionary<string, string>))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error: An error has occurred")]
    [SwaggerOperation(
      OperationId = "checkMicrochipNumberExistWithPtd",
      Tags = new[] { "Checker" },
      Summary = "Check Microchip Number Exist With Ptd ",
      Description = "Check Microchip Number Exist With Ptd "
  )]
    public async Task<IActionResult> CheckerMicrochipNumberExistWithPtd([FromBody, SwaggerRequestBody("The checker microchip number payload", Required = true)] CheckerMicrochipDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var exist = await _checkerService.CheckerMicrochipNumberExistWithPtd(model.MicrochipNumber!);

        return Ok(exist);
    }

    [HttpPost("getCheckOutcomes")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK: Returns the check outcomes", typeof(IEnumerable<CheckOutcomeResponse>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request: Invalid request", typeof(object))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error: An error has occurred")]
    public async Task<IActionResult> GetCheckOutcomes([FromBody, SwaggerRequestBody("The checker dashboard start hour and end hour payload", Required = true)] CheckerOutcomeDashboardDto model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors?.Count > 0)
                    .Select(x => new { Field = x.Key, Error = x.Value?.Errors?.FirstOrDefault()?.ErrorMessage })
                    .ToList();

                return BadRequest(new { message = "Validation failed", errors });
            }

            if (!int.TryParse(model.StartHour, out int startHour))
            {
                return BadRequest(new { error = "Invalid StartHour value" });
            }

            if (!int.TryParse(model.EndHour, out int endHour))
            {
                return BadRequest(new { error = "Invalid EndHour value" });
            }

            // Use the server's local time (GMT)
            var referenceDateTime = DateTime.Now;

            var startDate = referenceDateTime.AddHours(startHour);
            var endDate = referenceDateTime.AddHours(endHour);

            

            var results = await _checkSummaryService.GetRecentCheckOutcomesAsync(startDate, endDate);

            if (results == null || !results.Any())
            {
                return NotFound(new { error = "No check outcomes found within the specified time range." });
            }

            return Ok(results);
        }
        catch (Exception ex)
        {            
            return StatusCode(500, new { error = "An error occurred while fetching check outcomes", details = ex.Message });
        }
    }

    [HttpPost("getSpsCheckDetailsByRoute")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK: Returns SPS check details", typeof(IEnumerable<SpsCheckDetailResponseModel>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request: Invalid request", typeof(object))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error: An error has occurred")]
    public async Task<IActionResult> GetSpsCheckDetailsByRoute([FromBody] SpsCheckRequestModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "Invalid request." });
            }

            var checkDetails = await _checkSummaryService.GetSpsCheckDetailsByRouteAsync(model.Route, model.SailingDate, model.TimeWindowInHours);

            if (!checkDetails.Any())
            {
                return NotFound(new { message = "No SPS check details found." });
            }

            return Ok(checkDetails);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred during processing.", details = ex.Message });
        }
    }


    [HttpPost]
    [Route("getOrganisation")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK: Returns the Organisation", typeof(OrganisationResponseModel))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request: OrganisationId is not provided or is not valid", typeof(object))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found: There is no Organisation matching this OrganisationId")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error: An error has occurred")]
    [SwaggerOperation(
        OperationId = "organisationId",
        Tags = new[] { "Organisation" },
        Summary = "Retrieves a specific Organisation by OrganisationId",
        Description = "Returns the Organisation details for the specified OrganisationId"
    )]
    public async Task<IActionResult> GetOrganisationById([FromBody, SwaggerRequestBody("The search payload", Required = true)] OrganisationRequestModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var organisation = await _organisationService.GetOrganisation(Guid.Parse(model.OrganisationId));
        if (organisation == null)
        {
            return new NotFoundObjectResult(ApiConstants.OrganisationNotFound);
        }

       return Ok(organisation);
    }


    [HttpPost]
    [Route("getGbCheck")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK: Returns the GbCheck", typeof(GbCheckReportResponseModel))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request: GbCheckSummaryId is not provided or is not valid", typeof(object))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found: There is no GbCheck matching this GbCheckSummaryId")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error: An error has occurred")]
    [SwaggerOperation(
     OperationId = "gbCheckSummaryId",
     Tags = new[] { "GbCheck" },
     Summary = "Retrieves a specific GbCheck by GbCheckSummaryId",
     Description = "Returns the GbCheck details for the specified GbCheckSummaryId"
    )]
    public async Task<IActionResult> GetGbCheckById([FromBody, SwaggerRequestBody("The search payload", Required = true)] GbCheckReportRequestModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var checkReport = await _checkSummaryService.GetGbCheckReport(Guid.Parse(model.GbCheckSummaryId));
        if (checkReport == null)
        {
            return new NotFoundObjectResult(ApiConstants.GbCheckReportNotFound);
        }

        return Ok(checkReport);
    }

    [HttpPost]
    [Route("getIsUserSuspendedStatusByEmail")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK: Returns a bool", typeof(bool))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request: Email is not provided or is not valid", typeof(object))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found: There are no checks associated with this email")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error: An error has occurred")]
    [SwaggerOperation(
    OperationId = "getIsUserSuspendedStatus",
    Tags = new[] { "Checker" },
    Summary = "Retrieves a bool for if the user has a suspended application using their email for reference",
    Description = "Returns the suspended status for the specified Email"
    )]
    public async Task<IActionResult> GetIsUserSuspendedStatusByEmail([FromBody, SwaggerRequestBody("The search payload", Required = true)] string email)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _applicationService.GetIsUserSuspendedByEmail(email);

        return Ok(result);
    }


    [HttpPost]
    [Route("getCompleteCheckDetails")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK: Returns the complete check details", typeof(CompleteCheckDetailsResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request: CheckSummaryId is not valid", typeof(object))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found: There is no record matching the provided CheckSummaryId")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error: An error has occurred")]
    [SwaggerOperation(
     OperationId = "getCompleteCheckDetails",
     Tags = new[] { "Checker" },
     Summary = "Retrieves complete check details by CheckSummaryId",
     Description = "Returns complete details for the specified CheckSummaryId"
    )]
    public async Task<IActionResult> GetCompleteCheckDetails(
    [FromBody, SwaggerRequestBody("The search payload", Required = true)] CheckDetailsRequestModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { error = "Invalid input. Please provide a valid CheckSummaryId." });
        }

        var response = await _checkSummaryService.GetCompleteCheckDetailsAsync(model.CheckSummaryId);

        if (response == null)
        {
            return NotFound(new { error = "No data found for the provided CheckSummaryId." });
        }

        return Ok(response);
    }

   
}