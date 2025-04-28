using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Models;
using Defra.PTS.Checker.Models.Enums;
using Defra.PTS.Checker.Repositories;
using Defra.PTS.Checker.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Globalization;
using TravelDocument = Defra.PTS.Checker.Entities.TravelDocument;
using CheckOutcome = Defra.PTS.Checker.Entities.CheckOutcome;
using Defra.PTS.Checker.Models.CustomException;
using Defra.PTS.Checker.Services.Helpers;

namespace Defra.PTS.Checker.Services.Implementation;

public class CheckSummaryService : ICheckSummaryService
{
    private readonly CommonDbContext _dbContext;
    private readonly ILogger<CheckSummaryService> _logger;

    public CheckSummaryService(CommonDbContext dbContext, ILogger<CheckSummaryService> logger)
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

        await SetDuplicateChecksAsSuperceded(checkSummaryEntity);

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

    public async Task SetDuplicateChecksAsSuperceded(CheckSummary checkSummary)
    {
        var duplicateChecks = await _dbContext.CheckSummary
             .Where(c => c.GBCheck == checkSummary.GBCheck 
                        && c.ApplicationId == checkSummary.ApplicationId
                        && c.RouteId == checkSummary.RouteId
                        && c.Date == checkSummary.Date
                        && c.ScheduledSailingTime == checkSummary.ScheduledSailingTime
                        && c.FlightNo == checkSummary.FlightNo).ToListAsync();

        foreach (var check in duplicateChecks)
        {
            check.Superseded = true;
            _dbContext.CheckSummary.Update(check);
        }
    }

    public async Task<IEnumerable<CheckOutcomeResponse>> GetRecentCheckOutcomesAsync(DateTime startDate, DateTime endDate)
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

