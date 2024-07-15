using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Models;
using Defra.PTS.Checker.Repositories.Interface;
using Defra.PTS.Checker.Services.Interface;
using Microsoft.Extensions.Logging;

namespace Defra.PTS.Checker.Services.Implementation
{
    public class SailingService : ISailingService
    {
        private readonly IRepository<Route> _sailingRepository;
        private ILogger<SailingService> _log;
        public SailingService(ILogger<SailingService> log, IRepository<Route> sailingRepository)
        {
            _log = log;
            _sailingRepository = sailingRepository;
        }

        public async Task<IEnumerable<RouteResponse>> GetAllSailingRoutes()
        {
            IEnumerable<Route> routes = await _sailingRepository.GetAllAsync();

            return routes.Select(route => new RouteResponse
            {
                Id = route.Id,
                RouteName = route.RouteName,
            });
        }
    }
}
