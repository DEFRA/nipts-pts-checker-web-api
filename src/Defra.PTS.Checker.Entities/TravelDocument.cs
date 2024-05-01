using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Entities
{
    [ExcludeFromCodeCoverageAttribute]
    public class TravelDocument
    {
        public Guid Id { get; set; }
        public int? IssuingAuthorityId { get; set; }
        public Guid PetId { get; set; }
        public Guid OwnerId { get; set; }
        public Guid ApplicationId { get; set; }
        public string? QRCode { get; set; }
        public string DocumentReferenceNumber { get; set; }
        public bool? IsLifeTIme { get; set; }
        public DateTime? ValidityStartDate { get; set; }
        public DateTime? ValidityEndDate { get; set; }
        public int? StatusId { get; set; }
        public DateTime? DateOfIssue { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
