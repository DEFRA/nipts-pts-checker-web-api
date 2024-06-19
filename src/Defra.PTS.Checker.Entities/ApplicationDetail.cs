using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Entities
{
    [ExcludeFromCodeCoverage]
    public class ApplicationDetail
    {
        public string? Status { get; set; }
        public string? DocumentReferenceNumber { get; set; }
        public DateTime? DateOfIssue { get; set; }
        public string? ReferenceNumber { get; set; }
        public DateTime? DateOfApplication { get; set; }
        public string? MicrochipNumber { get; set; }
        public DateTime? MicrochippedDate { get; set; }
        public string? PetName { get; set; }
        public string? SpeciesOfPet { get; set; }
        public string? BreedName { get; set; }
        public string? SexOfPet { get; set; }
        public DateTime? DateOfBirthOfPet { get; set; }
        public string? ColourOfPet { get; set; }
        public string? UniqueFeaturesOfPet { get; set; }

    }
}