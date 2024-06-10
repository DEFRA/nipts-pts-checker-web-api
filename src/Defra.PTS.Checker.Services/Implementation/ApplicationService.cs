using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Repositories.Implementation;
using Defra.PTS.Checker.Repositories.Interface;
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
        private readonly IApplicationRepository _applicationRepository;
        public ApplicationService(ILogger<ApplicationService> log, IApplicationRepository applicationRepository)
        {
            _log = log; 
            _applicationRepository = applicationRepository;
        }
        public Task<Application> GetApplicationById(Guid id)
        {
            _log.LogInformation("Running inside method {0}", "GetApplication");
            var application = _applicationRepository.GetApplicationById(id);
            return application;
        }

        //public Task<IEnumerable<Application>> GetAllApplications()
        //{
        //    _log.LogInformation("Running inside method {0}", "GetAllApplications");
        //    var applications = _applicationRepository.GetAll();
        //    return Task.FromResult(applications);
        //}
    }
}
