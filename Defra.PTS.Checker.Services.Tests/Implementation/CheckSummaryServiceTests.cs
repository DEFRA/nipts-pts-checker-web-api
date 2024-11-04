

using Defra.PTS.Checker.Entities;
using Defra.PTS.Checker.Models;
using Defra.PTS.Checker.Services.Implementation;
using Defra.PTS.Checker.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using Defra.PTS.Checker.Models.Enums;

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
        public async Task SaveCheckSummary_ForFlight_ReturnsIdOnSave_ForPassOutcome()
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
                SailingOption = (int)SailingOption.Flight
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
        public async Task SaveCheckSummary_ForFerry_ReturnsIdOnSave_ForPassOutcome()
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
                SailingOption = (int)SailingOption.Ferry
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
                SailingOption = (int)SailingOption.Ferry
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await _service?.SaveCheckSummary(model)!);
            Assert.That(ex?.Message, Does.Contain("Value cannot be null"), "Exception message should indicate a null value.");
        }

        [Test]
        public async Task GetCheckOutcomesAsync_ReturnsGroupedResults()
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
            var result = await _service?.GetCheckOutcomesAsync(startDate, endDate)!;

            // Assert
            Assert.That(result, Is.Not.Null, "The result should not be null.");
            Assert.That(result.Count(), Is.EqualTo(2), $"The result should contain two grouped items but contains {result.Count()}.");
        }


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


        [Test]
        public async Task GetSpsCheckDetailsByRouteAsync_ReturnsCheckNeeded_WhenNoLinkedCheck()
        {
            // Arrange
            var specificDate = DateTime.Today.AddDays(-1); // Yesterday's date at midnight
            var scheduledSailingTime = TimeSpan.Zero; // 00:00:00

            var route = new Entities.Route { RouteName = "Route1" };

            if (_dbContext != null)
            {
                await _dbContext.Route.AddAsync(route);
                await _dbContext.SaveChangesAsync();

                var colour = new Entities.Colour { Name = "Brown", SpeciesId = 1 };
                await _dbContext.Colour.AddAsync(colour);
                await _dbContext.SaveChangesAsync();

                var pet = new Entities.Pet
                {
                    MicrochipNumber = "1234567890",
                    SpeciesId = 1,
                    ColourId = colour.Id
                };
                await _dbContext.Pet.AddAsync(pet);
                await _dbContext.SaveChangesAsync();

                var travelDocument = new Entities.TravelDocument
                {
                    DocumentReferenceNumber = "PTD001",
                    PetId = pet.Id
                };
                await _dbContext.TravelDocument.AddAsync(travelDocument);
                await _dbContext.SaveChangesAsync();

                // No CheckOutcome needed since LinkedCheckId is null

                var checkSummary = new Entities.CheckSummary
                {
                    RouteId = 1,
                    TravelDocumentId = travelDocument.Id,
                    Date = specificDate.Date,
                    ScheduledSailingTime = scheduledSailingTime,
                    GBCheck = true,
                    CheckOutcome = false,
                    LinkedCheckId = null, // No linked check
                    CheckOutcomeId = null // No outcome
                };
                await _dbContext.CheckSummary.AddAsync(checkSummary);
                await _dbContext.SaveChangesAsync();

                _dbContext.ChangeTracker.Clear();
            }

            // Act
            var timeWindowInHours = 48;
            if (_service != null)
            {
                var result = await _service.GetSpsCheckDetailsByRouteAsync("Route1", specificDate, timeWindowInHours);

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count(), Is.EqualTo(1));
                Assert.That(result.First().SPSOutcome, Is.EqualTo("Check Needed"));
            }
        }

        [Test]
        public async Task GetSpsCheckDetailsByRouteAsync_ReturnsAllowed_WhenLinkedCheckHasAllowedOutcome()
        {
            // Arrange
            var specificDate = DateTime.Today.AddDays(-1);
            var scheduledSailingTime = TimeSpan.Zero;

            var route = new Entities.Route { RouteName = "Route1" };

            if (_dbContext != null)
            {
                await _dbContext.Route.AddAsync(route);
                await _dbContext.SaveChangesAsync();

                var colour = new Entities.Colour { Name = "Black", SpeciesId = 1 };
                await _dbContext.Colour.AddAsync(colour);
                await _dbContext.SaveChangesAsync();

                var pet = new Entities.Pet
                {
                    MicrochipNumber = "1234567890",
                    SpeciesId = 1,
                    ColourId = colour.Id
                };
                await _dbContext.Pet.AddAsync(pet);
                await _dbContext.SaveChangesAsync();

                var travelDocument = new Entities.TravelDocument
                {
                    DocumentReferenceNumber = "PTD002",
                    PetId = pet.Id
                };
                await _dbContext.TravelDocument.AddAsync(travelDocument);
                await _dbContext.SaveChangesAsync();

                var checkOutcome = new Entities.CheckOutcome
                {
                    SPSOutcome = true, // Allowed
                    PassengerTypeId = 1 // Foot
                };
                await _dbContext.CheckOutcome.AddAsync(checkOutcome);
                await _dbContext.SaveChangesAsync();

                var checkSummary = new Entities.CheckSummary
                {
                    RouteId = 1,
                    TravelDocumentId = travelDocument.Id,
                    Date = specificDate.Date,
                    ScheduledSailingTime = scheduledSailingTime,
                    GBCheck = true,
                    CheckOutcome = false,
                    LinkedCheckId = Guid.NewGuid(), // Non-null linked check
                    CheckOutcomeId = checkOutcome.Id
                };
                await _dbContext.CheckSummary.AddAsync(checkSummary);
                await _dbContext.SaveChangesAsync();

                _dbContext.ChangeTracker.Clear();
            }

            // Act
            var timeWindowInHours = 48;
            if (_service != null)
            {
                var result = await _service.GetSpsCheckDetailsByRouteAsync("Route1", specificDate, timeWindowInHours);

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count(), Is.EqualTo(1));
                Assert.That(result.First().SPSOutcome, Is.EqualTo("Allowed"));
                Assert.That(result.First().TravelBy, Is.EqualTo("Foot"));
            }
        }

        [Test]
        public async Task GetSpsCheckDetailsByRouteAsync_ReturnsNotAllowed_WhenLinkedCheckHasNotAllowedOutcome()
        {
            // Arrange
            var specificDate = DateTime.Today.AddDays(-1);
            var scheduledSailingTime = TimeSpan.Zero;

            var route = new Entities.Route { RouteName = "Route1" };

            if (_dbContext != null)
            {
                await _dbContext.Route.AddAsync(route);
                await _dbContext.SaveChangesAsync();

                var colour = new Entities.Colour { Name = "White", SpeciesId = 2 };
                await _dbContext.Colour.AddAsync(colour);
                await _dbContext.SaveChangesAsync();

                var pet = new Entities.Pet
                {
                    MicrochipNumber = "9876543210",
                    SpeciesId = 2,
                    ColourId = colour.Id
                };
                await _dbContext.Pet.AddAsync(pet);
                await _dbContext.SaveChangesAsync();

                var travelDocument = new Entities.TravelDocument
                {
                    DocumentReferenceNumber = "PTD003",
                    PetId = pet.Id
                };
                await _dbContext.TravelDocument.AddAsync(travelDocument);
                await _dbContext.SaveChangesAsync();

                var checkOutcome = new Entities.CheckOutcome
                {
                    SPSOutcome = false, // Not allowed
                    PassengerTypeId = 2 // Vehicle
                };
                await _dbContext.CheckOutcome.AddAsync(checkOutcome);
                await _dbContext.SaveChangesAsync();

                var checkSummary = new Entities.CheckSummary
                {
                    RouteId = 1,
                    TravelDocumentId = travelDocument.Id,
                    Date = specificDate.Date,
                    ScheduledSailingTime = scheduledSailingTime,
                    GBCheck = true,
                    CheckOutcome = false,
                    LinkedCheckId = Guid.NewGuid(), // Non-null linked check
                    CheckOutcomeId = checkOutcome.Id
                };
                await _dbContext.CheckSummary.AddAsync(checkSummary);
                await _dbContext.SaveChangesAsync();

                _dbContext.ChangeTracker.Clear();
            }

            // Act
            var timeWindowInHours = 48;
            if (_service != null)
            {
                var result = await _service.GetSpsCheckDetailsByRouteAsync("Route1", specificDate, timeWindowInHours);

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count(), Is.EqualTo(1));
                Assert.That(result.First().SPSOutcome, Is.EqualTo("Not allowed"));
                Assert.That(result.First().TravelBy, Is.EqualTo("Vehicle"));
            }
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
