using Entity = Defra.PTS.Checker.Entities;

namespace Defra.PTS.Checker.Repositories.Interface
{
    public interface IUserRepository : IRepository<Entity.User>
    {
        Task<(Guid?, Guid?, string)> GetUserDetails(Guid contactId);
        Task<bool> DoesUserExists(Guid contactId);
        Task<bool> DoesAddresssExists(Guid addressId);

        Task<Entity.User> GetUser(string userEmailAddress);
    }
}
