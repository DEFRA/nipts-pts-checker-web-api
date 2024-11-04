using Defra.PTS.Checker.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Entity = Defra.PTS.Checker.Entities;

namespace Defra.PTS.Checker.Repositories.Implementation
{
    [ExcludeFromCodeCoverage]
    public class SailingRepository : Repository<Entity.Route>, ISailingRepository
    {
        public SailingRepository(DbContext dbContext) : base(dbContext)
        {

        }
    }
}
