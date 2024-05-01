
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Entities
{
    [ExcludeFromCodeCoverageAttribute]
    public class UserDetail
    {
        [Key]
        public Guid? AddressId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }       

        public string? Telephone { get; set; }
        public string? AddressLineOne { get; set; }        
        public string? AddressLineTwo { get; set; }
        public string? TownOrCity { get; set; }
        public string? County { get; set; }
        public string? PostCode { get; set; }
    }
}
