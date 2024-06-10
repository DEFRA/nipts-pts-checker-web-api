using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Entities
{
    [ExcludeFromCodeCoverage]
    [Table("TravelDocument")]
    public class TravelDocument
    {
        [Key]
        public Guid Id { get; set; }

        public int? IssuingAuthorityId { get; set; }

        [Required]
        public Guid PetId { get; set; }

        [Required]
        public Guid OwnerId { get; set; }

        [Required]
        public Guid ApplicationId { get; set; }

        public byte[]? QrCode { get; set; }

        [Required]
        [MaxLength(20)]
        public string? DocumentReferenceNumber { get; set; }

        public bool? IsLifeTime { get; set; }

        public DateTime? ValidityStartDate { get; set; }

        public DateTime? ValidityEndDate { get; set; }

        public int? StatusId { get; set; }

        public DateTime? DateOfIssue { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; } = DateTime.UtcNow;

        [MaxLength(300)]
        public string? DocumentSignedBy { get; set; }

        // Navigation properties
        [ForeignKey("ApplicationId")]
        public virtual Application? Application { get; set; }

        [ForeignKey("PetId")]
        public virtual Pet? Pet { get; set; }

        [ForeignKey("OwnerId")]
        public virtual Owner? Owner { get; set; }

    }
}
