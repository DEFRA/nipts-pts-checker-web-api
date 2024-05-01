using Defra.PTS.Checker.Entities;

namespace Defra.PTS.Checker.Services.Interface
{
    public interface IColorService
    {
        Task<IEnumerable<Color>> GetColor();
    }
}
