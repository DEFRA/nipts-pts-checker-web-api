using Defra.PTS.Checker.Models;

namespace Defra.PTS.Checker.Services.Interface
{
    public interface ICheckerService
    {
        Task<Guid> SaveChecker(CheckerDto checkerDto);

        Task<object?> CheckMicrochipNumberAsync(string microchipNumber);

        Task<bool> CheckerMicrochipNumberExistWithPtd(string microchipNumber);
    }
}