﻿using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Entities
{
    [ExcludeFromCodeCoverage]
    public class Colour
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SpeciesId { get; set; }
    }
}
