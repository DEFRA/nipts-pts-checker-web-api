using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Entities
{
    [ExcludeFromCodeCoverage]
    [Table("Application")]
    public class Application
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PetId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid OwnerId { get; set; }

        [MaxLength(300)]
        public string? OwnerNewName { get; set; }

        [MaxLength(50)]
        public string? OwnerNewTelephone { get; set; }

        public Guid? OwnerAddressId { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Status { get; set; }

        [Required]
        [MaxLength(20)]
        public string? ReferenceNumber { get; set; }

        [Required]
        public bool IsDeclarationSigned { get; set; }

        [Required]
        public bool IsConsentAgreed { get; set; }

        [Required]
        public bool IsPrivacyPolicyAgreed { get; set; }

        [Required]
        public DateTime DateOfApplication { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; } = DateTime.UtcNow;

        public Guid? DynamicId { get; set; }

        public DateTime? DateAuthorised { get; set; }

        public DateTime? DateRejected { get; set; }

        public DateTime? DateRevoked { get; set; }

        // Navigation properties
        [ForeignKey("PetId")]
        public virtual Pet? Pet { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("OwnerId")]
        public virtual Owner? Owner { get; set; }

        [ForeignKey("OwnerAddressId")]
        public virtual Address? OwnerAddress { get; set; }
    }
}
