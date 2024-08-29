using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.PTS.Checker.Models
{
    [ExcludeFromCodeCoverage]
    public class Pet
    {
        public Guid PetId { get; set; }
        public string? PetName { get; set; }
        public string? Species { get; set; }
        public string? BreedName { get; set; }
        public string? Sex { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ColourName { get; set; }
        public string? SignificantFeatures { get; set; }
        public string? MicrochipNumber { get; set; }
        public DateTime? MicrochippedDate { get; set; }
    }
}
