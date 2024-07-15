using Defra.PTS.Checker.Models.SchemaFilters;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Models;

[ExcludeFromCodeCoverage]
[SwaggerSchemaFilter(typeof(CheckOutcomeSchemaFilter))]
public class CheckOutcomeModel : IValidatableObject
{
    [SwaggerSchema("The application id")]
    [Required(ErrorMessage = "Application id is required")]
    public Guid ApplicationId { get; set; }

    [SwaggerSchema("The check outcome: Pass or Fail")]
    [Required(ErrorMessage = "Check outcome is required")]
    public string CheckOutcome { get; set; } = string.Empty;

    [SwaggerSchema("The checker id")]
    public Guid? CheckerId { get; set; }

    [SwaggerSchema("The route id")]
    [Required(ErrorMessage = "Route id is required")]
    public int? RouteId { get; set; }

    [SwaggerSchema("The sailing time")]
    [Required(ErrorMessage = "Sailing time is required")]
    public DateTime? SailingTime { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var validOutcomes = new List<string> { "Pass", "Fail" };
        if (!validOutcomes.Contains(CheckOutcome))
        {
            yield return new ValidationResult($"Outcome must be 'Pass' or 'Fail'", new[] { nameof(CheckOutcome) });
        }

        if (RouteId.GetValueOrDefault() == 0)
        {
            yield return new ValidationResult($"RouteId is required", new[] { nameof(RouteId) });
        }

        if (!SailingTime.HasValue || SailingTime == DateTime.MinValue)
        {
            yield return new ValidationResult($"SailingTime is not valid", new[] { nameof(SailingTime) });
        }
    }
}
