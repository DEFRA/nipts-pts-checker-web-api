using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Defra.PTS.Checker.Repositories.Implementation
{
    public class ApplicationRepository : Repository<Application>, IApplicationRepository
    {
        private readonly CommonDbContext _context;

        public ApplicationRepository(DbContext dbContext) : base(dbContext)
        {
            _context = dbContext as CommonDbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Application?> GetApplicationById(Guid applicationId)
        {
            if (applicationId == Guid.Empty)
            {
                throw new ArgumentException("Application ID cannot be empty.", nameof(applicationId));
            }

            return await _context.Application.FirstOrDefaultAsync(a => a.Id == applicationId)
                   ?? throw new KeyNotFoundException("Application not found.");
        }

        public async Task<bool> PerformHealthCheckLogic()
        {
            await _context.Database.OpenConnectionAsync();
            var isConnected = _context.Database.GetDbConnection().State == ConnectionState.Open;
            await _context.Database.CloseConnectionAsync();
            return isConnected;
        }

        public Application? GetMostRecentApplication(Guid petId)
        {
            var applications = _context.Application
                                        .Where(e => e.PetId == petId)
                                        .Select(e => new Application
                                        {
                                            Id = e.Id,
                                            ReferenceNumber = e.ReferenceNumber,
                                            DateOfApplication = e.DateOfApplication,
                                            Status = e.Status ?? "AWAITING VERIFICATION",
                                            DateAuthorised = e.DateAuthorised ?? DateTime.MinValue,
                                            DateRejected = e.DateRejected ?? DateTime.MinValue,
                                            DateRevoked = e.DateRevoked ?? DateTime.MinValue
                                        })
                                        .ToList();

            if (!applications.Any())
            {
                throw new ArgumentNullException(nameof(applications), "No applications found for the specified PetId.");
            }

            var mostRecentApplication = applications
                .OrderByDescending(a => new DateTime?[]
                {
                        a.DateAuthorised,
                        a.DateRejected,
                        a.DateRevoked
                }.Where(d => d.HasValue).Max() ?? DateTime.MinValue)
                .FirstOrDefault();

            return mostRecentApplication;
        }

        public async Task<IEnumerable<Application>> GetApplicationsByPetIdAsync(Guid petId)
        {
            return await _context.Application
                .Where(a => a.PetId == petId)
                .ToListAsync();
        }
    }
}
