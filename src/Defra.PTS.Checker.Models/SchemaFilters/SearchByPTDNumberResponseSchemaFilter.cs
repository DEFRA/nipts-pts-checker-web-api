using Defra.PTS.Checker.Models.Enums;
using Defra.PTS.Checker.Models.Search;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Models.SchemaFilters;

[ExcludeFromCodeCoverage]
public class SearchByPtdNumberResponseSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(SearchResponse))
        {
            // Customization logic goes here
            schema.Description = "Description for SearchByPtdNumberResponse";

            // For example, add custom properties or modify existing ones
            if (schema.Properties.ContainsKey("TravelDocument"))
            {
                schema.Properties["TravelDocument"].Description = "The travel document details.";
            }
            if (schema.Properties.ContainsKey("Pet"))
            {
                schema.Properties["Pet"].Description = "The pet details.";
            }
            if (schema.Properties.ContainsKey("Application"))
            {
                schema.Properties["Application"].Description = "The application details.";
            }
        }
    }
}
