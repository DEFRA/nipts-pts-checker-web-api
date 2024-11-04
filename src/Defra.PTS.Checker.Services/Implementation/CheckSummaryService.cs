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

        var checkSummaryEntity = new CheckSummary
        {
            ApplicationId = travelDocument.ApplicationId,
            CheckerId = checkOutcomeModel.CheckerId,
            CheckOutcome = outcome.Type == "Pass",
            ChipNumber = travelDocument.Application?.Pet?.MicrochipNumber,
            TravelDocumentId = travelDocument.Id,
            GBCheck = checkOutcomeModel.IsGBCheck,
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
        try
        {
            _logger.LogInformation("GetRecentCheckOutcomesAsync startDate", startDate);
            _logger.LogInformation("GetRecentCheckOutcomesAsync endDate", endDate);
            var checkOutcomes = await GetCheckOutcomesAsync(startDate, endDate);

            var results = checkOutcomes.Select(co => new CheckOutcomeResponse
            {
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
            .GroupBy(x => new { x.RouteName, x.DepartureDate, x.DepartureTime, x.CombinedDateTime })
            .Select(group => new CheckOutcomeResponse
            {
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

    public async Task<IEnumerable<SpsCheckDetailResponseModel>> GetSpsCheckDetailsByRouteAsync(
    string route, DateTime sailingDate, int timeWindowInHours)
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
        var checkSummaries = await _dbContext.CheckSummary
            .Where(cs => cs.RouteId == routeId
                         && cs.Date == sailingDateOnly
                         && cs.ScheduledSailingTime == sailingTimeOnly
                         && cs.GBCheck == true
                         && cs.CheckOutcome == false)
            .Select(cs => new
            {
                cs.Date,
                cs.ScheduledSailingTime,
                cs.LinkedCheckId,
                cs.CheckOutcomeId,
                DocumentReferenceNumber = cs.TravelDocument != null ? cs.TravelDocument.DocumentReferenceNumber : null,
                PetSpeciesId = cs.TravelDocument != null && cs.TravelDocument.Pet != null ? cs.TravelDocument.Pet.SpeciesId : (int?)null,
                PetColourName = cs.TravelDocument != null && cs.TravelDocument.Pet != null && cs.TravelDocument.Pet.Colour != null ? cs.TravelDocument.Pet.Colour.Name : null,
                PetOtherColour = cs.TravelDocument != null && cs.TravelDocument.Pet != null ? cs.TravelDocument.Pet.OtherColour : null,
                MicrochipNumber = cs.TravelDocument != null && cs.TravelDocument.Pet != null ? cs.TravelDocument.Pet.MicrochipNumber : null
            })
            .ToListAsync();

        var responseList = new List<SpsCheckDetailResponseModel>();

        foreach (var cs in checkSummaries)
        {
            // Combine Date and ScheduledSailingTime using Value property
            var combinedDateTime = cs.Date?.Add(cs.ScheduledSailingTime ?? TimeSpan.Zero) ?? DateTime.MinValue;


            // Determine the status based on LinkedCheckId and other conditions
            string status;
            string travelBy = "";

            if (cs.LinkedCheckId == null)
            {
                var timeSinceSailing = DateTime.Now - combinedDateTime;
                if (timeSinceSailing.TotalHours > timeWindowInHours)
                {
                    continue; // Skip "Did Not Attend" cases
                }

                status = "Check Needed";
            }
            else
            {
                var niCheck = await _dbContext.CheckOutcome
                     .Where(co => co.Id == cs.CheckOutcomeId)
                     .Select(co => new { co.SPSOutcome, co.PassengerTypeId })
                     .FirstOrDefaultAsync();

                if (niCheck == null)
                {
                    status = "Check Outcome Pending";
                }
                else
                {
                    travelBy = niCheck.PassengerTypeId switch
                    {
                        1 => "Foot",
                        2 => "Vehicle",
                        _ => ""
                    };

                    status = niCheck.SPSOutcome == true ? "Allowed" : "Not allowed";
                }
            }

            // Get species description from PetSpeciesType enum
            var petSpeciesDescription = GetEnumDescription((PetSpeciesType)(cs.PetSpeciesId ?? 0));

            // Get Colour name or use OtherColour as fallback if Colour is null
            var colourDescription = cs.PetColourName ?? cs.PetOtherColour ?? "";

            // Populate response model with relevant details, defaulting nulls to empty strings
            var responseItem = new SpsCheckDetailResponseModel
            {
                PTDNumber = cs.DocumentReferenceNumber ?? "",
                PetDescription = $"{petSpeciesDescription}, {colourDescription}",
                Microchip = cs.MicrochipNumber ?? "",
                TravelBy = travelBy,
                SPSOutcome = status
            };

            // Add to response list if not already present
            if (!responseList.Contains(responseItem))
            {
                responseList.Add(responseItem);
            }
        }

        return responseList;
    }

    
    private string GetEnumDescription(PetSpeciesType speciesType)
    {
        var field = speciesType.GetType().GetField(speciesType.ToString());
        var attribute = field?.GetCustomAttributes(typeof(DescriptionAttribute), false)
                              .Cast<DescriptionAttribute>()
                              .FirstOrDefault();
        return attribute?.Description ?? speciesType.ToString();
    }


}



