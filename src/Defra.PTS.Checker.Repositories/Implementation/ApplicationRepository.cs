using entity = Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Repositories.Implementation
{
    [ExcludeFromCodeCoverage]
    public class ApplicationRepository : Repository<entity.Application>, IApplicationRepository
    {
        private CommonDbContext commonContext
        {
            get
            {
                return _dbContext as CommonDbContext;
            }
        }

        public ApplicationRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public async Task<entity.Application> GetApplicationById(Guid applicationId)
        {
            return await commonContext.Application
                .Include(a => a.Pet)
                .Include(a => a.Owner)
                .Include(a => a.Pet)
                .Include(a => a.Pet.Breed)
                .Include(a => a.Pet.Colour)
                .FirstOrDefaultAsync(a => a.Id == applicationId);
        }

        public async Task<entity.TravelDocument> GetTravelDocumentByApplicationId(Guid applicationId)
        {
            return await commonContext.TravelDocument
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);
        }

        public async Task<bool> PerformHealthCheckLogic()
        {
            // Attempt to open a connection to the database
            await _dbContext.Database.OpenConnectionAsync();

            // Check if the connection is open
            if (_dbContext.Database.GetDbConnection().State == ConnectionState.Open)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
