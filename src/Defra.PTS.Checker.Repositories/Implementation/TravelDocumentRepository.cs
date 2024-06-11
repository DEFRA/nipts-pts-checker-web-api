using entity = Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Repositories.Implementation
{
    [ExcludeFromCodeCoverage]
    public class TravelDocumentRepository : Repository<entity.TravelDocument>, ITravelDocumentRepository
    {
        private CommonDbContext travelDocumentContext
        {
            get
            {
                return _dbContext as CommonDbContext;
            }
        }

        public TravelDocumentRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public async Task<entity.TravelDocument> GetTravelDocument(Guid? applicationId, Guid? ownerId, Guid? petId)
        {
                return await travelDocumentContext.TravelDocument.
                FirstOrDefaultAsync(a => a.ApplicationId == applicationId && a.OwnerId == ownerId && a.PetId == petId) ?? null!;           
        }

        public async Task<entity.TravelDocument> GetTravelDocumentByReferenceNumber(string referenceNumber)
        {
            return await travelDocumentContext.TravelDocument
                .Include(t => t.Application)
                .Include(t => t.Owner)
                .Include(t => t.Pet)
                .Include(t => t.Pet!.Breed)
                .Include(t => t.Pet!.Colour)
                .FirstOrDefaultAsync(a => a.DocumentReferenceNumber == referenceNumber) ?? null!;
        }
    }
}
