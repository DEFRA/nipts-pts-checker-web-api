using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Models;
using Defra.PTS.Checker.Repositories;
using Defra.PTS.Checker.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace Defra.PTS.Checker.Services.Implementation;

public class CheckSummaryService : ICheckSummaryService
{
    private readonly CommonDbContext _dbContext;

    public CheckSummaryService(CommonDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CheckOutcomeResponseModel> SaveCheckSummary(CheckOutcomeModel checkOutcomeModel)
    {
        var outcome = await _dbContext.Outcome.FirstOrDefaultAsync(x => x.Type == checkOutcomeModel.CheckOutcome);
        var travelDocument = await _dbContext!.TravelDocument
                        .Include(t => t.Application)
                        .Include(t => t.Pet)
            .SingleOrDefaultAsync(x => x.ApplicationId == checkOutcomeModel.ApplicationId);

        ArgumentNullException.ThrowIfNull(travelDocument);
        ArgumentNullException.ThrowIfNull(outcome);

        var startTime = DateTime.MinValue.Date;
        var endTime = DateTime.MinValue;

        if (checkOutcomeModel.SailingTime.HasValue)
        {
            startTime = checkOutcomeModel.SailingTime.Value.Date;
            endTime = checkOutcomeModel.SailingTime.Value;
        }

        var timeSpan = endTime - startTime;

        var checkOutcomeEntity = new CheckOutcome
        {
            Outcome = outcome.Id,
        };

        var entity = new CheckSummary
        {
            ApplicationId = travelDocument.ApplicationId,
            CheckerId = checkOutcomeModel.CheckerId,
            CheckOutcome = outcome.Id == 1,
            CheckOutcomeId = checkOutcomeEntity.Id,
            CheckOutcomeEntity = checkOutcomeEntity,
            Date = startTime,
            ChipNumber = travelDocument.Application?.Pet?.MicrochipNumber,
            TravelDocumentId = travelDocument.Id,
            ScheduledSailingTime = timeSpan,
            RouteId = checkOutcomeModel?.RouteId,
            GBCheck = false,
        };


        _dbContext.Add(checkOutcomeEntity);
        _dbContext.Add(entity);
        await _dbContext.SaveChangesAsync();

        var response = new CheckOutcomeResponseModel
        {
            CheckSummaryId = entity.Id
        };

        return response;
    }
}


