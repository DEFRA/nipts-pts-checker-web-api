using Defra.PTS.Checker.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using entity = Defra.PTS.Checker.Entities;

namespace Defra.PTS.Checker.Repositories.Implementation
{
    [ExcludeFromCodeCoverage]
    public class SailingRepository : Repository<entity.Route>, ISailingRepository
    {
        private CommonDbContext sailingContext
        {
            get
            {
                return _dbContext as CommonDbContext;
            }
        }

        public SailingRepository(DbContext dbContext) : base(dbContext)
        {

        }
    }
}
