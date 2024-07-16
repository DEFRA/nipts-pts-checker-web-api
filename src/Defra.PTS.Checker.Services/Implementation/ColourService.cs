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
    public class ColourService : IColourService
    {
        private readonly IRepository<Colour> _colourRepository;
        private ILogger<ColourService> _log;
        public ColourService(ILogger<ColourService> log, IRepository<Colour> colourRepository)
        {
            _log = log;
            _colourRepository = colourRepository;
        }

        public Task<IEnumerable<Colour>> GetAllColours()
        {
            _log.LogInformation("Running inside method {0}", "GetColour");
            var colours = _colourRepository.GetAll();
            return Task.FromResult(colours);
        }
    }
}
