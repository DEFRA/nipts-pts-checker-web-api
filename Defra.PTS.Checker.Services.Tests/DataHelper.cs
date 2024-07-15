using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Defra.PTS.Checker.Services.Tests;

public static class DataHelper
{
    public static CommonDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<CommonDbContext>()
            .UseInMemoryDatabase(databaseName: "sql_db")
            .Options;

        var context = new CommonDbContext(options);

        AddRoutes(context);
        AddOutcomes(context);
        AddTravelDocument(context);

        return context;
    }

    private static void AddRoutes(CommonDbContext context)
    {
        // Port
        context.Port.Add(new Port { Id = 1, PortName = "Port 1", PortLocation = "Test Location" });
        context.Port.Add(new Port { Id = 2, PortName = "Port 2", PortLocation = "Test Location" });
        context.Port.Add(new Port { Id = 3, PortName = "Port 3", PortLocation = "Test Location" });
        context.Port.Add(new Port { Id = 4, PortName = "Port 4", PortLocation = "Test Location" });

        // Operator
        context.Operator.Add(new Operator { Id = 1, OperatorName = "Operator 1" });
        context.Operator.Add(new Operator { Id = 2, OperatorName = "Operator 2" });

        // Route
        context.Route.Add(new Route { Id = 1, RouteName = "Route 1", ArrivalPortId = 1, DeparturePortId = 1, OperatorId = 1 });
        context.Route.Add(new Route { Id = 2, RouteName = "Route 2", ArrivalPortId = 2, DeparturePortId = 2, OperatorId = 2 });

        context.SaveChanges();
    }

    private static void AddOutcomes(CommonDbContext context)
    {
        context.Outcome.Add(new Outcome { Id = 1, Type = "Pass" });
        context.Outcome.Add(new Outcome { Id = 2, Type = "Fail" });
        context.SaveChanges();
    }

    private static void AddTravelDocument(CommonDbContext context)
    {
        // user
        var user = new User
        {
            Id = Guid.NewGuid(),
        };

        // pet
        var pet = new Pet
        {
            Id = Guid.NewGuid(),
            Name = "Test Pet",
            SpeciesId = 1,
            SexId = 1,
            ColourId = 1,
            MicrochipNumber = "123456789012345",
            HasUniqueFeature = 0,
            IdentificationType = 1,
            IsDateOfBirthKnown = 0,
        };

        // owner
        var owner = new Owner
        {
            Id = Guid.NewGuid(),
            Email = "Test@test.com",
            FullName = "Test Owner",
        };

        // application
        var application = new Application
        {
            Id = new Guid("FF0DF803-8033-4CF8-B877-AB69BEFE63D2"),
            DateAuthorised = DateTime.Now,
            DateOfApplication = DateTime.Now,
            CreatedOn = DateTime.Now,
            DynamicId = Guid.NewGuid(),
            OwnerId = owner.Id,
            Status = "Authorised", // Rejected,AWAITING VERIFICATION,Authorised
            PetId = pet.Id,
            UserId = user.Id,
            ReferenceNumber = "TestRef",
            Owner = owner,
            Pet = pet,
            User = user,
        };

        // travel document
        var travelDocument = new TravelDocument
        {
            Id = Guid.NewGuid(),
            DateOfIssue = DateTime.Now,
            DocumentReferenceNumber = "GB826CD186E",
            DocumentSignedBy = "Test User",

            ApplicationId = application.Id,
            Application = application,
            OwnerId = owner.Id,
            Owner = owner,
            PetId = pet.Id,
            Pet = pet,
        };

        context.User.Add(user);
        context.Pet.Add(pet);
        context.Owner.Add(owner);
        context.Application.Add(application);
        context.TravelDocument.Add(travelDocument);

        context.SaveChanges();
    }
}
