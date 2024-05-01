using Defra.PTS.Checker.Services.Interface;
using Defra.PTS.Checker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Defra.PTS.Checker.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorsController : ControllerBase
    {
        private readonly IColorService _color;
        public ColorsController(IColorService color)
        {
            _color = color;
        }
        // GET: api/<ColorsController>        
        [HttpGet]
        [ProducesResponseType(typeof(ColorResponse), StatusCodes.Status200OK)]        
        public async Task<IActionResult> GetAllColors()
        {
            var response = await _color.GetColor();

            return response == null
                ? NotFound()
                : Ok(response);
        }


        // GET api/<ColorsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ColorsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ColorsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ColorsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
