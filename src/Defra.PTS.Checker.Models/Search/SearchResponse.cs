using Defra.PTS.Checker.Models.SchemaFilters;
using Swashbuckle.AspNetCore.Annotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Models.Search;

[ExcludeFromCodeCoverage]
[SwaggerSchemaFilter(typeof(SearchByPtdNumberResponseSchemaFilter))]
public class SearchResponse
{
    public Pet Pet { get; set; }
    public Application Application { get; set; }
    public TravelDocument TravelDocument { get; set; }
}

