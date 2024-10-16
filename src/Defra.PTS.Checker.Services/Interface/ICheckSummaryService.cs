﻿using Defra.PTS.Checker.Models;

namespace Defra.PTS.Checker.Services.Interface;

public interface ICheckSummaryService
{
    Task<CheckOutcomeResponseModel> SaveCheckSummary(CheckOutcomeModel checkOutcomeModel);
    Task<IEnumerable<CheckOutcomeResponse>> GetRecentCheckOutcomesAsync(DateTime startDate, DateTime endDate);
}
