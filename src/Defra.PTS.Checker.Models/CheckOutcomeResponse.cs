using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Models;

[ExcludeFromCodeCoverage]
public class CheckOutcomeResponse
{
    public string? RouteName { get; set; }
    public string? OperatorName { get; set; }
    public string? DeparturePort { get; set; }
    public string? ArrivalPort { get; set; }
    public string? DepartureDate { get; set; }
    public string? DepartureTime { get; set; }
    public int PassCount { get; set; }
    public int FailCount { get; set; }
    public string? FailReason { get; set; }
    public string? ViewDetailsLink { get; set; }
    public DateTime CombinedDateTime { get; set; }
}

