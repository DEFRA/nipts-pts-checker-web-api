using Defra.PTS.Checker.Services.Interface;
using Defra.Trade.Address.V1.ApiClient.Api;
using Defra.Trade.Address.V1.ApiClient.Model;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Services.Implementation
{

    public class AddressLookupService : IAddressLookupService
    {
        private readonly IPlacesApi _placesApi;

        public AddressLookupService(IPlacesApi placesApi)
        {
            _placesApi = placesApi;
        }

        [ExcludeFromCodeCoverage]
        public async Task<List<AddressDto>> GetAddressesByPostcode(string postcode)
        {
            var addressList = await _placesApi.PostCodeLookupAsync(postcode);

            return addressList;
        }
    }
}
