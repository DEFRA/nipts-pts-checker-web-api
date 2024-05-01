using entity = Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Repositories.Implementation
{
    [ExcludeFromCodeCoverage]
    public class PetRepository : Repository<entity.Pet>, IPetRepository
    {
        private CommonDbContext petContext
        {
            get
            {
                return _dbContext as CommonDbContext;
            }
        }

        public PetRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
