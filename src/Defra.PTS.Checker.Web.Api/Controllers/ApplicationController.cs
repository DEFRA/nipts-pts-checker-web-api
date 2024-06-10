using Defra.PTS.Checker.Models;
using Defra.PTS.Checker.Services.Interface;
using Defra.Trade.Address.V1.ApiClient.Model;
using Microsoft.AspNetCore.Mvc;

namespace Defra.PTS.Checker.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public ApplicationController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        // GET: api/<ColoursController>        
        [HttpGet]
        [ProducesResponseType(typeof(Application), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetApplicationById(Guid id)
        {
            var response = await _applicationService.GetApplicationById(id);

            return response == null
                ? NotFound()
                : Ok(response);
        }

    }
}
