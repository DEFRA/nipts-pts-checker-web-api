using entity = Defra.PTS.Checker.Entities;

namespace Defra.PTS.Checker.Repositories.Interface
{
    public interface IOwnerRepository : IRepository<entity.Owner>
    {
        Task<bool> DoesOwnerExists(string ownerEmailAddress);

        Task<entity.Owner> GetOwner(Guid ownerId);
    }
}
