using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Entities
{
    [ExcludeFromCodeCoverage]
    [Table("CheckSummary")]
    public class CheckSummary
    {
        [Key]
        public Guid Id { get; set; }

        public bool? GBCheck { get; set; }

        public Guid? LinkedCheckId { get; set; }

        public Guid? CheckerId { get; set; }

        public int? RouteId { get; set; }

        public DateTime? Date { get; set; }

        public TimeSpan? ScheduledSailingTime { get; set; }

        public Guid ApplicationId { get; set; }

        public Guid TravelDocumentId { get; set; }

        [MaxLength(15)]
        public string? ChipNumber { get; set; }

        public bool? CheckOutcome { get; set; }

        public Guid? CheckOutcomeId { get; set; }

        // Navigation properties
        [ForeignKey("CheckOutcomeId")]
        public virtual CheckOutcome? CheckOutcomeEntity { get; set; }

        [ForeignKey("ApplicationId")]
        public virtual Application? Application { get; set; }

        [ForeignKey("TravelDocumentId")]
        public virtual TravelDocument? TravelDocument { get; set; }

        [ForeignKey("RouteId")]
        public virtual Route? RouteNavigation { get; set; }

        [ForeignKey("CheckerId")]
        public virtual Checker? Checker { get; set; }

        [ForeignKey("LinkedCheckId")]
        public virtual CheckSummary? LinkedCheck { get; set; }
    }
}
