

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
        private Mock<ILogger<CheckSummaryService>>? _loggerMock;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CommonDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new CommonDbContext(options);
            _loggerMock = new Mock<ILogger<CheckSummaryService>>();
            _service = new CheckSummaryService(_dbContext, _loggerMock.Object);

            DataHelper.AddRoutes(_dbContext);
            DataHelper.DetachTrackedEntities(_dbContext); 
        }

        [Test]
        public async Task SaveCheckSummary_ForFlight_ReturnsIdOnSave_ForPassOutcome()
        {
            // Arrange
            var applicationId = Guid.NewGuid();

            var route = await _dbContext?.Route?.FirstOrDefaultAsync()!;
            if (route == null)
            {
                route = new Entities.Route { RouteName = "Test Route" };
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
                route = new Entities.Route { RouteName = "Test Route" };
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
        public async Task SaveCheckSummary_IsGbCheck_Ferry_ReturnsIdOnSave_ForFailOutcome()
        {
            // Arrange
            var applicationId = Guid.NewGuid();

            var route = await _dbContext?.Route?.FirstOrDefaultAsync()!;
            if (route == null)
            {
                route = new Entities.Route { RouteName = "Test Route" };
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
                SailingOption = (int)SailingOption.Ferry,
                IsGBCheck = true,
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
        public async Task SaveCheckSummary_IsGbCheck_Flight_ReturnsIdOnSave_ForFailOutcome()
        {
            // Arrange
            var applicationId = Guid.NewGuid();

            var route = await _dbContext?.Route?.FirstOrDefaultAsync()!;
            if (route == null)
            {
                route = new Entities.Route { Id = 1, RouteName = "Test Route" };
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
                FlightNumber = null,
                SailingOption = (int)SailingOption.Flight,
                IsGBCheck = true,
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
        public async Task SaveCheckSummary_IsSPSCheck_Ferry_ReturnsIdOnSave_ForFailOutcome()
        {
            // Arrange
            var applicationId = Guid.NewGuid();

            var route = await _dbContext?.Route?.FirstOrDefaultAsync()!;
            if (route == null)
            {
                route = new Entities.Route { RouteName = "Test Route" };
                await _dbContext.Route.AddAsync(route);
                await _dbContext.SaveChangesAsync();
            }
            Guid gbCheckId = Guid.NewGuid();

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
                FlightNumber = null,
                SailingOption = (int)SailingOption.Ferry,
                IsGBCheck = false,
                GBCheckId = gbCheckId,
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

            var checkSummary = new Entities.CheckSummary
            {
                Id = gbCheckId,
                GBCheck = true,
            };
            await _dbContext.CheckSummary.AddAsync(checkSummary);
            await _dbContext.SaveChangesAsync();

            _dbContext.ChangeTracker.Clear();

            // Act
            var result = await _service?.SaveCheckSummary(model)!;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CheckSummaryId, Is.Not.Null);
        }

        [Test]
        public async Task SaveCheckSummary_IsSPSCheck_Flight_ReturnsIdOnSave_ForFailOutcome()
        {
            // Arrange
            var applicationId = Guid.NewGuid();

            var route = await _dbContext?.Route?.FirstOrDefaultAsync()!;
            if (route == null)
            {
                route = new Entities.Route { RouteName = "Test Route" };
                await _dbContext.Route.AddAsync(route);
                await _dbContext.SaveChangesAsync();
            }
            Guid gbCheckId = Guid.NewGuid();

            var model = new NonComplianceModel
            {
                CheckerId = Guid.NewGuid(),
                CheckOutcome = "Fail",
                ApplicationId = applicationId,
                RouteId = null,
                SailingTime = DateTime.UtcNow,
                MCNotMatch = true,
                VCNotMatchPTD = true,
                RelevantComments = "Comments on the failure",
                FlightNumber = "AB1234",
                SailingOption = (int)SailingOption.Ferry,
                IsGBCheck = false,
                GBCheckId = gbCheckId,
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

            var checkSummary = new Entities.CheckSummary
            {
                Id = gbCheckId,
                GBCheck = true,
            };
            await _dbContext.CheckSummary.AddAsync(checkSummary);
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
                route = new Entities.Route { RouteName = "Test Route" };
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
                route1 = new Entities.Route { RouteName = "Route1" };
                await _dbContext.Route.AddAsync(route1);
                await _dbContext.SaveChangesAsync();
            }

            var route2 = await _dbContext.Route.FirstOrDefaultAsync(r => r.RouteName == "Route2");
            if (route2 == null)
            {
                route2 = new Entities.Route { RouteName = "Route2" };
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
                    RouteId = route1.Id,
                    GBCheck = true,
                },
                new CheckSummary
                {
                    Date = DateTime.UtcNow.AddDays(-3),
                    ScheduledSailingTime = TimeSpan.FromHours(3),
                    CheckOutcome = false,
                    RouteId = route2.Id,
                    GBCheck = true,
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

        [Test]
        public async Task GetRecentCheckOutcomesAsync_ReturnsGroupedResults()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(-10);
            var endDate = DateTime.UtcNow;

            // Retrieve or add Route entities
            var route1 = await _dbContext?.Route?.FirstOrDefaultAsync(r => r.RouteName == "Route1")!;
            if (route1 == null)
            {
                route1 = new Entities.Route { RouteName = "Route1" };
                await _dbContext.Route.AddAsync(route1);
                await _dbContext.SaveChangesAsync();
            }

            var route2 = await _dbContext.Route.FirstOrDefaultAsync(r => r.RouteName == "Route2");
            if (route2 == null)
            {
                route2 = new Entities.Route { RouteName = "Route2" };
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
                    RouteId = route1.Id,
                    GBCheck = true
                    
                },
                new CheckSummary
                {
                    Date = DateTime.UtcNow.AddDays(-3),
                    ScheduledSailingTime = TimeSpan.FromHours(3),
                    CheckOutcome = false,
                    RouteId = route2.Id,
                    GBCheck = true
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

                var guid = new Guid("FF0DF803-8033-4CF8-B877-AB69BEFE63D2");
                var application = new Entities.Application { Id = guid, ReferenceNumber = "test", Status = "approved" };
                var existingApplication = await _dbContext.Application.FirstOrDefaultAsync(a => a.Id == guid);
                if (existingApplication == null)
                {
                    await _dbContext.Application.AddAsync(application);
                    await _dbContext.SaveChangesAsync();
                }

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
                    PetId = pet.Id,
                    Application = application,
                    ApplicationId = guid
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
                    Application = application,
                    ApplicationId = guid,
                    GBCheck = true,
                    CheckOutcome = false,
                    LinkedCheckId = null, // No linked check
                    CheckOutcomeId = null // No outcome,
                   
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

            var routeId = 1;
            var colourId = 1;
            var petId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var travelDocumentId = Guid.Parse("44444444-4444-4444-4444-444444444444");
            var checkOutcomeId = Guid.Parse("55555555-5555-5555-5555-555555555555");
            var niCheckSummaryId = Guid.Parse("66666666-6666-6666-6666-666666666666");
            var checkSummaryId = Guid.Parse("77777777-7777-7777-7777-777777777777");

            var route = new Entities.Route { RouteName = "Route1", Id = routeId };

            if (_dbContext != null)
            {
                var guid = new Guid("FF0DF803-8033-4CF8-B877-AB69BEFE63D2"); 
                var application = new Entities.Application {Id = guid, ReferenceNumber = "test", Status = "approved" };
                var existingApplication = await _dbContext.Application.FirstOrDefaultAsync(a => a.Id == guid);
                if (existingApplication == null)
                {
                    await _dbContext.Application.AddAsync(application);
                    await _dbContext.SaveChangesAsync();
                }


                var existingRoute = await _dbContext.Route.FirstOrDefaultAsync(r => r.Id == routeId);
                if (existingRoute == null)
                {
                    await _dbContext.Route.AddAsync(route);
                    await _dbContext.SaveChangesAsync();
                }

                var colour = new Entities.Colour { Name = "Black", SpeciesId = 1, Id = colourId };
                var existingColour = await _dbContext.Colour.FirstOrDefaultAsync(c => c.Id == colourId);
                if (existingColour == null)
                {
                    await _dbContext.Colour.AddAsync(colour);
                    await _dbContext.SaveChangesAsync();
                }

                var pet = new Entities.Pet
                {
                    Id = petId,
                    MicrochipNumber = "1234567890",
                    SpeciesId = 1,
                    ColourId = colour.Id
                };
                await _dbContext.Pet.AddAsync(pet);
                await _dbContext.SaveChangesAsync();

                var travelDocument = new Entities.TravelDocument
                {
                    Id = travelDocumentId,
                    DocumentReferenceNumber = "PTD002",
                    PetId = pet.Id,
                    ApplicationId = guid, 
                    Application = application
                };
                await _dbContext.TravelDocument.AddAsync(travelDocument);
                await _dbContext.SaveChangesAsync();

                var checkOutcome = new Entities.CheckOutcome
                {
                    Id = checkOutcomeId,
                    SPSOutcome = true, // Allowed
                    PassengerTypeId = 1 // Foot
                };
                await _dbContext.CheckOutcome.AddAsync(checkOutcome);
                await _dbContext.SaveChangesAsync();

                var niCheckSummary = new Entities.CheckSummary
                {
                    Id = niCheckSummaryId,
                    RouteId = route.Id,
                    TravelDocumentId = travelDocument.Id,
                    Date = specificDate.Date,
                    ScheduledSailingTime = scheduledSailingTime,
                    GBCheck = false,
                    CheckOutcome = true, // Allowed outcome
                    CheckOutcomeId = checkOutcome.Id,
                    Application = application,
                    ApplicationId = guid
                };
                await _dbContext.CheckSummary.AddAsync(niCheckSummary);
                await _dbContext.SaveChangesAsync();

                var checkSummary = new Entities.CheckSummary
                {
                    Id = checkSummaryId,
                    RouteId = route.Id,
                    TravelDocumentId = travelDocument.Id,
                    Date = specificDate.Date,
                    ScheduledSailingTime = scheduledSailingTime,
                    GBCheck = true,
                    CheckOutcome = false,
                    LinkedCheckId = niCheckSummary.Id,
                    CheckOutcomeId = checkOutcome.Id,
                    Application = application,
                    ApplicationId = guid
                };
                await _dbContext.CheckSummary.AddAsync(checkSummary);
                await _dbContext.SaveChangesAsync();

                _dbContext.ChangeTracker.Clear();
                _dbContext.Entry(route).State = EntityState.Detached;
                _dbContext.Entry(colour).State = EntityState.Detached;
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

            var routeId = 3;
            var colourId = 4;
            var petId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaab");
            var travelDocumentId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbba");
            var checkOutcomeId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccd");
            var niCheckSummaryId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddc");
            var checkSummaryId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeef");

            var route = new Entities.Route { RouteName = "Route1", Id = routeId };

            if (_dbContext != null)
            {
                var existingRoute = await _dbContext.Route.FirstOrDefaultAsync(r => r.Id == routeId);
                if (existingRoute == null)
                {
                    await _dbContext.Route.AddAsync(route);
                    await _dbContext.SaveChangesAsync();
                    _dbContext.Entry(route).State = EntityState.Detached; // Detach after save to prevent tracking conflicts
                }

                var colour = new Entities.Colour { Name = "White", SpeciesId = 2, Id = colourId };
                var existingColour = await _dbContext.Colour.FirstOrDefaultAsync(c => c.Id == colourId);
                if (existingColour == null)
                {
                    await _dbContext.Colour.AddAsync(colour);
                    await _dbContext.SaveChangesAsync();
                    _dbContext.Entry(colour).State = EntityState.Detached; // Detach after save to prevent tracking conflicts
                }

                var pet = new Entities.Pet
                {
                    Id = petId,
                    MicrochipNumber = "9876543210",
                    SpeciesId = 2,
                    ColourId = colour.Id
                };
                await _dbContext.Pet.AddAsync(pet);
                await _dbContext.SaveChangesAsync();

                var travelDocument = new Entities.TravelDocument
                {
                    Id = travelDocumentId,
                    DocumentReferenceNumber = "PTD003",
                    PetId = pet.Id
                };
                await _dbContext.TravelDocument.AddAsync(travelDocument);
                await _dbContext.SaveChangesAsync();

                var checkOutcome = new Entities.CheckOutcome
                {
                    Id = checkOutcomeId,
                    SPSOutcome = false, // Not allowed
                    PassengerTypeId = 2 // Vehicle
                };
                await _dbContext.CheckOutcome.AddAsync(checkOutcome);
                await _dbContext.SaveChangesAsync();

                var niCheckSummary = new Entities.CheckSummary
                {
                    Id = niCheckSummaryId,
                    RouteId = route.Id,
                    TravelDocumentId = travelDocument.Id,
                    Date = specificDate.Date,
                    ScheduledSailingTime = scheduledSailingTime,
                    GBCheck = false,
                    CheckOutcome = false, // Not allowed outcome
                    CheckOutcomeId = checkOutcome.Id
                };
                await _dbContext.CheckSummary.AddAsync(niCheckSummary);
                await _dbContext.SaveChangesAsync();

                var checkSummary = new Entities.CheckSummary
                {
                    Id = checkSummaryId,
                    RouteId = route.Id,
                    TravelDocumentId = travelDocument.Id,
                    Date = specificDate.Date,
                    ScheduledSailingTime = scheduledSailingTime,
                    GBCheck = true,
                    CheckOutcome = false,
                    LinkedCheckId = niCheckSummary.Id,
                    CheckOutcomeId = checkOutcome.Id
                };
                await _dbContext.CheckSummary.AddAsync(checkSummary);
                await _dbContext.SaveChangesAsync();

                _dbContext.ChangeTracker.Clear(); // Clear the tracker to reset the entity states
            }

            // Act
            var timeWindowInHours = 48;
            if (_service != null)
            {
                var result = await _service.GetSpsCheckDetailsByRouteAsync("Route1", specificDate, timeWindowInHours);

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count(), Is.EqualTo(0));
                
            }
        }

        [Test]
        public async Task GetGbReport_When_Report_Null_ReturnsValidObject()
        {
            Guid gbCheckSummaryID = Guid.NewGuid();

            if (_service != null)
            {
                var result = await _service.GetGbCheckReport(gbCheckSummaryID);

                Assert.That(result, Is.Null);
            }

        }

        [Test]
        public async Task GetGbReport_When_Report_IsNotNull_ReturnsValidObject()
        {
            Guid gbCheckSummaryID = Guid.Empty;
            if (_dbContext != null)
            {
                var checker = new Entities.Checker() { FullName = "N G", FirstName = "N", LastName = "G"  };
                await _dbContext.Checker.AddAsync(checker);
                await _dbContext.SaveChangesAsync();


                var checkOutcome = new Entities.CheckOutcome() { };
                await _dbContext.CheckOutcome.AddAsync(checkOutcome);
                await _dbContext.SaveChangesAsync();


                var checkSummary = new Entities.CheckSummary() { CheckerId = checker.Id, CheckOutcome = false, CheckOutcomeId = checkOutcome.Id };

                await _dbContext.CheckSummary.AddAsync(checkSummary);
                await _dbContext.SaveChangesAsync();

                gbCheckSummaryID = checkSummary.Id;
            }

            if (_service != null)
            {
                var result = await _service.GetGbCheckReport(gbCheckSummaryID);

                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<GbCheckReportResponseModel>());
                Assert.That(result?.GbCheckSummaryId, Is.EqualTo(gbCheckSummaryID));
            }
        }









        private void AddCheckerEntity(Guid checkerId, string firstName, string lastName, string fullName)
        {
            var checker = new Entities.Checker
            {
                Id = checkerId,
                FirstName = firstName,
                LastName = lastName,
                FullName = fullName
            };

            _dbContext?.Checker.Add(checker);
            _dbContext?.SaveChanges();
        }

        [Test]
        public async Task GetCompleteCheckDetailsAsync_ValidCheckSummaryId_ReturnsCompleteDetails()
        {
            var checkSummaryId = Guid.NewGuid();
            var applicationId = Guid.NewGuid();

            var pet = new Entities.Pet { Id = Guid.NewGuid(), MicrochipNumber = "1234567890" };
            var application = new Entities.Application
            {
                Id = applicationId,
                Pet = pet,
                ReferenceNumber = "REF123",
                Status = "Active"
            };

            var route = new Entities.Route { Id = 3, RouteName = "Test Route" };

            if (_dbContext == null)
            {
                throw new InvalidOperationException("Database context is not initialized.");
            }

            var existingRoute = _dbContext.Route.SingleOrDefaultAsync(r => r.Id == route.Id);
            if (existingRoute == null)
            {
                _dbContext.Route.Add(route);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                DetachTrackedEntities();
            }

            var checkerId = Guid.NewGuid();
            AddCheckerEntity(checkerId, "John", "Doe", "John Doe");

            var checkOutcome = new Entities.CheckOutcome
            {
                Id = Guid.NewGuid(),
                MCNotMatch = true,
                MCNotMatchActual = "9876543210",
                RelevantComments = "Relevant comment",
                SPSOutcomeDetails = "Additional comment",
                OIFailOther = true,
                GBRefersToDAERAOrSPS = true, 
                GBAdviseNoTravel = true,
                GBPassengerSaysNoTravel = true
            };

            var checkSummary = new CheckSummary
            {
                Id = checkSummaryId,
                Application = application,
                CheckerId = checkerId,
                RouteNavigation = route,
                UpdatedOn = DateTime.UtcNow,
                Date = DateTime.UtcNow.Date,
                ScheduledSailingTime = new TimeSpan(10, 30, 0),
                CheckOutcomeId = checkOutcome.Id,
                CheckOutcomeEntity = checkOutcome,
                ChipNumber = "1234567890"
            };

            await _dbContext.Pet.AddAsync(pet);
            await _dbContext.Application.AddAsync(application);
            await _dbContext.CheckOutcome.AddAsync(checkOutcome);
            await _dbContext.CheckSummary.AddAsync(checkSummary);
            await _dbContext.SaveChangesAsync();

            if (_service == null)
            {
                throw new InvalidOperationException("Service is not initialized.");
            }

            var result = await _service.GetCompleteCheckDetailsAsync(checkSummaryId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.MicrochipNumber, Is.EqualTo("1234567890"));
            Assert.That(result.CheckOutcome, Contains.Item("Passenger referred to DAERA/SPS at NI port"));
            Assert.That(result.ReasonForReferral, Contains.Item("Microchip number does not match the PTD"));
            Assert.That(result.GBCheckerName, Is.EqualTo("John Doe"));
            Assert.That(result.Route, Is.EqualTo("Test Route"));
            Assert.That(result.ScheduledDepartureDate, Is.EqualTo(DateTime.UtcNow.Date.ToString("yyyy-MM-dd")));
            Assert.That(result.ScheduledDepartureTime, Is.EqualTo("10:30:00"));
        }


        [Test]
        public async Task GetCompleteCheckDetailsAsync_ValidCheckSummaryIdWithoutOutcomes_ReturnsEmptyOutcomeAndReferral()
        {
            var checkSummaryId = Guid.NewGuid();
            var applicationId = Guid.NewGuid();

            var pet = new Entities.Pet { Id = Guid.NewGuid(), MicrochipNumber = "1234567890" };
            var application = new Entities.Application
            {
                Id = applicationId,
                Pet = pet,
                ReferenceNumber = "REF123",
                Status = "Active"
            };

            var route = new Entities.Route { Id = 4, RouteName = "Test Route" };

            if (_dbContext == null)
            {
                throw new InvalidOperationException("Database context is not initialized.");
            }

            var existingRoute = _dbContext.Route.SingleOrDefaultAsync(r => r.Id == route.Id);
            if (existingRoute == null)
            {
                _dbContext.Route.Add(route);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                DetachTrackedEntities();
            }

            var checkerId = Guid.NewGuid();
            AddCheckerEntity(checkerId, "Jane", "Smith", "Jane Smith");

            var checkSummary = new CheckSummary
            {
                Id = checkSummaryId,
                Application = application,
                CheckerId = checkerId,
                RouteNavigation = route,
                UpdatedOn = DateTime.UtcNow,
                Date = DateTime.UtcNow.Date,
                ScheduledSailingTime = new TimeSpan(10, 30, 0)
            };

            await _dbContext.Pet.AddAsync(pet);
            await _dbContext.Application.AddAsync(application);
            await _dbContext.CheckSummary.AddAsync(checkSummary);
            await _dbContext.SaveChangesAsync();

            if (_service == null)
            {
                throw new InvalidOperationException("Service is not initialized.");
            }

            var result = await _service.GetCompleteCheckDetailsAsync(checkSummaryId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.CheckOutcome, Is.Empty);
            Assert.That(result.ReasonForReferral, Is.Empty);
            Assert.That(result.AdditionalComments, Is.Empty);
            Assert.That(result.GBCheckerName, Is.EqualTo("Jane Smith"));
            Assert.That(result.Route, Is.EqualTo("Test Route"));
            Assert.That(result.ScheduledDepartureDate, Is.EqualTo(DateTime.UtcNow.Date.ToString("yyyy-MM-dd")));
            Assert.That(result.ScheduledDepartureTime, Is.EqualTo("10:30:00"));
        }

        [Test]
        public async Task GetCompleteCheckDetailsAsync_InvalidCheckSummaryId_ReturnsNull()
        {
            var invalidCheckSummaryId = Guid.NewGuid();

            if (_service == null)
            {
                throw new InvalidOperationException("Service is not initialized.");
            }

            var result = await _service.GetCompleteCheckDetailsAsync(invalidCheckSummaryId);

            Assert.That(result, Is.Null);
        }


        private void DetachTrackedEntities()
        {
            if (_dbContext == null)
            {
                throw new InvalidOperationException("Database context is not initialized.");
            }

            foreach (var entry in _dbContext.ChangeTracker.Entries().ToList())
            {
                entry.State = EntityState.Detached;
            }
        }





    }
    public static class DataHelper
    {
        public static void AddRoutes(CommonDbContext context)
        {
            var existingRoute1 = context.Route.SingleOrDefault(r => r.RouteName == "Route1");
            if (existingRoute1 == null)
            {
                context.Route.Add(new Entities.Route { RouteName = "Route1", Id = 1 });
            }

            var existingRoute2 = context.Route.SingleOrDefault(r => r.RouteName == "Route2");
            if (existingRoute2 == null)
            {
                context.Route.Add(new Entities.Route { RouteName = "Route2", Id = 2 });
            }

            context.SaveChanges();
        }

        public static void DetachTrackedEntities(CommonDbContext context)
        {
            foreach (var entry in context.ChangeTracker.Entries())
            {
                entry.State = EntityState.Detached;
            }
        }
    }



}
