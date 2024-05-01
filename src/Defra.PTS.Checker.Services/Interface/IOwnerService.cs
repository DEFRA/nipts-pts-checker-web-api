using Defra.PTS.Checker.Entities;
using modelUser = Defra.PTS.Checker.Models;

namespace Defra.PTS.Checker.Services.Interface
{
    public interface IOwnerService
    {
        void CreateOwner(modelUser.Owner owner);
    }
}
