using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.PTS.Checker.Models.Enums
{
    public enum SailingOption
    {
        [Description("Ferry")]
        Ferry = 1,

        [Description("Flight")]
        Flight = 2
    }
}
