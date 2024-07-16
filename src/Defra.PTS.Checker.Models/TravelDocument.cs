using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.PTS.Checker.Models
{
    public class TravelDocument
    {
        public Guid TravelDocumentId { get; set; }
        public string? TravelDocumentReferenceNumber { get; set; }
        public DateTime? TravelDocumentDateOfIssue { get; set; }
        public DateTime? TravelDocumentValidityStartDate { get; set; }
        public DateTime? TravelDocumentValidityEndDate { get; set; }
        public int? TravelDocumentStatusId { get; set; }
    }
}
