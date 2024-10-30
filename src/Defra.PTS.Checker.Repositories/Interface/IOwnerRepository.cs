using Entity = Defra.PTS.Checker.Entities;

namespace Defra.PTS.Checker.Repositories.Interface
{
    public interface IOwnerRepository : IRepository<Entity.Owner>
    {
        Task<bool> DoesOwnerExists(string ownerEmailAddress);

        Task<Entity.Owner> GetOwner(Guid ownerId);
    }
}
