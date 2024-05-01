using Defra.PTS.Checker.Services.Interface;
using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Repositories.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.PTS.Checker.Services.Implementation
{
    public class ColorService : IColorService
    {
        private readonly IRepository<Color> _colorRepository;
        private ILogger<ApplicationService> _log;
        public ColorService(ILogger<ApplicationService> log,IRepository<Color> colorRepository)
        {
            _log = log;
            _colorRepository = colorRepository;
        }

        public Task<IEnumerable<Color>> GetColor()
        {
            _log.LogInformation("Running inside method {0}", "GetColor");
            var colors = _colorRepository.GetAll();
            return (Task<IEnumerable<Color>>)colors;
        }
    }
}
