using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Models
{

    [ExcludeFromCodeCoverage]
    public class CompleteCheckDetailsResponse
    {
        public List<string>? CheckOutcome { get; set; } // Updated to handle multiple outcomes
        public List<string>? ReasonForReferral { get; set; } // Updated to handle multiple referral reasons
        public string? MicrochipNumber { get; set; } // Represents a single microchip number
        public List<string>? AdditionalComments { get; set; } // Updated to handle multiple comments
        public string? GBCheckerName { get; set; } // Represents the GB checker name
        public string? DateAndTimeChecked { get; set; } // Represents the date and time checked
        public string? Route { get; set; } // Represents the route information
        public string? ScheduledDepartureDate { get; set; } // Represents the departure date
        public string? ScheduledDepartureTime { get; set; } // Represents the departure time
    }

}
