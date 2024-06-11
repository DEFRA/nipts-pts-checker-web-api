namespace Defra.PTS.Checker.Services.Interface
{
    public interface ICheckerService
    {
        Task<object?> CheckMicrochipNumberAsync(string microchipNumber);
    }
}