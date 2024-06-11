using Defra.PTS.Checker.Entities;

namespace Defra.PTS.Checker.Services.Interface
{
    public interface IColourService
    {
        Task<IEnumerable<Colour>> GetAllColours();
    }
}
