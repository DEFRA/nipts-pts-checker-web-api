using Defra.PTS.Checker.Models;

namespace Defra.PTS.Checker.Services.Interface
{
    public interface ISailingService
    {
        Task<IEnumerable<RouteResponse>> GetAllSailingRoutes();
    }
}
