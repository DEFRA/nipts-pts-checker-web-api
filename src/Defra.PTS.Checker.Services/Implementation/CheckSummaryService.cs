using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Models;
using Defra.PTS.Checker.Models.Enums;
using Defra.PTS.Checker.Repositories;
using Defra.PTS.Checker.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer; // Include this namespace
using Microsoft.Extensions.Logging;
using Microsoft.Rest.ClientRuntime.Azure.Authentication.Utilities;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TravelDocument = Defra.PTS.Checker.Entities.TravelDocument;

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
        bool isGbCheck = checkOutcomeModel.IsGBCheck;
        var checkSummaryEntity = new CheckSummary
        {
            ApplicationId = travelDocument.ApplicationId,
            CheckerId = checkOutcomeModel.CheckerId,
            CheckOutcome = outcome.Type == "Pass",
            ChipNumber = travelDocument.Application?.Pet?.MicrochipNumber,
            TravelDocumentId = travelDocument.Id,
            GBCheck = isGbCheck,
            Date = startTime,
            ScheduledSailingTime = timeSpan,
            CreatedBy = checkOutcomeModel.CheckerId,
        };

        if (checkOutcomeModel.SailingOption == (int)Defra.PTS.Checker.Models.Enums.SailingOption.Flight)
        {
            checkSummaryEntity.FlightNo = checkOutcomeModel.FlightNumber;
        }

        if (checkOutcomeModel.SailingOption == (int)SailingOption.Ferry)
        {
            checkSummaryEntity.RouteId = checkOutcomeModel?.RouteId;
        }

        var nonComplianceModel = checkOutcomeModel as NonComplianceModel;
        Guid gbCheckId = nonComplianceModel?.GBCheckId ?? Guid.Empty;
        if ((bool)checkSummaryEntity.CheckOutcome)
        {
            //Pass
            _dbContext.Add(checkSummaryEntity);
        }
        else
        {
            //Fail            
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

            // Mapping LinkedCheckId to NI Entry          
            if (!isGbCheck && gbCheckId != Guid.Empty)
            {
                checkSummaryEntity.LinkedCheckId = gbCheckId;
            }

            _dbContext.Add(checkOutcomeEntity);
            _dbContext.Add(checkSummaryEntity);
        }

        await _dbContext.SaveChangesAsync();

        // On Fail Reverse Mapping LinkedCheckId to GB Entry 
        if (!isGbCheck && gbCheckId != Guid.Empty && !(bool)checkSummaryEntity.CheckOutcome)
        {
            var gbSummary = await _dbContext.CheckSummary.FirstOrDefaultAsync(a => a.Id == gbCheckId);
            if (gbSummary != null)
            {
                gbSummary.LinkedCheckId = checkSummaryEntity.Id;
                _dbContext.Update(gbSummary);
                await _dbContext.SaveChangesAsync();
            }
        }

        var response = new CheckOutcomeResponseModel
        {
            CheckSummaryId = checkSummaryEntity.Id
        };
        return response;
    }

    public async Task<IEnumerable<CheckOutcomeResponse>> GetRecentCheckOutcomesAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            _logger.LogInformation("GetRecentCheckOutcomesAsync startDate", startDate);
            _logger.LogInformation("GetRecentCheckOutcomesAsync endDate", endDate);
            var checkOutcomes = await GetCheckOutcomesAsync(startDate, endDate);

            var results = checkOutcomes.Select(co => new CheckOutcomeResponse
            {
                RouteId = co.RouteId,
                RouteName = co.RouteNavigation?.RouteName ?? "Unknown",
                DepartureDate = co.Date.HasValue
                    ? co.Date.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                    : "N/A",
                DepartureTime = co.ScheduledSailingTime.HasValue
                    ? co.ScheduledSailingTime.Value.ToString(@"hh\:mm", CultureInfo.InvariantCulture)
                    : "N/A",
                CombinedDateTime = co.Date.HasValue && co.ScheduledSailingTime.HasValue
                    ? co.Date.Value.Add(co.ScheduledSailingTime.Value)
                    : DateTime.MinValue,
                PassCount = co.CheckOutcome == true ? 1 : 0,
                FailCount = co.CheckOutcome == false ? 1 : 0
            })
            .Where(x => x.CombinedDateTime != DateTime.MinValue) // Ensure CombinedDateTime is valid
            .GroupBy(x => new { x.RouteId, x.RouteName, x.DepartureDate, x.DepartureTime, x.CombinedDateTime })
            .Select(group => new CheckOutcomeResponse
            {
                RouteId = group.Key.RouteId,
                RouteName = group.Key.RouteName,
                DepartureDate = group.Key.DepartureDate,
                DepartureTime = group.Key.DepartureTime,
                PassCount = group.Sum(x => x.PassCount),
                FailCount = group.Sum(x => x.FailCount),
                CombinedDateTime = group.Key.CombinedDateTime
            })
            .OrderByDescending(x => x.CombinedDateTime)
            .ToList();

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetRecentCheckOutcomesAsync");
            throw;
        }
    }

    public async Task<IEnumerable<CheckSummary>> GetCheckOutcomesAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            // Preliminary filtering to reduce the dataset
            var preliminaryStartDate = startDate.Date.AddDays(-1);
            var preliminaryEndDate = endDate.Date.AddDays(1);

            var summaries = await _dbContext.CheckSummary
                .Include(cs => cs.RouteNavigation)
                .Where(cs => cs.Date.HasValue && cs.ScheduledSailingTime.HasValue && cs.RouteId.HasValue)
                .Where(cs => cs.Date!.Value >= preliminaryStartDate && cs.Date.Value <= preliminaryEndDate)
                .Where(cs => cs.GBCheck == true)
                .ToListAsync();

            // Precise filtering and combining Date and Time in memory
            var filteredSummaries = summaries
                .Select(cs => new
                {
                    CheckSummary = cs,
                    CombinedDateTime = cs?.Date!.Value.Add(cs.ScheduledSailingTime!.Value)
                })
                .Where(cs => cs.CombinedDateTime >= startDate && cs.CombinedDateTime <= endDate)
                .OrderBy(cs => cs.CombinedDateTime)
                .Select(cs => cs.CheckSummary);

            return filteredSummaries;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetCheckOutcomesAsync");
            throw;
        }
    }

    public async Task<IEnumerable<SpsCheckDetailResponseModel>> GetSpsCheckDetailsByRouteAsync(string route, DateTime sailingDate, int timeWindowInHours)
    {
        // Extract date and time from sailingDate
        DateTime sailingDateOnly = sailingDate.Date;
        TimeSpan sailingTimeOnly = sailingDate.TimeOfDay;

        // Find RouteId based on the route name
        var routeEntity = await _dbContext.Route
            .Where(r => r.RouteName == route)
            .Select(r => new { r.Id })
            .FirstOrDefaultAsync();

        if (routeEntity == null)
        {
            throw new ArgumentException($"Route '{route}' not found.");
        }

        int routeId = routeEntity.Id;

        // Check if there is a scheduled ferry at the specified date and time
        var scheduledFerryExists = await _dbContext.CheckSummary
            .AnyAsync(cs => cs.RouteId == routeId
                            && cs.Date == sailingDateOnly
                            && cs.ScheduledSailingTime == sailingTimeOnly);

        if (!scheduledFerryExists)
        {
            // No ferry is scheduled at this date and time for this route
            return new List<SpsCheckDetailResponseModel>();
        }

        // Fetch records matching the specific sailing
        List<InterimCheckSummary> checkSummaries = await getCheckSummariesBySailing(sailingDateOnly, sailingTimeOnly, routeId);

        return await getSpsCheckDetailResponse(timeWindowInHours, checkSummaries);
    }

    private async Task<List<InterimCheckSummary>> getCheckSummariesBySailing(DateTime sailingDateOnly, TimeSpan sailingTimeOnly, int routeId)
    {
        return await _dbContext.CheckSummary
            .GroupJoin(
                _dbContext.Application,
                cs => cs.ApplicationId,
                a => a.Id,
                (cs, applications) => new { CheckSummary = cs, Applications = applications.DefaultIfEmpty() }
            )
            .SelectMany(
                x => x.Applications.Select(a => new { x.CheckSummary, Application = a }),
                (x, result) => new { x.CheckSummary, Application = result.Application }
            )
            .Where(i => i.CheckSummary.RouteId == routeId
                         && i.CheckSummary.Date == sailingDateOnly
                         && i.CheckSummary.ScheduledSailingTime == sailingTimeOnly
                         && i.CheckSummary.GBCheck == true
                         && i.CheckSummary.CheckOutcome == false)
            .Select(i => new InterimCheckSummary
            {
                Id = i.CheckSummary.Id,
                Date = i.CheckSummary.Date,
                ScheduledSailingTime = i.CheckSummary.ScheduledSailingTime,
                LinkedCheckId = i.CheckSummary.LinkedCheckId,
                CheckOutcomeId = i.CheckSummary.CheckOutcomeId,
                DocumentReferenceNumber = i.Application != null && i.Application.Status != "Authorised" && i.Application.Status != "Revoked"
                            ? i.Application.ReferenceNumber
                            : GetTravelDocumentReferenceNumber(i.CheckSummary.TravelDocument!),
                PetSpeciesId = GetSpeciesId(i.CheckSummary.TravelDocument!),
                PetColourName = GetPetColourName(i.CheckSummary.TravelDocument!),
                PetOtherColour = GetPetOtherColour(i.CheckSummary.TravelDocument!),
                MicrochipNumber = GetMicrochipNumber(i.CheckSummary.TravelDocument!)
            })
            .ToListAsync();

    }

    private async Task<IEnumerable<SpsCheckDetailResponseModel>> getSpsCheckDetailResponse(int timeWindowInHours, List<InterimCheckSummary> checkSummaries)
    {
        var responseList = new List<SpsCheckDetailResponseModel>();

        foreach (var cs in checkSummaries)
        {
            var combinedDateTime = GetCombinedDateTime(cs);

            // Determine the status and travel method
            var (status, travelBy) = await GetStatusAndTravelMethod(cs, timeWindowInHours, combinedDateTime);

            // Get species description and colour
            var petSpeciesDescription = GetPetSpeciesDescription(cs.PetSpeciesId);
            var colourDescription = GetColourDescription(cs.PetColourName, cs.PetOtherColour);

            // Populate response model
            var responseItem = CreateResponseItem(cs, petSpeciesDescription, colourDescription, travelBy, status);

            // Add to response list if not already present
            if (!responseList.Contains(responseItem))
            {
                responseList.Add(responseItem);
            }
        }

        return responseList;
    }

    private static DateTime GetCombinedDateTime(InterimCheckSummary cs)
    {
        return cs.Date?.Add(cs.ScheduledSailingTime ?? TimeSpan.Zero) ?? DateTime.MinValue;
    }

    private async Task<(string status, string travelBy)> GetStatusAndTravelMethod(InterimCheckSummary cs, int timeWindowInHours, DateTime combinedDateTime)
    {
        if (cs.LinkedCheckId == null)
        {
            return GetStatusForMissingLinkedCheckId(combinedDateTime, timeWindowInHours);
        }

        return await GetStatusForLinkedCheckId(cs);
    }

    private static (string status, string travelBy) GetStatusForMissingLinkedCheckId(DateTime combinedDateTime, int timeWindowInHours)
    {
        var timeSinceSailing = DateTime.Now - combinedDateTime;
        if (timeSinceSailing.TotalHours > timeWindowInHours)
        {
            return ("", ""); // Skip "Did Not Attend" cases
        }

        return ("Check Needed", "");
    }

    private async Task<(string status, string travelBy)> GetStatusForLinkedCheckId(InterimCheckSummary cs)
    {
        var niCheckSummaryRecord = await _dbContext.CheckSummary
           .Where(co => co.Id == cs.LinkedCheckId)
           .Select(co => new { co.CheckOutcomeId })
           .FirstOrDefaultAsync();

        if (niCheckSummaryRecord == null || niCheckSummaryRecord.CheckOutcomeId == null)
        {
            return ("Check Outcome Pending", "");
        }

        var niCheck = await _dbContext.CheckOutcome
            .Where(co => co.Id == niCheckSummaryRecord.CheckOutcomeId)
            .Select(co => new { co.SPSOutcome, co.PassengerTypeId })
            .FirstOrDefaultAsync();

        if (niCheck == null)
        {
            return ("Check Outcome Pending", "");
        }

        var travelBy = GetTravelMethod(niCheck.PassengerTypeId);
        var status = niCheck.SPSOutcome == true ? "Allowed" : "Not allowed";

        return (status, travelBy);
    }


    private static string GetTravelMethod(int? passengerTypeId)
    {
        return passengerTypeId switch
        {
            1 => "Foot",
            2 => "Vehicle",
            _ => ""
        };
    }

    private string GetPetSpeciesDescription(int? petSpeciesId)
    {
        return GetEnumDescription((PetSpeciesType)(petSpeciesId ?? 0));
    }

    private static string GetColourDescription(string? petColourName, string? petOtherColour)
    {
        return petColourName ?? petOtherColour ?? "";
    }

    private static SpsCheckDetailResponseModel CreateResponseItem(InterimCheckSummary cs, string petSpeciesDescription, string colourDescription, string travelBy, string status)
    {
        return new SpsCheckDetailResponseModel
        {
            PTDNumber = cs.DocumentReferenceNumber ?? "",
            PetDescription = $"{petSpeciesDescription}, {colourDescription}",
            Microchip = cs.MicrochipNumber ?? "",
            TravelBy = travelBy,
            SPSOutcome = status,
            CheckSummaryId = cs.Id
        };
    }

    private string GetEnumDescription(PetSpeciesType speciesType)
    {
        var field = speciesType.GetType().GetField(speciesType.ToString());
        var attribute = field?.GetCustomAttributes(typeof(DescriptionAttribute), false)
                              .Cast<DescriptionAttribute>()
                              .FirstOrDefault();
        return attribute?.Description ?? speciesType.ToString();
    }

    private static string? GetTravelDocumentReferenceNumber(TravelDocument travelDocument)
    {
        return travelDocument?.DocumentReferenceNumber;
    }

    private static int? GetSpeciesId(TravelDocument travelDocument)
    {
        return travelDocument?.Pet?.SpeciesId;
    }

    private static string? GetPetColourName(TravelDocument travelDocument)
    {
        return travelDocument?.Pet?.Colour?.Name;
    }

    private static string? GetPetOtherColour(TravelDocument travelDocument)
    {
        return travelDocument?.Pet?.OtherColour;
    }

    private static string? GetMicrochipNumber(TravelDocument travelDocument)
    {
        return travelDocument?.Pet?.MicrochipNumber;
    }
}

public class InterimCheckSummary
{
    public Guid Id { get; set; }
    public DateTime? Date { get; set; }
    public TimeSpan? ScheduledSailingTime { get; set; }
    public Guid? LinkedCheckId { get; set; }
    public Guid? CheckOutcomeId { get; set; }
    public string? DocumentReferenceNumber { get; set; }
    public int? PetSpeciesId { get; set; }
    public string? PetColourName { get; set; }
    public string? PetOtherColour { get; set; }
    public string? MicrochipNumber { get; set; }
}


