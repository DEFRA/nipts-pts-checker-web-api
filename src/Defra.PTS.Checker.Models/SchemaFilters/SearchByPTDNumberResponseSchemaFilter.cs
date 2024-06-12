using Defra.PTS.Checker.Models.Enums;
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
        schema.Example = new OpenApiObject
        {
            ["documentReferenceNumber"] = new OpenApiString("GB826CD186E"),
            ["dateOfIssue"] = new OpenApiDate(DateTime.Now.Date),
            ["status"] = new OpenApiString("Approved"),
            ["microchipNumber"] = new OpenApiString("123456789012345"),
            ["microchippedDate"] = new OpenApiDate(DateTime.Now.AddDays(-60).Date),
            ["name"] = new OpenApiString("Toto"),
            ["breed"] = new OpenApiString("Afghan Hound"),
            ["colour"] = new OpenApiString("Black"),
            ["SpeciesId"] = new OpenApiString(PetSpeciesType.Dog.ToString()),
            ["sex"] = new OpenApiString(PetGenderType.Male.ToString()),
            ["dob"] = new OpenApiDate(DateTime.Now.AddDays(-90).Date),
            ["uniqueFeatureDescription"] = new OpenApiString("White star on his chest")
        };
    }
}
