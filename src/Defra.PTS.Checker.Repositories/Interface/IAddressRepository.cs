using Defra.PTS.Checker.Models.Enums;
using entity = Defra.PTS.Checker.Entities;

namespace Defra.PTS.Checker.Repositories.Interface
{
    public interface IAddressRepository : IRepository<entity.Address>
    {
        Task<entity.Address> GetAddress(Guid? addressId, AddressType addressType);
    }
}
