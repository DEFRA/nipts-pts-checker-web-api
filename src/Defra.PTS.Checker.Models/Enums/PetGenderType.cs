using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Defra.PTS.Checker.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]

public enum PetGenderType
{
    [Description("Male")]
    Male = 1,

    [Description("Female")]
    Female = 2
}
