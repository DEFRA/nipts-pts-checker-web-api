using Defra.PTS.Checker.Models.SchemaFilters;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.PTS.Checker.Models
{
    [ExcludeFromCodeCoverage]
    [SwaggerSchemaFilter(typeof(GbCheckReportResponseSchemaFilter))]
    public class GbCheckReportResponseModel
    {
        public Guid GbCheckSummaryId { get; set; }

        public CheckDetails? CheckDetails { get; set; }

        public CheckOutcome? CheckOutcome { get; set; }
    }
}
