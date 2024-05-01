using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Models
{
    [ExcludeFromCodeCoverage]
    public class Address
    {
        public Guid Id { get; set; }
        public string? AddressLineOne { get; set; }
        public string? AddressLineTwo { get; set; }
        public string? TownOrCity { get; set; }
        public string? County { get; set; }
        public string? PostCode { get; set; }
        public string? CountryName { get; set; }
        public string? AddressType { get; set; }
        public bool? IsActive { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
