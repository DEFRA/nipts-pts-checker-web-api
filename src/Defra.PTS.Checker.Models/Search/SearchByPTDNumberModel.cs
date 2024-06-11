using System.ComponentModel.DataAnnotations;

namespace Defra.PTS.Checker.Models.Search;

public class SearchByPTDNumberModel
{
    [Required]
    public string PTDNumber {  get; set; }
}
