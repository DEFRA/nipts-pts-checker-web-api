using Defra.PTS.Checker.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.PTS.Checker.Services.Interface
{
    public interface ITravelDocumentService
    {
        Task<TravelDocument> GetTravelDocumentByReferenceNumber(string referenceNumber);
        Task<TravelDocument> GetTravelDocumentByPTDNumber(string ptdNumber);
    }
}
