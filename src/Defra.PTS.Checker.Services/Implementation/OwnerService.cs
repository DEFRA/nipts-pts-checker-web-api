using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Repositories.Interface;
using Defra.PTS.Checker.Services.Interface;
using Microsoft.Extensions.Logging;
using modelUser = Defra.PTS.Checker.Models;

namespace Defra.PTS.Checker.Services.Implementation
{
    public class OwnerService : IOwnerService
    {
        private readonly IRepository<Owner> _ownerRepository;
        private ILogger<OwnerService> _log;
        public OwnerService(ILogger<OwnerService> log, IRepository<Owner> ownerRepository)
        {
            _ownerRepository = ownerRepository;
        }

        public void CreateOwner(modelUser.Owner owner)
        {
            var ownerDb = new Owner()
            {
                FullName = owner.FullName,
                Email = owner.Email,
                Telephone = owner.Telephone,
            };

            _ownerRepository.Add(ownerDb);
            _ownerRepository.SaveChanges();
        }
    }
}
