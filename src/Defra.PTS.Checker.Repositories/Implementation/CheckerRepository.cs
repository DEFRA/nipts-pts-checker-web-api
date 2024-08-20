using Defra.PTS.Checker.Repositories.Interface;
using System.Diagnostics.CodeAnalysis;
using entity = Defra.PTS.Checker.Entities;

namespace Defra.PTS.Checker.Repositories.Implementation;

[ExcludeFromCodeCoverage]
public class CheckerRepository : Repository<entity.Checker>, ICheckerRepository
{

    private CommonDbContext userContext
    {
        get
        {
            return _dbContext as CommonDbContext;
        }
    }

    public CheckerRepository(Microsoft.EntityFrameworkCore.DbContext dbContext) : base(dbContext)
    {
    }
}
