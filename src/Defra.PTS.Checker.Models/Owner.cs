using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Models
{
    [ExcludeFromCodeCoverage]
    public class Owner
    {
        public string? Name { get; set; }
        public string? Telephone { get; set; }
        public string? Email { get; set; }
        public OwnerAddress? Address { get; set; }
    }
}
