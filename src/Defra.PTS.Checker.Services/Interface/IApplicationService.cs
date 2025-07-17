using Defra.PTS.Checker.Entities;

namespace Defra.PTS.Checker.Services.Interface
{
    public interface IApplicationService
    {
        Task<Application> GetApplicationById(Guid id);
        Task<object?> GetApplicationByReferenceNumber(string referenceNumber);
        Task<object?> GetApplicationByPTDNumber(string ptdNumber);
        Task<bool> GetIsUserSuspendedByEmail(string email);
    }
}
