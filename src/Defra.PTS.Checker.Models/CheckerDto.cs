﻿using System.ComponentModel.DataAnnotations;

namespace Defra.PTS.Checker.Models;

public class CheckerDto
{
    [Required(ErrorMessage = "Checker Id is required")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Checker first name is required")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Checker last name is required")]
    public string? LastName { get; set; }

    public int? RoleId { get; set; }
}