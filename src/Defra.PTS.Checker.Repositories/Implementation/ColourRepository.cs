using entity = Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Repositories.Implementation
{
    [ExcludeFromCodeCoverage]
    public class ColourRepository : Repository<entity.Color>, IColourRepository
    {
        private CommonDbContext colourContext
        {
            get
            {
                return _dbContext as CommonDbContext;
            }
        }

        public ColourRepository(DbContext dbContext) : base(dbContext)
        {

        }
    }
}
