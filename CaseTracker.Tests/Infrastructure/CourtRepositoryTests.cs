using System;
using System.Linq;
using System.Threading.Tasks;
using CaseTrackerDomain.Models;
using CaseTrackerInfrastructure.Data;
using CaseTrackerInfrastructure.Repositories.Courts;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CaseTracker.Tests.Infrastructure
{
    public class CourtRepositoryTests
    {
        private ApplicationDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task UpsertStateAsync_ShouldInsertStateAndBeIdempotent()
        {
            // Arrange
            using var context = CreateDbContext();
            var repo = new CourtRepository(context);

            // Act 1 - Insert
            var state1 = await repo.UpsertStateAsync("TN", "Tamil Nadu");

            // Assert 1
            Assert.NotNull(state1);
            Assert.Equal("TN", state1.StateCode);
            Assert.Equal("Tamil Nadu", state1.StateName);
            Assert.Equal(1, await context.States.CountAsync());

            // Act 2 - Idempotent call with same name
            var state2 = await repo.UpsertStateAsync("TN", "Tamil Nadu");

            // Assert 2
            Assert.Equal(state1.StateId, state2.StateId);
            Assert.Equal(1, await context.States.CountAsync());
        }

        [Fact]
        public async Task UpsertDistrictAsync_ShouldInsertDistrictUnderState()
        {
            // Arrange
            using var context = CreateDbContext();
            var repo = new CourtRepository(context);

            var state = await repo.UpsertStateAsync("TN", "Tamil Nadu");

            // Act
            var district = await repo.UpsertDistrictAsync(state.StateId, "01", "Chennai");

            // Assert
            Assert.NotNull(district);
            Assert.Equal("01", district.DistrictCode);
            Assert.Equal("Chennai", district.DistrictName);
            Assert.Equal(state.StateId, district.StateId);

            var districtsInState = (await repo.GetDistrictsByStateAsync(state.StateId)).ToList();
            Assert.Single(districtsInState);
            Assert.Equal("Chennai", districtsInState[0].DistrictName);
        }

        [Fact]
        public async Task AddSyncHistoryAsync_ShouldPersistAuditHistory()
        {
            // Arrange
            using var context = CreateDbContext();
            var repo = new CourtRepository(context);

            var history = new CourtMasterSyncHistory
            {
                Id = Guid.NewGuid(),
                State = "TN",
                StartedAt = DateTimeOffset.UtcNow.AddMinutes(-1),
                CompletedAt = DateTimeOffset.UtcNow,
                Status = "Completed",
                RecordsRead = 50,
                RecordsInserted = 10,
                RecordsUpdated = 2,
                RecordsSkipped = 38,
                ApiRequests = 15,
                DurationMs = 2500
            };

            // Act
            await repo.AddSyncHistoryAsync(history);

            // Assert
            var histories = (await repo.GetSyncHistoriesAsync(5)).ToList();
            Assert.Single(histories);
            Assert.Equal("TN", histories[0].State);
            Assert.Equal("Completed", histories[0].Status);
            Assert.Equal(50, histories[0].RecordsRead);
        }
    }
}
