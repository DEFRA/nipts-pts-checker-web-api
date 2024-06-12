using Defra.PTS.Checker.Entities;

namespace Defra.PTS.Checker.Repositories.Interface
{
    public interface ITravelDocumentRepository :  IRepository<TravelDocument>
    {
        Task<TravelDocument> GetTravelDocument(Guid? applicationId, Guid? ownerId, Guid? petId);
        Task<TravelDocument> GetTravelDocumentByReferenceNumber(string referenceNumber);   
        Task<IEnumerable<TravelDocument>> GetByPetIdAsync(Guid petId);
        Task<TravelDocument?> GetTravelDocumentByApplicationIdAsync(Guid applicationId);

        Task<TravelDocument> GetTravelDocumentByPTDNumber(string ptdNumber);
    }
}
