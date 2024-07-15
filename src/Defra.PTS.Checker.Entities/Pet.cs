using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Entities
{
    [ExcludeFromCodeCoverage]
    public class Pet
    {
        public Guid Id { get; set; }
        public int IdentificationType { get; set; }
        public string? MicrochipNumber { get; set; }
        public DateTime? MicrochippedDate { get; set; }
        public int SpeciesId { get; set; }
        public int? BreedId { get; set; }
        public int? BreedTypeId { get; set; }
        public string? AdditionalInfoMixedBreedOrUnknown { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SexId { get; set; }
        public int IsDateOfBirthKnown { get; set; }
        public DateTime? DOB { get; set; }
        public int? ApproximateAge { get; set; }
        public int ColourId { get; set; }
        public string? OtherColour { get; set; }
        public int HasUniqueFeature { get; set; }
        public string? UniqueFeatureDescription { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        // Navigation properties
        [ForeignKey("BreedId")]
        public virtual Breed? Breed { get; set; }
        // Navigation properties
        [ForeignKey("ColourId")]
        public virtual Colour? Colour { get; set; }
    }
}
