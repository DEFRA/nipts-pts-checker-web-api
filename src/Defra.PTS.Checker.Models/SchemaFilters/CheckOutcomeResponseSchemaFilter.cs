using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Models.SchemaFilters;

[ExcludeFromCodeCoverage]
public class CheckOutcomeResponseSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(CheckOutcomeResponseModel))
        {
            // Customization logic goes here
            schema.Description = "Description for CheckOutcomeResponse";

            // For example, add custom properties or modify existing ones
            if (schema.Properties.TryGetValue("CheckSummaryId", out var property))
            {
                property.Description = "The check summary id.";
            }
        }
    }
}
