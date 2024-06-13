using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Repositories.Implementation;
using Defra.PTS.Checker.Repositories.Interface;
using Defra.PTS.Checker.Services.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.PTS.Checker.Services.Implementation
{
    public class TravelDocumentService : ITravelDocumentService
    {
        private readonly ITravelDocumentRepository _travelDocumentRepository;

        public TravelDocumentService(ITravelDocumentRepository travelDocumentRepository)
        {
            _travelDocumentRepository = travelDocumentRepository;
        }

        public Task<TravelDocument> GetTravelDocumentByPTDNumber(string ptdNumber)
        {
            return _travelDocumentRepository.GetTravelDocumentByPTDNumber(ptdNumber);
        }

        public async Task<TravelDocument> GetTravelDocumentByApplicationId(Guid applicationId)
        {
            var travelDocument = await _travelDocumentRepository.GetTravelDocumentByApplicationIdAsync(applicationId);

            return travelDocument!;
        }

    }
}
