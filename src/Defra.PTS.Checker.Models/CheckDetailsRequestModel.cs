using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Models
{
    [ExcludeFromCodeCoverage]
    public class CheckDetailsRequestModel
    {
        [Required]
        [MaxLength(20)]
        public string? Identifier { get; set; }

        [MaxLength(300)]
        public string? RouteName { get; set; }

        public DateTime? Date { get; set; }

        public TimeSpan? ScheduledTime { get; set; }
    }
}
