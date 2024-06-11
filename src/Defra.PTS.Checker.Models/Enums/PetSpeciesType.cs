using System.ComponentModel;

namespace Defra.PTS.Checker.Models.Enums;

public enum PetSpeciesType
{
    [Description("")]
    None = 0,

    [Description("Dog")]
    Dog = 1,

    [Description("Cat")]
    Cat = 2,

    [Description("Ferret")]
    Ferret = 3,

    [Description("Other")]
    Other = 4
}
