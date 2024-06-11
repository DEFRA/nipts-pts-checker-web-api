using Defra.PTS.Checker.Entities;
using entity = Defra.PTS.Checker.Entities;

namespace Defra.PTS.Checker.Repositories.Interface
{
    public interface IApplicationRepository : IRepository<entity.Application>
    {
        Task<entity.Application> GetApplicationById(Guid applicationId);
        Task<VwApplication?> GetApplicationByPTDNumber(string ptdNumber);

        Task<bool> PerformHealthCheckLogic();
    }
}
