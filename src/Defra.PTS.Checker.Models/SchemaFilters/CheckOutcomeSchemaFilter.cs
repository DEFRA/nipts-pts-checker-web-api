using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Models.SchemaFilters;

[ExcludeFromCodeCoverage]
public class CheckOutcomeSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        schema.Example = new OpenApiObject
        {
            ["applicationId"] = new OpenApiString(Guid.NewGuid().ToString()),
            ["checkOutcome"] = new OpenApiString("Pass"),
            ["checkerId"] = new OpenApiString(Guid.NewGuid().ToString()),
            ["routeId"] = new OpenApiInteger(1),
            ["sailingTime"] = new OpenApiString("2024-10-14T17:17:00Z"),
            ["sailingOption"] = new OpenApiInteger(1),
            ["flightNumber"] = new OpenApiString("AB3456"),
            ["isGBCheck"] = new OpenApiBoolean(true),
        };
    }
}
