using entity = Defra.PTS.Checker.Entities;

namespace  Defra.PTS.Checker.Repositories.Interface
{
    public interface ITravelDocumentRepository :  IRepository<entity.TravelDocument>
    {
        Task<entity.TravelDocument> GetTravelDocument(Guid? applicationId, Guid? ownerId, Guid? petId);
        Task<entity.TravelDocument> GetTravelDocumentByReferenceNumber(string referenceNumber);
    }
}
