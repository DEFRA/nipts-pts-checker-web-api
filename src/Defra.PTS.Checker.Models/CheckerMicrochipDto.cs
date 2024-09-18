using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Models;

[ExcludeFromCodeCoverage]
public class CheckerMicrochipDto
{   
    [Required(ErrorMessage = "Microchip number is required")]
    public string? MicrochipNumber { get; set; }
}