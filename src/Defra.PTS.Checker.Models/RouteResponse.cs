using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Models
{
    [ExcludeFromCodeCoverage]
    public class RouteResponse
    {
        public int Id { get; set; }
        public string RouteName { get; set; }
    }
}
