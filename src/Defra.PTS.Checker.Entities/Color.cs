using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Entities
{
    [ExcludeFromCodeCoverageAttribute]
    public class Color
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SpeciesId { get; set; }
    }
}
