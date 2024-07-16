using Defra.PTS.Checker.Entities;

namespace Defra.PTS.Checker.Repositories.Interface
{
    public interface IApplicationRepository : IRepository<Application>
    {      

        Task<Application> GetApplicationById(Guid applicationId);

        Task<Application?> GetApplicationByReferenceNumber(string referenceNumber);

        Task<bool> PerformHealthCheckLogic();

        Application? GetMostRecentApplication(Guid petId);

        Task<IEnumerable<Application>> GetApplicationsByPetIdAsync(Guid petId);
    }
}
