
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Defra.PTS.Checker.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        private static readonly Dictionary<int, object> SampleData = new Dictionary<int, object>
        {
            { 1, new { Id = 1, Name = "John Doe", Age = 30 } },
            { 2, new { Id = 2, Name = "Jane Smith", Age = 25 } },
            { 3, new { Id = 3, Name = "Sam Brown", Age = 20 } }
        };

        [HttpPost]
        [Route("GetDataById")]
        public IActionResult GetDataById([FromBody] RequestModel request)
        {
            try
            {
                if (request.Id == null)
                {
                    return BadRequest(new { ErrorMessage = "ID is required" });
                }

                // Simulate a 500 error for a specific ID, e.g., ID = 500
                if (request.Id.Value == 500)
                {
                    throw new Exception("Simulated internal server error.");
                }

                if (SampleData.ContainsKey(request.Id.Value))
                {
                    return Ok(SampleData[request.Id.Value]);
                }
                else
                {
                    return NotFound(new { ErrorMessage = "ID not found" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ErrorMessage = "An internal server error occurred", Details = ex.Message });
            }
        }
    }

    public class RequestModel
    {
        public int? Id { get; set; }
    }
}
