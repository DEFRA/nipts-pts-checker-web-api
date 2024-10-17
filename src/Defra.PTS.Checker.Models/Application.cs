
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Models
{
    [ExcludeFromCodeCoverage]
    public class Application
    {
        public Guid ApplicationId { get; set; }
        public string? ReferenceNumber { get; set; }
        public DateTime DateOfApplication { get; set; }
        public string? Status { get; set; }
        public DateTime? DateAuthorised { get; set; }
        public DateTime? DateRejected { get; set; }
        public DateTime? DateRevoked { get; set; }
    }
}
