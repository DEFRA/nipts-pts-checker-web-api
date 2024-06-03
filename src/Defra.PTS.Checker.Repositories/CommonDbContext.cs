using Defra.PTS.Checker.Entities;
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
        public DbSet<entity.Role> Role { get; set; }
        public DbSet<entity.Checker> Checker { get; set; }
        public DbSet<entity.Outcome> Outcome { get; set; }
        public DbSet<entity.PasengerType> PasengerType { get; set; }
        public DbSet<entity.CheckOutcome> CheckOutcome { get; set; }
        public DbSet<entity.CheckSummary> CheckSummary { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
              .HasOne(co => co.OutcomeNavigation)
              .WithMany()
              .HasForeignKey(co => co.Outcome)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CheckOutcome>()
                .HasOne(co => co.ODTypeNavigation)
                .WithMany()
                .HasForeignKey(co => co.ODType)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CheckOutcome>()
                .HasOne(co => co.VCFailBreedActualNavigation)
                .WithMany()
                .HasForeignKey(co => co.VCFailBreedActual)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CheckOutcome>()
                .HasOne(co => co.PDBreedNavigation)
                .WithMany()
                .HasForeignKey(co => co.PDBreed)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CheckOutcome>()
                .HasOne(co => co.VCFailColourActualNavigation)
                .WithMany()
                .HasForeignKey(co => co.VCFailColourActual)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CheckOutcome>()
                .HasOne(co => co.PDColourNavigation)
                .WithMany()
                .HasForeignKey(co => co.PDColour)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CheckSummary>()
              .HasOne(cs => cs.CheckOutcome)
              .WithMany()
              .HasForeignKey(cs => cs.CheckOutcomeId)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CheckSummary>()
                .HasOne(cs => cs.Owner)
                .WithMany()
                .HasForeignKey(cs => cs.OwnerId)
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