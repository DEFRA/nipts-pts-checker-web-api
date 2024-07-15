using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Repositories.Interface;
using Defra.PTS.Checker.Services.Interface;

namespace Defra.PTS.Checker.Services.Implementation
{
    public class TravelDocumentService : ITravelDocumentService
    {
        private readonly ITravelDocumentRepository _travelDocumentRepository;

        public TravelDocumentService(ITravelDocumentRepository travelDocumentRepository)
        {
            _travelDocumentRepository = travelDocumentRepository;
        }

        public async Task<TravelDocument> GetTravelDocumentByPTDNumber(string ptdNumber)
        {
            var travelDocument = await _travelDocumentRepository.GetTravelDocumentByPTDNumber(ptdNumber);

            return travelDocument!;
        }

        public async Task<TravelDocument> GetTravelDocumentByApplicationId(Guid applicationId)
        {
            var travelDocument = await _travelDocumentRepository.GetTravelDocumentByApplicationIdAsync(applicationId);

            return travelDocument!;
        }

    }
}
