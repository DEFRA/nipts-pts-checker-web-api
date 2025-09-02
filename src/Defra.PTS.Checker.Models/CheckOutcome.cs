using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.PTS.Checker.Models
{
    [ExcludeFromCodeCoverage]
    public class CheckOutcome
    {
        //Check Outcome
        public bool? GBRefersToDAERAOrSPS { get; set; }
        public bool? GBAdviseNoTravel { get; set; }
        public bool? GBPassengerSaysNoTravel { get; set; }

        //Reason for referral
        public bool? MCNotMatch { get; set; }
        public string? MCNotMatchActual { get; set; }
        public bool? MCNotFound { get; set; }
        public bool? VCNotMatchPTD { get; set; }
        public bool? OIFailPotentialCommercial { get; set; }
        public bool? OIFailAuthTravellerNoConfirmation { get; set; }
        public bool? OIRefusedToSignDeclaration { get; set; }
        public bool? OIFailOther { get; set; }

        //Additional Comments
        public string? RelevantComments { get; set; } = string.Empty;
    }
}