    public async Task<IEnumerable<CheckSummary>> GetCheckOutcomesAsync(DateTime startDate, DateTime endDate)
    {
        // Preliminary filtering to reduce the dataset
        var preliminaryStartDate = startDate.Date.AddDays(-1);
        var preliminaryEndDate = endDate.Date.AddDays(1);

        var summaries = await _dbContext.CheckSummary
            .Include(cs => cs.RouteNavigation)
            .Where(cs => cs.Date.HasValue && cs.ScheduledSailingTime.HasValue && cs.RouteId.HasValue)
            .Where(cs => cs.Date!.Value >= preliminaryStartDate && cs.Date.Value <= preliminaryEndDate)
            .Where(cs => cs.GBCheck == true)
            .Where(cs => cs.Superseded == null || cs.Superseded == false)
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

    public async Task<GbCheckReportResponseModel?> GetGbCheckReport(Guid gbCheckSummaryId)
    {
        try
        {
            var checkReport = await _dbContext!.CheckSummary
                .Include(t => t.CheckOutcomeEntity)
                .Include(t => t.Checker)
                .SingleOrDefaultAsync(x => x.Id == gbCheckSummaryId);

            if (checkReport == null)
            {
                _logger.LogInformation("No check report found with gbCheckSummaryId: {0}", gbCheckSummaryId);
                return null;
            }

            return new GbCheckReportResponseModel
            {
                GbCheckSummaryId = gbCheckSummaryId,
                CheckDetails = new CheckDetails() 
                {
                    CheckersName = checkReport.Checker?.FullName ?? checkReport.Checker?.FirstName + " " + checkReport.Checker?.LastName,
                    DateAndTimeChecked = checkReport.CreatedOn,
                    DepartureDate = checkReport.Date.HasValue? checkReport.Date.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture): null,
                    DepartureTime = checkReport.ScheduledSailingTime.HasValue ? checkReport.ScheduledSailingTime.Value.ToString(@"hh\:mm") : null,
                    RouteId = checkReport.RouteId,
                },
                CheckOutcome = new Models.CheckOutcome()
                {                    
                    GBRefersToDAERAOrSPS = checkReport.CheckOutcomeEntity?.GBRefersToDAERAOrSPS,
                    GBAdviseNoTravel = checkReport.CheckOutcomeEntity?.GBAdviseNoTravel,
                    GBPassengerSaysNoTravel = checkReport.CheckOutcomeEntity?.GBPassengerSaysNoTravel,

                    MCNotMatch = checkReport.CheckOutcomeEntity?.MCNotMatch,
                    MCNotMatchActual = checkReport.CheckOutcomeEntity?.MCNotMatchActual,
                    MCNotFound = checkReport.CheckOutcomeEntity?.MCNotFound,
                    VCNotMatchPTD = checkReport.CheckOutcomeEntity?.VCNotMatchPTD,
                    OIFailPotentialCommercial = checkReport.CheckOutcomeEntity?.OIFailPotentialCommercial,
                    OIFailAuthTravellerNoConfirmation = checkReport.CheckOutcomeEntity?.OIFailAuthTravellerNoConfirmation,
                    OIFailOther = checkReport.CheckOutcomeEntity?.OIFailOther,

                    RelevantComments = checkReport.CheckOutcomeEntity?.RelevantComments,
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetGbCheckReport");
            throw new CheckerApiException("Error in GetGbCheckReport", ex);
        }
    }




    public async Task<CompleteCheckDetailsResponse?> GetCompleteCheckDetailsAsync(Guid checkSummaryId)
    {
        try
        {
            var checkSummary = await _dbContext.CheckSummary
                .Include(cs => cs.Application)
                    .ThenInclude(app => app != null ? app.Pet : null)
                .Include(cs => cs.CheckOutcomeEntity)
                .Include(cs => cs.Checker)
                .Include(cs => cs.RouteNavigation)
                .FirstOrDefaultAsync(cs => cs.Id == checkSummaryId);

            if (checkSummary == null)
            {
                return null;
            }

            await _dbContext.Entry(checkSummary)
                .Reference(cs => cs.TravelDocument)
                .LoadAsync();

            var checkOutcomes = await _dbContext.CheckOutcome
                .Where(co => co.Id == checkSummary.CheckOutcomeId)
                .ToListAsync();

            var referralTexts = AddReferralTexts(checkOutcomes);

            var outcomeReasons = new List<string>();
            if (checkOutcomes.Any(o => o.GBRefersToDAERAOrSPS == true))
                outcomeReasons.Add("Passenger referred to DAERA/SPS at NI port");
            if (checkOutcomes.Any(o => o.GBAdviseNoTravel == true))
                outcomeReasons.Add("Passenger advised not to travel");
            if (checkOutcomes.Any(o => o.GBPassengerSaysNoTravel == true))
                outcomeReasons.Add("Passenger says they will not travel");

            var displayMicrochipNumber = checkOutcomes.Any(o =>
                o.MCNotMatch == true || !string.IsNullOrWhiteSpace(o.MCNotMatchActual));

            var additionalComments = checkOutcomes
                .Select(o => o.RelevantComments ?? "None")
                .ToList();

            var detailsComments = checkOutcomes
                .Select(o => o.SPSOutcomeDetails ?? "None")
                .ToList();

            var response = new CompleteCheckDetailsResponse
            {
                CheckOutcome = outcomeReasons,
                ReasonForReferral = referralTexts,
                MicrochipNumber = displayMicrochipNumber ? checkSummary.ChipNumber : null,
                AdditionalComments = additionalComments,
                DetailsComments = detailsComments,
                GBCheckerName = checkSummary.Checker?.FullName ?? string.Empty,
                DateAndTimeChecked = checkSummary.UpdatedOn?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty,
                Route = checkSummary.RouteNavigation?.RouteName ?? string.Empty,
                ScheduledDepartureDate = checkSummary.Date?.ToString("yyyy-MM-dd") ?? string.Empty,
                ScheduledDepartureTime = checkSummary.ScheduledSailingTime?.ToString(@"hh\:mm\:ss") ?? string.Empty
            };

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving complete check details.");
            return null;
        }
    }

    private static List<string> AddReferralTexts(List<CheckOutcome> checkOutcomes)
    {

        var referralTexts = new List<string>();
        foreach (var referral in checkOutcomes)
        {
            if (referral.MCNotMatch == true) referralTexts.Add("Microchip number does not match the PTD");
            if (referral.MCNotFound == true) referralTexts.Add("Cannot find microchip");
            if (referral.VCNotMatchPTD == true) referralTexts.Add("Pet does not match the PTD");
            if (referral.OIFailPotentialCommercial == true) referralTexts.Add("Potential commercial movement");
            if (referral.OIFailAuthTravellerNoConfirmation == true) referralTexts.Add("Authorised person but no confirmation");
            if (referral.OIFailOther == true) referralTexts.Add("Other reason");
        }

        return referralTexts;
    }


    private async Task<List<InterimCheckSummary>> getCheckSummariesBySailing(DateTime sailingDateOnly, TimeSpan sailingTimeOnly, int routeId)
    {

        var checkSummaries = _dbContext.CheckSummary
            .Include(c => c.Application)
            .Include(c => c.TravelDocument)
            .Include(c => c.TravelDocument.Pet)
            .Where(c => c.RouteId == routeId 
            && c.Date == sailingDateOnly 
            && c.ScheduledSailingTime == sailingTimeOnly
            && c.GBCheck == true
            && c.CheckOutcome == false
            && (c.Superseded == null || c.Superseded == false))
            .Select(i => new InterimCheckSummary
            {
                Id = i.Id,
                Date = i.Date,
                ScheduledSailingTime = i.ScheduledSailingTime,
                LinkedCheckId = i.LinkedCheckId,
                CheckOutcomeId = i.CheckOutcomeId,
                DocumentReferenceNumber = i.Application.Status != "Authorised" && i.Application.Status != "Revoked"
                ? i.Application.ReferenceNumber : (GetTravelDocumentReferenceNumber(i.TravelDocument!)),
                PetSpeciesId = i.TravelDocument != null && i.TravelDocument.Pet != null
                    ? i.TravelDocument.Pet.SpeciesId
                    : (int?)null,
                PetColourName = i.TravelDocument != null && i.TravelDocument.Pet != null
                     && i.TravelDocument.Pet.Colour != null
                        ? i.TravelDocument.Pet.Colour.Name
                        : null,
                PetOtherColour = i.TravelDocument != null && i.TravelDocument.Pet != null
                       ? i.TravelDocument.Pet.OtherColour
                       : null,
                MicrochipNumber = i.TravelDocument != null && i.TravelDocument.Pet != null
                        ? i.TravelDocument.Pet.MicrochipNumber
                        : null
            })
            .ToListAsync();

        return await checkSummaries;
    }

    private async Task<IEnumerable<SpsCheckDetailResponseModel>> getSpsCheckDetailResponse(int timeWindowInHours, List<InterimCheckSummary> checkSummaries)
    {
        var responseList = new List<SpsCheckDetailResponseModel>();
        string checkNeededText = "Check needed";

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

        //Split list into check needed and not needed
        var responseListCheckNeeded = responseList
            .Where(x => x.SPSOutcome == checkNeededText)
            .ToList();
        var responseListNoCheckNeeded = responseList
            .Where(x => x.SPSOutcome != checkNeededText)
            .ToList();

        // Sort both lists
        responseListCheckNeeded = responseListCheckNeeded
            .OrderBy(s => s.PTDNumber, new MixedStringComparer()) // Custom comparer for mixed strings
            .ToList();
        responseListNoCheckNeeded = responseListNoCheckNeeded
            .OrderBy(s => s.PTDNumber, new MixedStringComparer()) // Custom comparer for mixed strings
            .ToList();

        //Merge sorted lists
        var mergedList = responseListCheckNeeded.Concat(responseListNoCheckNeeded).ToList();

        return mergedList;
    }

    private static DateTime GetCombinedDateTime(InterimCheckSummary cs)
    {
        return cs.Date?.Add(cs.ScheduledSailingTime ?? TimeSpan.Zero) ?? DateTime.MinValue;
    }

    private async Task<(string status, string travelBy)> GetStatusAndTravelMethod(InterimCheckSummary cs, int timeWindowInHours, DateTime combinedDateTime)
    {

        var check = await _dbContext.CheckOutcome
            .Where(co => co.Id == cs.CheckOutcomeId)
            .Select(co => new { co.PassengerTypeId })
            .FirstOrDefaultAsync();

        var status = "";
        var travelBy = "";

        if (cs.LinkedCheckId == null)
        {
            status = GetStatusForMissingLinkedCheckId(combinedDateTime, timeWindowInHours);
        }
        else
        {
            status = await GetStatusForLinkedCheckId(cs);
        }

        if (check != null) 
        {
            travelBy = GetTravelMethod(check!.PassengerTypeId);
        }

        return (status, travelBy);
    }

    private static string GetStatusForMissingLinkedCheckId(DateTime combinedDateTime, int timeWindowInHours)
    {
        TimeSpan timeSinceSailing = DateTime.UtcNow - combinedDateTime;
        if (timeSinceSailing.TotalHours > timeWindowInHours)
        {
            return (""); 
        }

        return ("Check needed");
    }

    private async Task<string> GetStatusForLinkedCheckId(InterimCheckSummary cs)
    {
        var niCheckSummaryRecord = await _dbContext.CheckSummary
           .Where(co => co.Id == cs.LinkedCheckId)
           .Select(co => new { co.CheckOutcomeId })
           .FirstOrDefaultAsync();

        if (niCheckSummaryRecord == null || niCheckSummaryRecord.CheckOutcomeId == null)
        {
            return ("Check Outcome Pending");
        }

        var niCheck = await _dbContext.CheckOutcome

            .Where(co => co.Id == niCheckSummaryRecord.CheckOutcomeId)
            .Select(co => new { co.SPSOutcome, co.PassengerTypeId })
            .FirstOrDefaultAsync();

        if (niCheck == null)
        {
            return ("Check Outcome Pending");
        }

        var status = niCheck.SPSOutcome == true ? "Allowed" : "Not allowed";

        return (status);
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

    private static string GetPetSpeciesDescription(int? petSpeciesId)
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

    private static string GetEnumDescription(PetSpeciesType speciesType)
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



