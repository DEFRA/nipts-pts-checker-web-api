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
        private readonly ILogger<TravelDocumentService> _log;
        private readonly ITravelDocumentRepository _travelDocumentRepository;

        public TravelDocumentService(ILogger<TravelDocumentService> log, ITravelDocumentRepository travelDocumentRepository)
        {
            _log = log;
            _travelDocumentRepository = travelDocumentRepository;
        }

        public Task<TravelDocument> GetTravelDocumentByReferenceNumber(string referenceNumber)
        {
            _log.LogInformation("Running inside method {0}", "GetTravelDocumentByReferenceNumber");
            var travelDocument = _travelDocumentRepository.GetTravelDocumentByReferenceNumber(referenceNumber);
            return travelDocument;
        }

        public Task<TravelDocument> GetTravelDocumentByPTDNumber(string ptdNumber)
        {
            return _travelDocumentRepository.GetTravelDocumentByPTDNumber(ptdNumber);
        }

    }
}
