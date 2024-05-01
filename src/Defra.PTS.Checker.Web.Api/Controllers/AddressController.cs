using Defra.PTS.Checker.Services.Interface;
using Defra.Trade.Address.V1.ApiClient.Model;
using Microsoft.AspNetCore.Mvc;

namespace Defra.PTS.Checker.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : Controller
    {
        private readonly IAddressLookupService _addressLookupService;

        public AddressController(IAddressLookupService addressLookupService)
        {
            _addressLookupService = addressLookupService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<AddressDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAddressess(string postcode)
        {
            try
            {
                var response = await _addressLookupService.GetAddressesByPostcode(postcode);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
