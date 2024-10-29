using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Models;
using Defra.PTS.Checker.Services.Implementation;
using Defra.PTS.Checker.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;

namespace Defra.PTS.Checker.Services.Tests.Implementation
{
    [TestFixture]
    public class CheckSummaryServiceTests
    {
        private CheckSummaryService? _service;
        private CommonDbContext? _dbContext;
        private Mock<ILogger<CheckerService>>? _loggerMock;

        [SetUp]
        public void Setup()
        {
            
            var options = new DbContextOptionsBuilder<CommonDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new CommonDbContext(options);

            _loggerMock = new Mock<ILogger<CheckerService>>();
            _service = new CheckSummaryService(_dbContext, _loggerMock.Object);

            
            DataHelper.AddRoutes(_dbContext);
        }

        [Test]
        public async Task SaveCheckSummary_ReturnsIdOnSave_ForPassOutcome()
        {
            // Arrange
            var applicationId = Guid.NewGuid();

            var route = await _dbContext?.Route?.FirstOrDefaultAsync()!;
            if (route == null)
            {
                route = new Route { RouteName = "Test Route" };
                await _dbContext.Route.AddAsync(route);
                await _dbContext.SaveChangesAsync();
            }

            var model = new CheckOutcomeModel
            {
                CheckerId = Guid.NewGuid(),
                CheckOutcome = "Pass",
                ApplicationId = applicationId,
                RouteId = route.Id,
                SailingTime = DateTime.UtcNow,
            };

            var outcome = await _dbContext.Outcome.FirstOrDefaultAsync(o => o.Type == model.CheckOutcome);
            if (outcome == null)
            {
                outcome = new Outcome { Type = model.CheckOutcome };
                await _dbContext.Outcome.AddAsync(outcome);
                await _dbContext.SaveChangesAsync();
            }

            var pet = new Entities.Pet { MicrochipNumber = "1234567890" };
            await _dbContext.Pet.AddAsync(pet);
            await _dbContext.SaveChangesAsync();

            var application = new Entities.Application
            {
                Id = applicationId,
                PetId = pet.Id,
                ReferenceNumber = "REF123",
                Status = "Active"
            };
            await _dbContext.Application.AddAsync(application);

            var travelDocument = new Entities.TravelDocument
            {
                ApplicationId = applicationId,
                PetId = pet.Id,
                DocumentReferenceNumber = "DOC123"
            };
            await _dbContext.TravelDocument.AddAsync(travelDocument);
            await _dbContext.SaveChangesAsync();
            _dbContext.ChangeTracker.Clear();

            // Act
            var result = await _service?.SaveCheckSummary(model)!;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CheckSummaryId, Is.Not.Null);
        }

