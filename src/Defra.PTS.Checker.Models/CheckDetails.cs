using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.PTS.Checker.Models
{
    [ExcludeFromCodeCoverage]
    public class CheckDetails
    {
        public string? CheckersName { get; set; } = string.Empty;
        public DateTime? DateAndTimeChecked { get; set; }
        public int? RouteId { get; set; }
        public string? DepartureDate { get; set; } = string.Empty;
        public string? DepartureTime { get; set; } = string.Empty;
    }
}
