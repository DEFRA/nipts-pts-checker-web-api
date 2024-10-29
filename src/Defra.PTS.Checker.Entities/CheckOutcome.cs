using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Entities
{
    [ExcludeFromCodeCoverage]
    [Table("CheckOutcome")]
    public class CheckOutcome
    {
        [Key]
        public Guid Id { get; set; }
        public bool? MCNotFound { get; set; }
        public bool? MCNotMatch { get; set; }

        [MaxLength(15)]
        public string? MCNotMatchActual { get; set; }

        public bool? VCNotMatchPTD { get; set; }

        public bool? OIFailPotentialCommercial { get; set; }

        public bool? OIFailAuthTravellerNoConfirmation { get; set; }

        public bool? OIFailOther { get; set; }

        public int? PassengerTypeId { get; set; }

        [MaxLength(500)]
        public string? RelevantComments { get; set; }

        public bool? GBRefersToDAERAOrSPS { get; set; }

        public bool? GBAdviseNoTravel { get; set; }

        public bool? GBPassengerSaysNoTravel { get; set; }

        public bool? SPSOutcome { get; set; }

        [MaxLength(500)]
        public string? SPSOutcomeDetails { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; } = DateTime.UtcNow;


        [ForeignKey("PassengerTypeId")]
        public virtual PasengerType? PassengerTypeNavigation { get; set; }

    }
}
