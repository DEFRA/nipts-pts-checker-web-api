using Azure;
using Defra.PTS.Checker.Models;
using Defra.PTS.Checker.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Defra.PTS.Checker.Web.Api.Controllers
{
    /// <summary>
    /// Provides endpoints for retrieving sailing route reference data.
    /// </summary>
    [Route("api")]
    [ApiController]
    public class SailingController : ControllerBase
    {
        private readonly ISailingService _sailingService; 
        /// <summary>
        /// Initialises a new instance of the <see cref="SailingController"/> class.
        /// </summary>
        /// <param name="sailingService">The sailing service.</param>
        public SailingController(ISailingService sailingService)
        {
            _sailingService = sailingService;
        }

        /// <summary>
        /// Retrieves all available sailing routes.
        /// </summary>
        /// <returns>The list of sailing routes, or a not found result.</returns>
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
