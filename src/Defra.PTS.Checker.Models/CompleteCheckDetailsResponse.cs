using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Models
{

    [ExcludeFromCodeCoverage]
    public class CompleteCheckDetailsResponse
    {
        public string? CheckOutcome { get; set; } = "Passenger referred to DAERA/SPS at NI Port";
        public string? PTDNumber { get; set; }
        public string? ApplicationReference { get; set; }
        public string? Status { get; set; }
        public DateTime? DateAuthorised { get; set; }
        public string? MicrochipNumber { get; set; }
        public string? PetName { get; set; }
        public string? Species { get; set; }
        public string? BreedName { get; set; }
        public DateTime? DateOfIssue { get; set; }
        public string? CheckerName { get; set; }
        public string? DateTimeChecked { get; set; }
        public string? Route { get; set; }
        public string? ScheduledDepartureDate { get; set; }
        public string? ScheduledDepartureTime { get; set; }
        public string? RelevantComments { get; set; }
        public bool HasMultipleRecords { get; set; }
    }


}
