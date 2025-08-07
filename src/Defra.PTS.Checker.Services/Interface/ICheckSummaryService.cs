using Defra.PTS.Checker.Models;

namespace Defra.PTS.Checker.Services.Interface;

public interface ICheckSummaryService
{
    Task<CheckOutcomeResponseModel> SaveCheckSummary(CheckOutcomeModel checkOutcomeModel);
    Task<IEnumerable<CheckOutcomeResponse>> GetRecentCheckOutcomesAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<SpsCheckDetailResponseModel>> GetSpsCheckDetailsByRouteAsync(string route, DateTime sailingDate, int timeWindowInHours);
    Task<GbCheckReportResponseModel?> GetGbCheckReport(Guid gbCheckSummaryId);
    Task<CompleteCheckDetailsResponse?> GetCompleteCheckDetailsAsync(Guid checkSummaryId);
    Task UpdateCheckOutcomeSps(CheckOutcomeRequest checkOutcomeRequest);
}
