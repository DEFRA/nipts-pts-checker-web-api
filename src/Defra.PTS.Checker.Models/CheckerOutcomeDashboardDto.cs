using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Models;

[ExcludeFromCodeCoverage]
public class CheckerOutcomeDashboardDto : IValidatableObject
{
    public string? StartHour { get; set; }
    public string? EndHour { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var validationResults = new List<ValidationResult>();

        // Validate StartHour
        if (string.IsNullOrWhiteSpace(StartHour))
        {
            validationResults.Add(new ValidationResult("Start Hour is required", new[] { nameof(StartHour) }));
        }
        else if (!int.TryParse(StartHour, out _))
        {
            validationResults.Add(new ValidationResult("Start Hour must be a valid integer", new[] { nameof(StartHour) }));
        }

        // Validate EndHour
        if (string.IsNullOrWhiteSpace(EndHour))
        {
            validationResults.Add(new ValidationResult("End Hour is required", new[] { nameof(EndHour) }));
        }
        else if (!int.TryParse(EndHour, out _))
        {
            validationResults.Add(new ValidationResult("End Hour must be a valid integer", new[] { nameof(EndHour) }));
        }

        // Additional validation (if both StartHour and EndHour are valid)
        if (int.TryParse(StartHour, out var startHourInt) && int.TryParse(EndHour, out var endHourInt) && startHourInt > endHourInt)
        {
            validationResults.Add(new ValidationResult("Start Hour cannot be greater than End Hour", new[] { nameof(StartHour), nameof(EndHour) }));
        }

        return validationResults;
    }
}
