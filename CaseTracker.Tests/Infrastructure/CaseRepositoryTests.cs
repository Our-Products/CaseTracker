using System;
using System.Linq;
using System.Threading.Tasks;
using CaseTrackerDomain.Models;
using CaseTrackerInfrastructure.Data;
using CaseTrackerInfrastructure.Repositories.Cases;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CaseTracker.Tests.Infrastructure
{
    public class CaseRepositoryTests
    {
        private ApplicationDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetEligibleForSyncAsync_ShouldFilterStrictlyBySyncRules()
        {
            // Arrange
            using var context = CreateDbContext();
            var repo = new CaseRepository(context);

            var now = DateTimeOffset.UtcNow;

            // 1. Eligible: Never synced
            var caseNeverSynced = new Case
            {
                CaseId = Guid.NewGuid(),
                CaseNumber = "WP/01/2026",
                CnrNumber = "TNHC010000012026",
                CaseType = "WP",
                CaseTitle = "Test 1",
                CaseStage = "Admission",
                CaseStatus = "Pending",
                Status = "Active",
                IsEcourtSynced = true,
                LastSyncedAt = null
            };

            // 2. Eligible: Synced 30 hours ago
            var caseOldSync = new Case
            {
                CaseId = Guid.NewGuid(),
                CaseNumber = "WP/02/2026",
                CnrNumber = "TNHC010000022026",
                CaseType = "WP",
                CaseTitle = "Test 2",
                CaseStage = "Hearing",
                CaseStatus = "Pending",
                Status = "Active",
                IsEcourtSynced = true,
                LastSyncedAt = now.AddHours(-30)
            };

            // 3. Ineligible: Synced 2 hours ago (within 24h interval)
            var caseRecentSync = new Case
            {
                CaseId = Guid.NewGuid(),
                CaseNumber = "WP/03/2026",
                CnrNumber = "TNHC010000032026",
                CaseType = "WP",
                CaseTitle = "Test 3",
                CaseStage = "Hearing",
                CaseStatus = "Pending",
                Status = "Active",
                IsEcourtSynced = true,
                LastSyncedAt = now.AddHours(-2)
            };

            // 4. Ineligible: Disposed
            var caseDisposed = new Case
            {
                CaseId = Guid.NewGuid(),
                CaseNumber = "WP/04/2026",
                CnrNumber = "TNHC010000042026",
                CaseType = "WP",
                CaseTitle = "Test 4",
                CaseStage = "Disposed",
                CaseStatus = "Disposed",
                Status = "Active",
                IsEcourtSynced = true,
                LastSyncedAt = now.AddHours(-40)
            };

            // 5. Ineligible: Archived/Inactive
            var caseArchived = new Case
            {
                CaseId = Guid.NewGuid(),
                CaseNumber = "WP/05/2026",
                CnrNumber = "TNHC010000052026",
                CaseType = "WP",
                CaseTitle = "Test 5",
                CaseStage = "Hearing",
                CaseStatus = "Pending",
                Status = "Archived",
                IsEcourtSynced = true,
                LastSyncedAt = null
            };

            // 6. Ineligible: No CNR
            var caseNoCnr = new Case
            {
                CaseId = Guid.NewGuid(),
                CaseNumber = "WP/06/2026",
                CnrNumber = null,
                CaseType = "WP",
                CaseTitle = "Test 6",
                CaseStage = "Hearing",
                CaseStatus = "Pending",
                Status = "Active",
                IsEcourtSynced = true,
                LastSyncedAt = null
            };

            await context.Cases.AddRangeAsync(caseNeverSynced, caseOldSync, caseRecentSync, caseDisposed, caseArchived, caseNoCnr);
            await context.SaveChangesAsync();

            // Act
            var eligible = (await repo.GetEligibleForSyncAsync(10, 24)).ToList();

            // Assert
            Assert.Equal(2, eligible.Count);
            Assert.Contains(eligible, c => c.CaseId == caseNeverSynced.CaseId);
            Assert.Contains(eligible, c => c.CaseId == caseOldSync.CaseId);
            Assert.DoesNotContain(eligible, c => c.CaseId == caseRecentSync.CaseId);
            Assert.DoesNotContain(eligible, c => c.CaseId == caseDisposed.CaseId);
            Assert.DoesNotContain(eligible, c => c.CaseId == caseArchived.CaseId);
            Assert.DoesNotContain(eligible, c => c.CaseId == caseNoCnr.CaseId);
        }
    }
}