        [Test]
        public async Task SaveCheckSummary_ReturnsIdOnSave_ForFailOutcome()
        {
            // Arrange
            var applicationId = Guid.NewGuid();

            var route = await _dbContext?.Route?.FirstOrDefaultAsync()!;
            if (route == null)
            {
                route = new Route { RouteName = "Test Route" };
                await _dbContext.Route.AddAsync(route);
                await _dbContext.SaveChangesAsync();
            }

            var model = new NonComplianceModel
            {
                CheckerId = Guid.NewGuid(),
                CheckOutcome = "Fail",
                ApplicationId = applicationId,
                RouteId = route.Id,
                SailingTime = DateTime.UtcNow,
                MCNotMatch = true,
                VCNotMatchPTD = true,
                RelevantComments = "Comments on the failure",
                FlightNumber = "AB1234",
            };

            var outcome = await _dbContext.Outcome.FirstOrDefaultAsync(o => o.Type == model.CheckOutcome);
            if (outcome == null)
            {
                outcome = new Outcome { Type = model.CheckOutcome };
                await _dbContext.Outcome.AddAsync(outcome);
                await _dbContext.SaveChangesAsync();
            }

            var pet = new Entities.Pet { MicrochipNumber = "1234567890" };
            await _dbContext.Pet.AddAsync(pet);
            await _dbContext.SaveChangesAsync();

            var application = new Entities.Application
            {
                Id = applicationId,
                PetId = pet.Id,
                ReferenceNumber = "REF123",
                Status = "Active"
            };
            await _dbContext.Application.AddAsync(application);

            var travelDocument = new Entities.TravelDocument
            {
                ApplicationId = applicationId,
                PetId = pet.Id,
                DocumentReferenceNumber = "DOC123"
            };
            await _dbContext.TravelDocument.AddAsync(travelDocument);
            await _dbContext.SaveChangesAsync();
            _dbContext.ChangeTracker.Clear();

            // Act
            var result = await _service?.SaveCheckSummary(model)!;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CheckSummaryId, Is.Not.Null);
        }


        [Test]
        public async Task SaveCheckSummary_ThrowsException_WhenTravelDocumentIsNull()
        {
            // Arrange            
            var route = await _dbContext?.Route?.FirstOrDefaultAsync()!;
            if (route == null)
            {
                route = new Route { RouteName = "Test Route" };
                await _dbContext.Route.AddAsync(route);
                await _dbContext.SaveChangesAsync();
            }

            var model = new CheckOutcomeModel
            {
                CheckerId = Guid.NewGuid(),
                CheckOutcome = "Fail",
                ApplicationId = Guid.NewGuid(), // Use a new GUID to ensure no matching TravelDocument
                RouteId = route.Id,
                SailingTime = DateTime.UtcNow,
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await _service?.SaveCheckSummary(model)!);
            Assert.That(ex?.Message, Does.Contain("Value cannot be null"), "Exception message should indicate a null value.");
        }

        
        [Ignore("Test wokring locally but failing on azure")]
        public async Task GetRecentCheckOutcomesAsync_ReturnsGroupedResults()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(-10);
            var endDate = DateTime.UtcNow;

            // Retrieve or add Route entities
            var route1 = await _dbContext?.Route?.FirstOrDefaultAsync(r => r.RouteName == "Route1")!;
            if (route1 == null)
            {
                route1 = new Route { RouteName = "Route1" };
                await _dbContext.Route.AddAsync(route1);
                await _dbContext.SaveChangesAsync();
            }

            var route2 = await _dbContext.Route.FirstOrDefaultAsync(r => r.RouteName == "Route2");
            if (route2 == null)
            {
                route2 = new Route { RouteName = "Route2" };
                await _dbContext.Route.AddAsync(route2);
                await _dbContext.SaveChangesAsync();
            }

            // Set up CheckSummary data
            var checkSummaryList = new List<CheckSummary>
            {
                new CheckSummary
                {
                    Date = DateTime.UtcNow.AddDays(-5),
                    ScheduledSailingTime = TimeSpan.FromHours(2),
                    CheckOutcome = true,
                    RouteId = route1.Id
                },
                new CheckSummary
                {
                    Date = DateTime.UtcNow.AddDays(-3),
                    ScheduledSailingTime = TimeSpan.FromHours(3),
                    CheckOutcome = false,
                    RouteId = route2.Id
                }
            };

            await _dbContext.CheckSummary.AddRangeAsync(checkSummaryList);
            await _dbContext.SaveChangesAsync();
            _dbContext.ChangeTracker.Clear();

            // Act
            var result = await _service?.GetRecentCheckOutcomesAsync(startDate, endDate)!;

            // Assert
            Assert.That(result, Is.Not.Null, "The result should not be null.");
            Assert.That(result.Count(), Is.EqualTo(2), $"The result should contain two grouped items but contains {result.Count()}.");
        }
    }

    public static class DataHelper
    {
        public static void AddRoutes(CommonDbContext context)
        {
            if (!context.Route.Any(r => r.RouteName == "Route1"))
            {
                context.Route.Add(new Route { RouteName = "Route1" });
            }
            if (!context.Route.Any(r => r.RouteName == "Route2"))
            {
                context.Route.Add(new Route { RouteName = "Route2" });
            }
            context.SaveChanges();
        }

        
    }

}
