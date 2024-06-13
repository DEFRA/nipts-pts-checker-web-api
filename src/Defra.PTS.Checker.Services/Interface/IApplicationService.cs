using Defra.PTS.Checker.Entities;

namespace Defra.PTS.Checker.Services.Interface
{
    public interface IApplicationService
    {
        Task<Application> GetApplicationById(Guid id);
        Task<Application> GetApplicationByReferenceNumber(string referenceNumber);
    }
}
