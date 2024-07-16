using Defra.PTS.Checker.Models.SchemaFilters;
using Swashbuckle.AspNetCore.Annotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Models;

[ExcludeFromCodeCoverage]
[SwaggerSchemaFilter(typeof(CheckOutcomeResponseSchemaFilter))]
public class CheckOutcomeResponseModel
{
    [SwaggerSchema("The check summary id")]
    public Guid CheckSummaryId { get; set; }
}
