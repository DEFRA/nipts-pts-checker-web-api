using Defra.PTS.Checker.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using entity = Defra.PTS.Checker.Entities;

namespace Defra.PTS.Checker.Repositories
{
    [ExcludeFromCodeCoverage]
    public class CommonDbContext : DbContext
    {
        public CommonDbContext(DbContextOptions<CommonDbContext> options) : base(options)
        {

        }

        public DbSet<entity.User> User { get; set; } = null!;
        public DbSet<entity.Owner> Owner { get; set; } = null!;
        public DbSet<entity.Address> Address { get; set; } = null!;
        public DbSet<entity.Application> Application { get; set; } = null!;
        public DbSet<entity.Pet> Pet { get; set; } = null!;
        public DbSet<entity.Breed> Breed { get; set; } = null!;
        public DbSet<entity.Colour> Colour { get; set; } = null!;
        public DbSet<entity.TravelDocument> TravelDocument { get; set; } = null!;
        public DbSet<entity.Operator> Operator { get; set; } = null!;
        public DbSet<entity.Port> Port { get; set; } = null!;
        public DbSet<entity.Route> Route { get; set; } = null!;
        public DbSet<entity.Role> Role { get; set; } = null!;
        public DbSet<entity.Checker> Checker { get; set; } = null!;
        public DbSet<entity.Outcome> Outcome { get; set; } = null!;
        public DbSet<entity.PasengerType> PasengerType { get; set; } = null!;
        public DbSet<entity.CheckOutcome> CheckOutcome { get; set; } = null!;
        public DbSet<entity.CheckSummary> CheckSummary { get; set; } = null!;
        public DbSet<entity.VwApplication> VwApplications { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Application>()
                .HasOne(a => a.Pet)
                .WithMany()
                .HasForeignKey(a => a.PetId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Application>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Application>()
                .HasOne(a => a.Owner)
                .WithMany()
                .HasForeignKey(a => a.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Application>()
                .HasOne(a => a.OwnerAddress)
                .WithMany()
                .HasForeignKey(a => a.OwnerAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TravelDocument>()
               .HasOne(td => td.Application)
               .WithMany()
               .HasForeignKey(td => td.ApplicationId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TravelDocument>()
                .HasOne(td => td.Pet)
                .WithMany()
                .HasForeignKey(td => td.PetId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TravelDocument>()
                .HasOne(td => td.Owner)
                .WithMany()
                .HasForeignKey(td => td.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Route>()
              .HasOne(r => r.DeparturePortNavigation)
              .WithMany(p => p.DepartureRoutes)
              .HasForeignKey(r => r.DeparturePortId)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Route>()
                .HasOne(r => r.ArrivalPortNavigation)
                .WithMany(p => p.ArrivalRoutes)
                .HasForeignKey(r => r.ArrivalPortId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Route>()
                .HasOne(r => r.OperatorNavigation)
                .WithMany(o => o.Routes)
                .HasForeignKey(r => r.OperatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<entity.Checker>()
              .HasOne(c => c.RoleNavigation)
              .WithMany(r => r.Checkers)
              .HasForeignKey(c => c.RoleId)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CheckOutcome>()
                .HasOne(co => co.PassengerTypeNavigation)
                .WithMany()
                .HasForeignKey(co => co.PassengerTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CheckSummary>()
              .HasOne(cs => cs.CheckOutcomeEntity)
              .WithMany()
              .HasForeignKey(cs => cs.CheckOutcomeId)
              .OnDelete(DeleteBehavior.Restrict);         

            modelBuilder.Entity<CheckSummary>()
                .HasOne(cs => cs.Application)
                .WithMany()
                .HasForeignKey(cs => cs.ApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CheckSummary>()
                .HasOne(cs => cs.TravelDocument)
                .WithMany()
                .HasForeignKey(cs => cs.TravelDocumentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CheckSummary>()
                .HasOne(cs => cs.RouteNavigation)
                .WithMany()
                .HasForeignKey(cs => cs.RouteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CheckSummary>()
                .HasOne(cs => cs.Checker)
                .WithMany()
                .HasForeignKey(cs => cs.CheckerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CheckSummary>()
                .HasOne(cs => cs.LinkedCheck)
                .WithMany()
                .HasForeignKey(cs => cs.LinkedCheckId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}