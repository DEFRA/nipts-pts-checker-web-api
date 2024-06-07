using System;
using System.Collections.Generic;
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
        public bool? PTDFailNoPTD { get; set; }
        public bool? PTDFailStatusAwaitingApproval { get; set; }
        public bool? PTDFailStatusUnsuccessfulRevokedSuspended { get; set; }
        public bool? PTDFailOther { get; set; }

        [MaxLength(300)]
        public string PTDFailOtherDetail { get; set; }

        public bool? MCFailNotFound { get; set; }
        public bool? MCFailScannerIssue { get; set; }
        public bool? VCFailSpecies { get; set; }

        [MaxLength(300)]
        public string VCFailSpeciesActual { get; set; }

        public bool? VCFailBreed { get; set; }
        public int? VCFailBreedActual { get; set; }
        public bool? VCFailColour { get; set; }
        public int? VCFailColourActual { get; set; }
        public bool? VCFailFeatures { get; set; }

        [MaxLength(300)]
        public string VCFailFeaturesActual { get; set; }

        public bool? OIFailAHCExpInv { get; set; }
        public bool? OIFailCommercial { get; set; }
        public int? OIFailCommercialLorry { get; set; }
        public int? OIFailCommercialNotInLane { get; set; }
        public int? OIFailCommercialLogos { get; set; }

        [MaxLength(300)]
        public string OIFailCommercialComments { get; set; }

        public bool? OIFailRefusedChecks { get; set; }

        [MaxLength(300)]
        public string OIFailRefusedChecksDetail { get; set; }

        public bool? OIFailOther { get; set; }

        [MaxLength(300)]
        public string OIFailOtherDetail { get; set; }

        public bool? PDSpecies { get; set; }
        public int? PDBreed { get; set; }
        public int? PDColour { get; set; }

        [MaxLength(300)]
        public string PDFeatures { get; set; }

        public int? ODType { get; set; }

        [MaxLength(300)]
        public string ODComments { get; set; }

        public int? Outcome { get; set; }
        public int? OutcomeAdvNotTravelTravelled { get; set; }
        public int? OutcomeAdvNotTravelNotTravelled { get; set; }
        public int? OutcomeAdvNotTravelNotSay { get; set; }

        [ForeignKey("Outcome")]
        public virtual Outcome OutcomeNavigation { get; set; }

        [ForeignKey("ODType")]
        public virtual PasengerType ODTypeNavigation { get; set; }

        [ForeignKey("VCFailBreedActual")]
        public virtual Breed VCFailBreedActualNavigation { get; set; }

        [ForeignKey("PDBreed")]
        public virtual Breed PDBreedNavigation { get; set; }

        [ForeignKey("VCFailColourActual")]
        public virtual Colour VCFailColourActualNavigation { get; set; }

        [ForeignKey("PDColour")]
        public virtual Colour PDColourNavigation { get; set; }
    }
}
