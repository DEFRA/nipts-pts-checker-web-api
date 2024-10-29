using Defra.PTS.Checker.Models.SchemaFilters;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using sailOptions = Defra.PTS.Checker.Models.Enums.SailingOption;

namespace Defra.PTS.Checker.Models
{
    [ExcludeFromCodeCoverage]
    [SwaggerSchemaFilter(typeof(NonComplianceSchemaFilter))]
    public class NonComplianceModel : CheckOutcomeModel, IValidatableObject
    {
        [SwaggerSchema("Microchip number does not match the PTD")]
        public bool? MCNotMatch { get; set; }

        [SwaggerSchema("Microchip number found in scan")]
        [StringLength(300, ErrorMessage = "Microchip number cannot exceed 15 characters.")]
        public string? MCNotMatchActual { get; set; }

        [SwaggerSchema("Cannot find microchip")]
        public bool? MCNotFound { get; set; }

        [SwaggerSchema("Visual Check Pet does not match the PTD")]
        public bool? VCNotMatchPTD { get; set; }

        [SwaggerSchema("Other Issues Potential commercial movement")]
        public bool? OIFailPotentialCommercial { get; set; }

        [SwaggerSchema("Other Issues Authorised traveller but no confirmation")]
        public bool? OIFailAuthTravellerNoConfirmation { get; set; }

        [SwaggerSchema("Other Issues Other reason")]
        public bool? OIFailOther { get; set; }

        [SwaggerSchema("Type of passenger Foot passenger or Vehicle")]
        public int? PassengerTypeId { get; set; }

        [SwaggerSchema("Relevant Comments")]
        [StringLength(500, ErrorMessage = "Relevant Comments cannot exceed 500 characters.")]
        public string? RelevantComments { get; set; }

        [SwaggerSchema("GB Outcome Passenger referred to DAERA/SPS at NI port")]
        public bool? GBRefersToDAERAOrSPS { get; set; }

        [SwaggerSchema("GB Outcome Passenger advised not to travel")]
        public bool? GBAdviseNoTravel { get; set; }

        [SwaggerSchema("GB Outcome Passenger says they will not travel")]
        public bool? GBPassengerSaysNoTravel { get; set; }

        [SwaggerSchema("SPS Outcome Allowed or Not Allowed to travel under Windsor Framework")]
        public bool? SPSOutcome { get; set; }

        [SwaggerSchema("SPS Outcome Details")]
        [StringLength(500, ErrorMessage = "SPS Outcome Details cannot exceed 500 characters.")]
        public string? SPSOutcomeDetails { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validSailingOption = new List<int> { (int)sailOptions.Ferry, (int)sailOptions.Flight };
            if ((SailingOption == null) || (SailingOption != null && !validSailingOption.Contains(SailingOption.GetValueOrDefault())))
            {
                yield return new ValidationResult($"Valid RouteOption is required", new[] { nameof(SailingOption) });
            }

            var validOutcomes = new List<string> { "Fail" };
            if (!validOutcomes.Contains(CheckOutcome))
            {
                yield return new ValidationResult($"Outcome must be 'Fail'", new[] { nameof(CheckOutcome) });
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
}
