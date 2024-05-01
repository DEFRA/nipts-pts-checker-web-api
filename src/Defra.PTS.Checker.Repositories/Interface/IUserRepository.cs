using entity = Defra.PTS.Checker.Entities;

namespace Defra.PTS.Checker.Repositories.Interface
{
    public interface IUserRepository : IRepository<entity.User>
    {
        Task<(Guid?, Guid?, string)> GetUserDetails(Guid contactId);
        Task<bool> DoesUserExists(Guid contactId);
        Task<bool> DoesAddresssExists(Guid addressId);

        Task<entity.User> GetUser(string userEmailAddress);
    }
}
