using System.ComponentModel;

namespace Defra.PTS.Checker.Models.Enums
{
    public enum Status
    {
        [Description("Authorised")]
        Authorised = 1,
        [Description("Rejected")]
        Rejected = 2,
        [Description("Revoked")]
        Revoked = 3,
        [Description("Awaiting Verifictaion")]
        AwaitingVerifictaion = 4,
    }
}
