using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Models
{

    [ExcludeFromCodeCoverage]
    public class CompleteCheckDetailsResponse
    {
       
        public List<string> CheckOutcome { get; set; } = new(); 
        public List<string> ReasonForReferral { get; set; } = new();
        public string? MicrochipNumber { get; set; }
        public List<string> AdditionalComments { get; set; } = new();
        public List<string> DetailsComments { get; set; } = new();
        public string GBCheckerName { get; set; } = string.Empty;
        public string DateAndTimeChecked { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty; 
        public string ScheduledDepartureDate { get; set; } = string.Empty;
        public string ScheduledDepartureTime { get; set; } = string.Empty;
    }


}
