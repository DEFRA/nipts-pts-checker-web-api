using Defra.PTS.Checker.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using entity = Defra.PTS.Checker.Entities;

namespace Defra.PTS.Checker.Repositories.Implementation
{
    [ExcludeFromCodeCoverage]
    public class PetRepository : Repository<entity.Pet>, IPetRepository
    {
        private readonly CommonDbContext _context;

        public PetRepository(DbContext dbContext) : base(dbContext)
        {
            _context = dbContext as CommonDbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<entity.Pet>> GetByMicrochipNumberAsync(string microchipNumber)
        {
            if (string.IsNullOrEmpty(microchipNumber))
            {
                throw new ArgumentException("Microchip number cannot be null or empty.", nameof(microchipNumber));
            }

         
            return await _context.Pet
            .Where(p => p.MicrochipNumber == microchipNumber)            
            .Include(p => p.Breed)
            .Include(p => p.Colour)
            .ToListAsync();
        }
    }
}