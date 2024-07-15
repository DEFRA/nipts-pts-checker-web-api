using Defra.PTS.Checker.Models;
using Defra.PTS.Checker.Services.Interface;
using Microsoft.AspNetCore.Mvc;

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


        // GET api/<ColoursController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ColoursController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ColoursController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ColoursController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
