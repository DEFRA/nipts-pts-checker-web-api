using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using entity = Defra.PTS.Checker.Entities;

namespace Defra.PTS.Checker.Repositories.Implementation
{
    [ExcludeFromCodeCoverage]
    public class OwnerRepository : Repository<entity.Owner>, IOwnerRepository
    {

        private CommonDbContext userContext
        {
            get
            {
                return _dbContext as CommonDbContext;
            }
        }

        public OwnerRepository(Microsoft.EntityFrameworkCore.DbContext dbContext) : base(dbContext)
        {
        }

        public async Task<bool> DoesOwnerExists(string ownerEmailAddress)
        {
           return await userContext.Owner.AnyAsync(a => a.Email == ownerEmailAddress);
        }

        public async Task<Owner> GetOwner(Guid ownerId)
        {
            return await userContext.Owner.FirstOrDefaultAsync(a => a.Id == ownerId);
        }
    }
}
