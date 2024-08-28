using Defra.PTS.Checker.Services.Interface;
using Defra.PTS.Checker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Defra.PTS.Checker.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColoursController : ControllerBase
    {
        private readonly IColourService _colour;
        public ColoursController(IColourService colour)
        {
            _colour = colour;
        }
        // GET: api/<ColoursController>        
        [HttpGet]
        [ProducesResponseType(typeof(ColourResponse), StatusCodes.Status200OK)]        
        public async Task<IActionResult> GetAllColours()
        {
            var response = await _colour.GetAllColours();

            return response == null
                ? NotFound()
                : Ok(response);
        }
    }
}
