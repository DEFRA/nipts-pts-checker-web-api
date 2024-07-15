using Defra.PTS.Checker.Models.Enums;
using Defra.PTS.Checker.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using entity = Defra.PTS.Checker.Entities;

namespace Defra.PTS.Checker.Repositories.Implementation
{
    [ExcludeFromCodeCoverage]
    public class AddressRepository : Repository<entity.Address>, IAddressRepository
    {
        private CommonDbContext addressContext
        {
            get
            {
                return _dbContext as CommonDbContext;
            }
        }

        public AddressRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public async Task<entity.Address> GetAddress(Guid? addressId, AddressType addressType)
        {
            return await addressContext.Address.FirstOrDefaultAsync(a => a.Id == addressId && a.AddressType == addressType.ToString() && a.IsActive == true);
        }
    }
}
