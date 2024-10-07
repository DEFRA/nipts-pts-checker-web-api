using Defra.PTS.Checker.Models.Enums;
using Defra.PTS.Checker.Models.SchemaFilters;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Models.Search;

[ExcludeFromCodeCoverage]
[SwaggerSchemaFilter(typeof(SearchByPtdNumberResponseSchemaFilter))]
public class SearchResponse
{
    public Pet? Pet { get; set; }    
    public Application? Application { get; set; }
    public TravelDocument? TravelDocument { get; set; }
    public Owner? PetOwner { get; set; }
}

