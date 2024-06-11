using Defra.PTS.Checker.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Defra.PTS.Checker.Web.Api.Controllers
{
    [Route("api/checker")]
    [ApiController]
    public class CheckerController : ControllerBase
    {
        private readonly ICheckerService _checkerService;

        public CheckerController(ICheckerService checkerService)
        {
            _checkerService = checkerService;
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
        public string MicrochipNumber { get; set; }
    }
}
