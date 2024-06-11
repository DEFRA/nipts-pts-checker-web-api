using Defra.PTS.Checker.Entities;

﻿using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using entity = Defra.PTS.Checker.Entities;

namespace Defra.PTS.Checker.Repositories.Implementation
{
    [ExcludeFromCodeCoverage]
    public class TravelDocumentRepository : Repository<TravelDocument>, ITravelDocumentRepository
    {
        private readonly CommonDbContext _context;

        public TravelDocumentRepository(DbContext dbContext) : base(dbContext)
        {
            _context = dbContext as CommonDbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<TravelDocument> GetTravelDocument(Guid? applicationId, Guid? ownerId, Guid? petId)
        {
            if (applicationId == null || ownerId == null || petId == null)
            {
                throw new ArgumentNullException("Application ID, Owner ID, and Pet ID are required.");
            }

            return await _context.TravelDocument
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId && a.OwnerId == ownerId && a.PetId == petId)
               ?? throw new KeyNotFoundException("Travel document not found.");
        }

        public async Task<TravelDocument> GetTravelDocumentByReferenceNumber(string referenceNumber)
        {
            return await _context!.TravelDocument
                .Include(t => t.Application)
                .Include(t => t.Owner)
                .Include(t => t.Pet)
                .Include(t => t.Pet!.Breed)
                .Include(t => t.Pet!.Colour)
                .FirstOrDefaultAsync(a => a.DocumentReferenceNumber == referenceNumber) ?? null!;
        }

        public async Task<TravelDocument> GetTravelDocumentByPTDNumber(string ptdNumber)
        {
            ArgumentNullException.ThrowIfNull(_context);
            ArgumentNullException.ThrowIfNull(ptdNumber);

            return await _context!.TravelDocument
                        .Include(t => t.Application)
                        .Include(t => t.Owner)
                        .Include(t => t.Pet)
                        .Include(t => t.Pet!.Breed)
                        .Include(t => t.Pet!.Colour)
                        .SingleOrDefaultAsync(x => x.DocumentReferenceNumber == ptdNumber) ?? null!;
        }

        public async Task<IEnumerable<TravelDocument>> GetByPetIdAsync(Guid petId)
        {
            var applications = await _context.Application
                .Where(a => a.PetId == petId)
                .ToListAsync();

            var applicationIds = applications.Select(a => a.Id).ToList();

            return await _context.TravelDocument
                .Where(td => applicationIds.Contains(td.ApplicationId))
                .ToListAsync();
        }

        public async Task<TravelDocument?> GetTravelDocumentByApplicationIdAsync(Guid applicationId)
        {

            var travelDocument = await _context.TravelDocument
                .Where(td => td.ApplicationId == applicationId)
                .Select(td => new TravelDocument
                {
                    Id = td.Id,
                    DocumentReferenceNumber = td.DocumentReferenceNumber,
                    DateOfIssue = td.DateOfIssue,
                    ValidityStartDate = td.ValidityStartDate,
                    ValidityEndDate = td.ValidityEndDate,
                    StatusId = td.StatusId,
                    PetId = td.PetId,
                    OwnerId = td.OwnerId,
                    ApplicationId = td.ApplicationId
                }).FirstOrDefaultAsync();

            return travelDocument;
        }

    }
}