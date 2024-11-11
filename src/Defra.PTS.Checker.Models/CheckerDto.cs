using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Models;

[ExcludeFromCodeCoverage]
public class CheckerDto
{
    [Required(ErrorMessage = "Checker Id is required")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Checker first name is required")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Checker last name is required")]
    public string? LastName { get; set; }

    public int? RoleId { get; set; }

    public Guid? OrganisationId { get; set; }
}