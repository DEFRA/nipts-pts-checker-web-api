using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Entities
{
    [ExcludeFromCodeCoverageAttribute]
    public class VwApplication
    {
        [Key]
        public Guid ApplicationId { get; set; }
        public string? Status { get; set; }
        public string? ReferenceNumber { get; set; }
        public DateTime DateOfApplication { get; set; }

        public Guid UserId { get; set; }
        public string? UserFullName { get; set; }
        public string? UserEmail { get; set; }

        public Guid OwnerId { get; set; }
        public string? OwnerName { get; set; }
        public string? OwnerEmail { get; set; }
        public string? OwnerPhoneNumber { get; set; }
        public Guid? OwnerAddressId { get; set; }
        public string? OwnerAddressLineOne { get; set; }
        public string? OwnerAddressLineTwo { get; set; }
        public string? OwnerTownOrCity { get; set; }
        public string? OwnerCounty { get; set; }
        public string? OwnerPostcode { get; set; }

        public Guid PetId { get; set; }
        public string? PetName { get; set; }
        public int? PetSpeciesId { get; set; }
        public int? PetBreedId { get; set; }
        public string? PetBreedName { get; set; }
        public string? PetBreedOther { get; set; }
        public int? PetGenderId { get; set; }
        public DateTime? PetBirthDate { get; set; }
        public int? PetColourId { get; set; }
        public string? PetColourName { get; set; }
        public string? PetColourOther { get; set; }
        public int? PetHasUniqueFeature { get; set; }
        public string? PetUniqueFeature { get; set; }

        public string? MicrochipNumber { get; set; }
        public DateTime? MicrochippedDate { get; set; }

        public string? DocumentReferenceNumber { get; set; }
        public DateTime? DocumentIssueDate { get; set; }
        public string? DocumentSignedBy { get; set; }
    }
}