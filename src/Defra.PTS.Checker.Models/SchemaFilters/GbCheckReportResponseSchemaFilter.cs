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
    public class GbCheckReportResponseSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {           
            if (context.Type == typeof(GbCheckReportResponseModel))
            {
                // Adding a top-level example
                schema.Example = new OpenApiObject
                {
                    ["gbCheckSummaryId"] = new OpenApiString("example-summary-id"),
                    ["checkDetails"] = new OpenApiObject
                    {
                        ["checkersName"] = new OpenApiString("John Doe"),
                        ["dateAndTimeChecked"] = new OpenApiString("2024-11-19T14:00:00Z"),
                        ["routeId"] = new OpenApiInteger(2),
                        ["departureDate"] = new OpenApiString(DateTime.UtcNow.ToString("dd/MM/yyyy")),
                        ["departureTime"] = new OpenApiString("14:00")
                    },
                    ["CheckOutcome"] = new OpenApiObject
                    {
                        ["gbRefersToDAERAOrSPS"] = new OpenApiBoolean(true),
                        ["gbAdviseNoTravel"] = new OpenApiBoolean(true),
                        ["gbPassengerSaysNoTravel"] = new OpenApiBoolean(true),
                        ["mcNotMatch"] = new OpenApiBoolean(true),
                        ["mcNotMatchActual"] = new OpenApiString("123456789012345"),
                        ["mcNotFound"] = new OpenApiBoolean(true),
                        ["vcNotMatchPTD"] = new OpenApiBoolean(true),
                        ["oiFailPotentialCommercial"] = new OpenApiBoolean(true),
                        ["oiFailAuthTravellerNoConfirmation"] = new OpenApiBoolean(true),
                        ["oiRefusedToSignDeclaration"] = new OpenApiBoolean(true),
                        ["oiFailOther"] = new OpenApiBoolean(true),
                        ["relevantComments"] = new OpenApiString("John Doe"),
                    }
                };
            }
        }
    }
}
