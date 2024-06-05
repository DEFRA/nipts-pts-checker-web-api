using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.PTS.Checker.Services.Interface
{
    public interface ISailingService
    {
        Task<IEnumerable<RouteResponse>> GetAllSailingRoutes();
    }
}
