using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Defra.PTS.Checker.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        // GET: api/<OwnerController>
        [HttpGet]
        public IActionResult GetAllOwners()
        {
            string[] owners = new string[] { "value1", "value2" };
            return Ok(owners);
        }

        // GET api/<OwnerController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<OwnerController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<OwnerController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<OwnerController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
