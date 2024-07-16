using Defra.PTS.Checker.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using entity = Defra.PTS.Checker.Entities;

namespace Defra.PTS.Checker.Repositories.Implementation
{
    [ExcludeFromCodeCoverage]
    public class ColourRepository : Repository<entity.Colour>, IColourRepository
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
