using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.PTS.Checker.Models
{
    public class CheckOutcomeRequest
    {
        public string CheckSummaryId { get; set; }
        public string CheckOutcome { get; set; }
        public string CheckOutcomeDetails { get; set; }
    }

}
