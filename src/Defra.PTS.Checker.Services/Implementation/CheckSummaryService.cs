using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Models;
using Defra.PTS.Checker.Repositories;
using Defra.PTS.Checker.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Defra.PTS.Checker.Services.Implementation;

public class CheckSummaryService : ICheckSummaryService
{
    private readonly CommonDbContext _dbContext;
    private ILogger<CheckSummaryService> _log;

    public CheckSummaryService(CommonDbContext dbContext, ILogger<CheckSummaryService> log)
    {
        _log = log;
        _dbContext = dbContext;
    }

    public async Task<CheckOutcomeResponseModel> SaveCheckSummary(CheckOutcomeModel checkOutcomeModel)
    {
        var outcome = await _dbContext.Outcome.FirstOrDefaultAsync(x => x.Type == checkOutcomeModel.CheckOutcome);
        var travelDocument = await _dbContext!.TravelDocument
                        .Include(t => t.Application)
                        .Include(t => t.Pet)
            .SingleOrDefaultAsync(x => x.DocumentReferenceNumber == checkOutcomeModel.PTDNumber);

        ArgumentNullException.ThrowIfNull(travelDocument);
        ArgumentNullException.ThrowIfNull(outcome);

        var timeSpan = checkOutcomeModel.SailingTime - checkOutcomeModel.SailingTime.Value.Date;

        var entity = new CheckSummary
        {
            ApplicationId = travelDocument.ApplicationId,
            CheckerId = checkOutcomeModel.CheckerId,
            CheckOutcome = false,
            Date = checkOutcomeModel.SailingTime.Value.Date,
            ChipNumber = travelDocument.Application.Pet.MicrochipNumber,
            TravelDocumentId = travelDocument.Id,
            ScheduledSailingTime = timeSpan,
            RouteId = checkOutcomeModel.RouteId,
            GBCheck = false,
        };

        _dbContext.Add(entity);
        await _dbContext.SaveChangesAsync();

        var response = new CheckOutcomeResponseModel
        {
            CheckSummaryId = entity.Id
        };

        return response;
    }
}


