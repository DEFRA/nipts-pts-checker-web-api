using Defra.PTS.Checker.Entities;

namespace Defra.PTS.Checker.Services.Interface
{
    public interface ITravelDocumentService
    {
        Task<TravelDocument> GetTravelDocumentByApplicationId(Guid applicationId);

        Task<TravelDocument> GetTravelDocumentByPTDNumber(string ptdNumber);
    }
}
