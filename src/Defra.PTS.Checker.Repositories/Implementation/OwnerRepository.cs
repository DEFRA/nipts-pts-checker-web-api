using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Entity = Defra.PTS.Checker.Entities;

namespace Defra.PTS.Checker.Repositories.Implementation
{
    [ExcludeFromCodeCoverage]
    public class OwnerRepository : Repository<Entity.Owner>, IOwnerRepository
    {

        private CommonDbContext? UserContext
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
           return await UserContext!.Owner.AnyAsync(a => a.Email == ownerEmailAddress);
        }

        public async Task<Owner> GetOwner(Guid ownerId)
        {
            return await UserContext!.Owner.FirstOrDefaultAsync(a => a.Id == ownerId) ?? null!;
        }
    }
}
