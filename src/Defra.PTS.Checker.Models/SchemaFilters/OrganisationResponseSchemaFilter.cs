using Defra.PTS.Checker.Models.Search;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.PTS.Checker.Models.SchemaFilters
{
    [ExcludeFromCodeCoverage]
    public class OrganisationResponseSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            schema.Example = new OpenApiObject
            {
                ["id"] = new OpenApiString(Guid.NewGuid().ToString()),
                ["name"] = new OpenApiString("GBCheck"),
                ["location"] = new OpenApiString("GB"),
                ["externalId"] = new OpenApiString(Guid.NewGuid().ToString()),
                ["activeFrom"] = new OpenApiString("2024-10-14T17:17:00Z"),
                ["activeTo"] = new OpenApiString("2024-10-14T17:17:00Z"),
                ["isActive"] = new OpenApiBoolean(true),
            };
        }
    }
}
