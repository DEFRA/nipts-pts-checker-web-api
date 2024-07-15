using Defra.PTS.Checker.Models.Constants;
using Defra.PTS.Checker.Models.SchemaFilters;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Models;

[ExcludeFromCodeCoverage]
[SwaggerSchemaFilter(typeof(CheckOutcomeSchemaFilter))]
public class CheckOutcomeModel : IValidatableObject
{
    [SwaggerSchema("The pet travel document number")]
    [Required(ErrorMessage = "PTD number is required")]
    [StringLength(20, ErrorMessage = "PTD number must be 20 characters or less")]
    public string PTDNumber {  get; set; } = string.Empty;

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
        if (!string.IsNullOrWhiteSpace(PTDNumber) && !PTDNumber.ToLower().StartsWith(ApiConstants.PTDNumberPrefix.ToLower()))
        {
            yield return new ValidationResult($"PTD number must start with {ApiConstants.PTDNumberPrefix}", new[] { nameof(PTDNumber) });
        }

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
