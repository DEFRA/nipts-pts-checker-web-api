using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Models.SchemaFilters;

[ExcludeFromCodeCoverage]
public class NonComplianceSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        schema.Example = new OpenApiObject
        {
            ["applicationId"] = new OpenApiString(Guid.NewGuid().ToString()),
            ["checkOutcome"] = new OpenApiString("Fail"),
            ["checkerId"] = new OpenApiString(Guid.NewGuid().ToString()),
            ["routeId"] = new OpenApiInteger(1),
            ["sailingTime"] = new OpenApiString("2024-10-14T17:17:00Z"),
            ["sailingOption"] = new OpenApiInteger(1),
            ["flightNumber"] = new OpenApiString("AB3456"),
            ["isGBCheck"] = new OpenApiBoolean(true),

            // Adding Non Compliance Details
            ["mcNotMatch"] = new OpenApiBoolean(true),
            ["mcNotMatchActual"] = new OpenApiString("123456789123456"),
            ["mcNotFound"] = new OpenApiBoolean(true),
            ["vcNotMatchPTD"] = new OpenApiBoolean(true),
            ["oiFailPotentialCommercial"] = new OpenApiBoolean(true),
            ["oiFailAuthTravellerNoConfirmation"] = new OpenApiBoolean(true),
            ["oiFailOther"] = new OpenApiBoolean(true),
            ["passengerTypeId"] = new OpenApiInteger(1),
            ["relevantComments"] = new OpenApiString("Relevant Comments"),
            ["gbRefersToDAERAOrSPS"] = new OpenApiBoolean(true),
            ["gbAdviseNoTravel"] = new OpenApiBoolean(true),
            ["gbPassengerSaysNoTravel"] = new OpenApiBoolean(true),
            ["spsOutcome"] = new OpenApiBoolean(true),
            ["spsOutcomeDetails"] = new OpenApiString("SPS Outcome Details"),
            ["gBCheckId"] = new OpenApiString(Guid.NewGuid().ToString()),
        };
    }
}
