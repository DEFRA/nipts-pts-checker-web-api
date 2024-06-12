using Defra.PTS.Checker.Models.Enums;
using Defra.PTS.Checker.Models.SchemaFilters;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Models.Search;

[ExcludeFromCodeCoverage]
[SwaggerSchemaFilter(typeof(SearchByPtdNumberResponseSchemaFilter))]
public class SearchByPtdNumberResponse
{
    /// <summary>
    /// Document Reference Number
    /// </summary>
    public string DocumentReferenceNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Document Issue Date
    /// </summary>
    public DateTime? DateOfIssue { get; set; }

    /// <summary>
    /// Document Status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Microchip Number
    /// </summary>
    public string MicrochipNumber { get; set; } = string.Empty;

    /// <summary>
    /// Microchipped Date
    /// </summary>
    public DateTime? MicrochippedDate { get; set; }

    /// <summary>
    /// Pet Name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Pet Colour
    /// </summary>
    public string Colour { get; set; } = string.Empty;

    /// <summary>
    /// Pet Species ID
    /// </summary>
    [EnumDataType(typeof(PetSpeciesType))]
    public PetSpeciesType SpeciesId { get; set; }

    /// <summary>
    /// Pet Breed Name
    /// </summary>
    public string Breed { get; set; } = string.Empty;

    /// <summary>
    /// Pet Gender
    /// </summary>
    [EnumDataType(typeof(PetGenderType))]
    public PetGenderType Sex { get; set; } 

    /// <summary>
    /// Pet Date of Birth
    /// </summary>
    public DateTime? DOB { get; set; }

    /// <summary>
    /// Pet Unique Feature
    /// </summary>
    public string UniqueFeatureDescription { get; set; } = string.Empty;
}

