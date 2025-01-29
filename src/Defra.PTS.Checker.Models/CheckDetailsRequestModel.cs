using System.ComponentModel.DataAnnotations;

namespace Defra.PTS.Checker.Models
{
    public class CheckDetailsRequestModel
    {
        [Required]
        public Guid CheckSummaryId { get; set; } 
    }
}
