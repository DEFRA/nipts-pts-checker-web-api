using Azure;
using Defra.PTS.Checker.Models;
using Defra.PTS.Checker.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Defra.PTS.Checker.Web.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class SailingController : ControllerBase
    {
        private readonly ISailingService _sailingService; 
        public SailingController(ISailingService sailingService)
        {
            _sailingService = sailingService;
        }

        [HttpGet]
        [Route("sailing-routes")]
        [ProducesResponseType(typeof(IEnumerable<RouteResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllSailingRoutes()
        {
            var sailingRoutes = await _sailingService.GetAllSailingRoutes();

            return sailingRoutes == null
                ? NotFound()
                : Ok(sailingRoutes);
        }
    }
}
