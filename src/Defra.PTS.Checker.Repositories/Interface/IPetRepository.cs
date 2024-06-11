using Defra.PTS.Checker.Entities;
using entity = Defra.PTS.Checker.Entities;

namespace Defra.PTS.Checker.Repositories.Interface
{
    public interface IPetRepository : IRepository<entity.Pet>
    {
        Task<IEnumerable<Pet>> GetByMicrochipNumberAsync(string microchipNumber);
    }
}
