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
            var descriptions = new Dictionary<string, string>
            {
                { "TravelDocument", "The travel document details." },
                { "Pet", "The pet details." },
                { "Application", "The application details." }
            };

            foreach (var key in descriptions.Keys)
            {
                if (schema.Properties.TryGetValue(key, out var property))
                {
                    property.Description = descriptions[key];
                }
            }
        }
    }
}
