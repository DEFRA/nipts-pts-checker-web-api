using Defra.PTS.Checker.Entities;
using Defra.Trade.Address.V1.ApiClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.PTS.Checker.Services.Interface
{
    public interface IAddressLookupService
    {
        Task<List<AddressDto>> GetAddressesByPostcode(string postcode);
    }
}
