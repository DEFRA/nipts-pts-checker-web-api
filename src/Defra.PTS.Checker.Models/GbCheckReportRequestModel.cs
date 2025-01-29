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
    [SwaggerSchemaFilter(typeof(GbCheckReportRequestSchemaFilter))]
    public class GbCheckReportRequestModel
    {
        [SwaggerSchema("The GbCheckSummaryId")]
        [Required(ErrorMessage = "GbCheckSummaryId is required")]
        public string GbCheckSummaryId { get; set; } = string.Empty;
    }
}
