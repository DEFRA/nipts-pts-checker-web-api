using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Models;
using Defra.PTS.Checker.Models.Enums;
using Defra.PTS.Checker.Repositories;
using Defra.PTS.Checker.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Defra.PTS.Checker.Services.Implementation;

public class CheckSummaryService : ICheckSummaryService
{
    private readonly CommonDbContext _dbContext;
    private readonly ILogger<CheckerService> _logger;

    public CheckSummaryService(CommonDbContext dbContext, ILogger<CheckerService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
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

        //TODO Check if the date value being populated is correct
        var checkSummaryEntity = new CheckSummary
        {
            ApplicationId = travelDocument.ApplicationId,
            CheckerId = checkOutcomeModel.CheckerId,
            CheckOutcome = outcome.Id == 1,
            ChipNumber = travelDocument.Application?.Pet?.MicrochipNumber,
            TravelDocumentId = travelDocument.Id,
            GBCheck = checkOutcomeModel.IsGBCheck,
            Date = startTime,
            ScheduledSailingTime = timeSpan,
            CreatedBy = checkOutcomeModel.CheckerId,
        };

        if (checkOutcomeModel.SailingOption ==  (int)SailingOption.Flight)
        {
            checkSummaryEntity.FlightNo = checkOutcomeModel.FlightNumber;
        }

        if (checkOutcomeModel.SailingOption == (int)SailingOption.Ferry)
        {
            checkSummaryEntity.RouteId = checkOutcomeModel?.RouteId;
        }

        
        if ((bool)checkSummaryEntity.CheckOutcome)
        {
            //Pass
            _dbContext.Add(checkSummaryEntity);
        }
        else
        {
            //Fail
            var nonComplianceModel = checkOutcomeModel as NonComplianceModel;
            var checkOutcomeEntity = new CheckOutcome
            {
                MCNotMatch = nonComplianceModel?.MCNotMatch,
                MCNotMatchActual = nonComplianceModel?.MCNotMatchActual,
                MCNotFound = nonComplianceModel?.MCNotFound,
                VCNotMatchPTD = nonComplianceModel?.VCNotMatchPTD,
                OIFailPotentialCommercial = nonComplianceModel?.OIFailPotentialCommercial,
                OIFailAuthTravellerNoConfirmation = nonComplianceModel?.OIFailAuthTravellerNoConfirmation,
                OIFailOther = nonComplianceModel?.OIFailOther,
                PassengerTypeId = nonComplianceModel?.PassengerTypeId,
                RelevantComments = nonComplianceModel?.RelevantComments,
                GBRefersToDAERAOrSPS = nonComplianceModel?.GBRefersToDAERAOrSPS,
                GBAdviseNoTravel = nonComplianceModel?.GBAdviseNoTravel,
                GBPassengerSaysNoTravel = nonComplianceModel?.GBPassengerSaysNoTravel,
                SPSOutcome = nonComplianceModel?.SPSOutcome,
                SPSOutcomeDetails = nonComplianceModel?.SPSOutcomeDetails,
                CreatedBy = nonComplianceModel?.CheckerId,
            };
            checkSummaryEntity.CheckOutcomeId = checkOutcomeEntity.Id;
            checkSummaryEntity.CheckOutcomeEntity = checkOutcomeEntity;

            _dbContext.Add(checkOutcomeEntity);
            _dbContext.Add(checkSummaryEntity);
        }

        await _dbContext.SaveChangesAsync();
        var response = new CheckOutcomeResponseModel
        {
            CheckSummaryId = checkSummaryEntity.Id
        };
        return response;
    }

    public async Task<IEnumerable<CheckOutcomeResponse>> GetRecentCheckOutcomesAsync(DateTime startDate, DateTime endDate)
    {
        var checkOutcomes = await GetCheckOutcomesAsync(startDate, endDate);

        var results = checkOutcomes.Select(co => new CheckOutcomeResponse
        {
            RouteName = co.RouteNavigation?.RouteName ?? "Unknown",
            DepartureDate = co.Date?.ToShortDateString() ?? "N/A",
            DepartureTime = co.ScheduledSailingTime.HasValue ? co.ScheduledSailingTime.Value.ToString(@"hh\:mm") : "N/A",
            PassCount = co.CheckOutcome == true ? 1 : 0,
            FailCount = co.CheckOutcome == false ? 1 : 0
        })
        .GroupBy(x => new { x.RouteName, x.DepartureDate, x.DepartureTime })
        .Select(group => new CheckOutcomeResponse
        {
            RouteName = group.Key.RouteName,
            DepartureDate = group.Key.DepartureDate,
            DepartureTime = group.Key.DepartureTime,
            PassCount = group.Sum(x => x.PassCount),
            FailCount = group.Sum(x => x.FailCount)
        })
        .OrderByDescending(x => DateTime.Parse($"{x.DepartureDate} {x.DepartureTime}"))
        .ToList();

        return results;
    }



    public async Task<IEnumerable<CheckSummary>> GetCheckOutcomesAsync(DateTime startDate, DateTime endDate)
    {
        // Pre-filter on approximate date range to reduce data fetched
        var dateRangeStart = startDate.Date.AddDays(-1);
        var dateRangeEnd = endDate.Date.AddDays(1);

        var summaries = await _dbContext.CheckSummary
            .Include(cs => cs.RouteNavigation)
            .Where(cs => cs.Date.HasValue && cs.ScheduledSailingTime.HasValue)
            .Where(cs => cs.Date >= dateRangeStart && cs.Date <= dateRangeEnd)
            .ToListAsync();

        // Perform calculation and filtering in memory
        var filteredSummaries = summaries
            .Select(cs => new
            {
                CheckSummary = cs,
                CombinedDateTime = cs.Date.Value.Add(cs.ScheduledSailingTime.Value)
            })
            .Where(cs => cs.CombinedDateTime >= startDate && cs.CombinedDateTime <= endDate)
            .OrderByDescending(cs => cs.CombinedDateTime)
            .Select(cs => cs.CheckSummary);

        return filteredSummaries;
    }

}


