using Defra.PTS.Checker.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using entity = Defra.PTS.Checker.Entities;

namespace Defra.PTS.Checker.Repositories.Implementation
{
    [ExcludeFromCodeCoverage]
    public class UserRepository : Repository<entity.User>, IUserRepository
    {

        private CommonDbContext userContext
        {
            get
            {
                return _dbContext as CommonDbContext;
            }
        }

        public UserRepository(Microsoft.EntityFrameworkCore.DbContext dbContext) : base(dbContext)
        {
        }

        public async Task<bool> DoesUserExists(Guid contactId)
        {
            var reu = userContext.User.FirstOrDefault();
           return await userContext.User.AnyAsync(a => a.ContactId == contactId);
        }

        public async Task<bool> DoesAddresssExists(Guid addressId)
        {
            return await userContext.Address.AnyAsync(a => a.Id == addressId);
        }
        
        public async Task<(Guid?, Guid?, string)> GetUserDetails(Guid contactId)
        {
            var user = await userContext.User.FirstOrDefaultAsync(a => a.ContactId == contactId);
            return user != null ? (user.Id, user.AddressId, user.Email) : (Guid.Empty, Guid.Empty, string.Empty);
        }

        public async Task<entity.User> GetUser(string userEmailAddress)
        {
            return await userContext.User.SingleOrDefaultAsync(a => a.Email == userEmailAddress);
        }
    }
}
