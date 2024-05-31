using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using entity = Defra.PTS.Checker.Entities;

namespace  Defra.PTS.Checker.Repositories
{
    [ExcludeFromCodeCoverage]
    public class CommonDbContext : DbContext
    {
        public CommonDbContext(DbContextOptions<CommonDbContext> options) : base(options)
        {
                
        }

        public DbSet<entity.User> User { get; set; }
        public DbSet<entity.Owner> Owner { get; set; }
        public DbSet<entity.Address> Address { get; set; }
        public DbSet<entity.Application> Application { get; set; }
        public DbSet<entity.Pet> Pet { get; set; }
        public DbSet<entity.Breed> Breed { get; set; }
        public DbSet<entity.Color> Color { get; set; }
        public DbSet<entity.TravelDocument> TravelDocument { get; set; }
        public DbSet<entity.Operator> Operator { get; set; }
        public DbSet<entity.Port> Port { get; set; }
        public DbSet<entity.Route> Route { get; set; }
    }
}