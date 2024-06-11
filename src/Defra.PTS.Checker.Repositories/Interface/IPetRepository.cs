using Defra.PTS.Checker.Entities;


namespace Defra.PTS.Checker.Repositories.Interface
{
    public interface IPetRepository : IRepository<Pet>
    {
        Task<IEnumerable<Pet>> GetByMicrochipNumberAsync(string microchipNumber);
    }
}
