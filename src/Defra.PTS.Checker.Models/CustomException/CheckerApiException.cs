using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.PTS.Checker.Models.CustomException
{
    [ExcludeFromCodeCoverage]
    public class CheckerApiException : Exception
    {
        public CheckerApiException() { }

        public CheckerApiException(string message) : base(message) { }

        public CheckerApiException(string message, Exception innerException) : base(message, innerException) { }
    }
}
