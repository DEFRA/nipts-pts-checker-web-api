using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Models
{
    [ExcludeFromCodeCoverage]
    public class UserRequest
    {
        public Guid? ContactId { get; set; }
        public Address? Address { get; set; }
    }
}