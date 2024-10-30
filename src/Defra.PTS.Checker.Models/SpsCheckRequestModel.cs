using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Models
{
    [ExcludeFromCodeCoverage]
    public class SpsCheckRequestModel
    {
        public required string Route { get; set; }
        public DateTime SailingDate { get; set; }
        public int TimeWindowInHours { get; set; } = 48; 
    }
}
