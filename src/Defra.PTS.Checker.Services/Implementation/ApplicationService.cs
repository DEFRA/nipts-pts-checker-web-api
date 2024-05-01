using Defra.PTS.Checker.Services.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.PTS.Checker.Services.Implementation
{
    public class ApplicationService : IApplicationService
    {
        private ILogger<ApplicationService> _log;
        public ApplicationService(ILogger<ApplicationService> log)
        {
            _log = log;  
        }
        public string GetApplication()
        {
            _log.LogInformation("Running inside method {0}", "GetApplication");
            return "Application Service Sample is Running";
        }
    }
}
