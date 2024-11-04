using Defra.PTS.Checker.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Entity = Defra.PTS.Checker.Entities;

namespace Defra.PTS.Checker.Repositories.Implementation
{
    [ExcludeFromCodeCoverage]
    public class BreedRepository : Repository<Entity.Breed>, IBreedRepository
    {
        public BreedRepository(DbContext dbContext) : base(dbContext)
        {

        }
    }
}
