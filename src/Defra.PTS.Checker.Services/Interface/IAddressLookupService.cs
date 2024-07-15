using Defra.Trade.Address.V1.ApiClient.Model;

namespace Defra.PTS.Checker.Services.Interface
{
    public interface IAddressLookupService
    {
        Task<List<AddressDto>> GetAddressesByPostcode(string postcode);
    }
}
