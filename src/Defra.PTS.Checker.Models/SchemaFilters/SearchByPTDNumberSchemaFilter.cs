using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Models.SchemaFilters;

[ExcludeFromCodeCoverage]
public class SearchByPTDNumberSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        schema.Example = new OpenApiObject
        {
            ["ptdNumber"] = new OpenApiString("GB826CD186E")
        };
    }
}
