using Defra.PTS.Checker.Models.Constants;
using Defra.PTS.Checker.Models.SchemaFilters;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using sailOptions = Defra.PTS.Checker.Models.Enums.SailingOption;

namespace Defra.PTS.Checker.Models;

[ExcludeFromCodeCoverage]
[SwaggerSchemaFilter(typeof(CheckOutcomeSchemaFilter))]
public class CheckOutcomeModel : IValidatableObject
{
    [SwaggerSchema("The application id")]
    [Required(ErrorMessage = "Application id is required")]
    public Guid ApplicationId {  get; set; }

    [SwaggerSchema("The check outcome: Pass or Fail")]
    [Required(ErrorMessage = "Check outcome is required")]
    public string CheckOutcome { get; set; } = string.Empty;

    [SwaggerSchema("The checker id")]
    public Guid? CheckerId { get; set; }

    [SwaggerSchema("The route id")]    
    public int? RouteId { get; set; }

    [SwaggerSchema("The sailing time")]
    public DateTime? SailingTime { get; set; }

    [SwaggerSchema("The sailing option")]
    [Required(ErrorMessage = "Sailing Option is required")]
    public int? SailingOption { get; set; }

    [SwaggerSchema("The Flight Number")]
    public string? FlightNumber { get; set; }

    [SwaggerSchema("The Checker Type")]
    [Required(ErrorMessage = "Checker Type is required")]
    public bool IsGBCheck { get; set; }

    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var validSailingOption = new List<int> { (int)sailOptions.Ferry, (int)sailOptions.Flight };
        if ((SailingOption == null) || (SailingOption != null && !validSailingOption.Contains(SailingOption.GetValueOrDefault())))
        {
            yield return new ValidationResult($"Valid RouteOption is required", new[] { nameof(SailingOption) });
        }

        var validOutcomes = new List<string> { "Pass" };
        if (!validOutcomes.Contains(CheckOutcome))
        {
            yield return new ValidationResult($"Outcome must be 'Pass'", new[] { nameof(CheckOutcome) });
        }

        if (!SailingTime.HasValue)
        {
            yield return new ValidationResult($"Sailing time is required", new[] { nameof(SailingTime) });
        }

        if (SailingTime == DateTime.MinValue)
        {
            yield return new ValidationResult($"Sailing time is not valid", new[] { nameof(SailingTime) });
        }

        if (SailingOption.GetValueOrDefault() == (int)sailOptions.Ferry)
        {
            if (RouteId != null && RouteId.GetValueOrDefault() == 0)
            {
                yield return new ValidationResult($"RouteId is required", new[] { nameof(RouteId) });
            }            
        }

        if (SailingOption.GetValueOrDefault() == (int)sailOptions.Flight)
        {
            if (string.IsNullOrEmpty(FlightNumber) || string.IsNullOrWhiteSpace(FlightNumber))
            {
                yield return new ValidationResult($"Flight Number is required", new[] { nameof(FlightNumber) });
            }
        }
    }
}
