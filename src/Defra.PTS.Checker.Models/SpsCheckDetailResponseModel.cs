using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Models
{
    [ExcludeFromCodeCoverage]
    public class SpsCheckDetailResponseModel
    {
        public required string PTDNumber { get; set; }
        public string? PetDescription { get; set; }
        public string? Microchip { get; set; }
        public string? TravelBy { get; set; }
        public string? SPSOutcome { get; set; }
        public Guid CheckSummaryId { get; set; }

    }
}
