﻿using Defra.PTS.Checker.Models.Constants;
using Defra.PTS.Checker.Models.SchemaFilters;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Models.Search;

[ExcludeFromCodeCoverage]
[SwaggerSchemaFilter(typeof(SearchByPtdNumberSchemaFilter))]
public class SearchByPtdNumberRequest : IValidatableObject
{
    [SwaggerSchema("The pet travel document number")]
    [Required(ErrorMessage = "PTD number is required")]
    [StringLength(20, ErrorMessage = "PTD number must be 20 characters or less")]
    public string PTDNumber {  get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(PTDNumber) && !PTDNumber.StartsWith(ApiConstants.PTDNumberPrefix, StringComparison.OrdinalIgnoreCase))
        {
            yield return new ValidationResult($"PTD number must start with {ApiConstants.PTDNumberPrefix}", new[] { nameof(PTDNumber) });
        }
    }
}
