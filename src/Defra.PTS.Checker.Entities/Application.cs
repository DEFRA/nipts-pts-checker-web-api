using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Entities
{
    [ExcludeFromCodeCoverageAttribute]
    public class Application
    {
        public Guid Id { get; set; }
        public Guid PetId { get; set; }
        public Guid UserId { get; set; }
        public Guid OwnerId { get; set; }
        public string? OwnerNewName { get; set; }
        public string? OwnerNewTelephone { get; set; }
        public Guid? OwnerAddressId { get; set; }
        public string ReferenceNumber { get; set; }
        public bool IsDeclarationSigned { get; set; }
        public bool IsConsentAgreed { get; set; }
        public bool IsPrivacyPolicyAgreed { get; set; }
        public DateTime? DateOfApplication { get; set; }
        public string? Status { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public Guid? DynamicId { get; set; }
        public DateTime? DateAuthorised { get; set; }
        public DateTime? DateRejected { get; set; }
        public DateTime? DateRevoked { get; set; }
    }
}