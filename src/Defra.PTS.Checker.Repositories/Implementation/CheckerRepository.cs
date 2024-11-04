using Defra.PTS.Checker.Repositories.Interface;
using System.Diagnostics.CodeAnalysis;
using Entity = Defra.PTS.Checker.Entities;

namespace Defra.PTS.Checker.Repositories.Implementation;

[ExcludeFromCodeCoverage]
public class CheckerRepository : Repository<Entity.Checker>, ICheckerRepository
{
    public CheckerRepository(Microsoft.EntityFrameworkCore.DbContext dbContext) : base(dbContext)
    {
    }
}
