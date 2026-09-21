using System;
using System.Linq;
using System.Threading.Tasks;
using CaseTrackerDomain.Models;
using CaseTrackerInfrastructure.Data;
using CaseTrackerInfrastructure.Repositories.Courts;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CaseTracker.Functions.Tests
{
    public class SharedInfrastructureTests
    {
        private ApplicationDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CourtRepository_UpsertStateAsync_ShouldBeIdempotent()
        {
            using var context = CreateDbContext();
            var repo = new CourtRepository(context);

            var s1 = await repo.UpsertStateAsync("TN", "Tamil Nadu");
            Assert.NotNull(s1);
            Assert.Equal(1, await context.States.CountAsync());

            var s2 = await repo.UpsertStateAsync("TN", "Tamil Nadu");
            Assert.Equal(s1.StateId, s2.StateId);
            Assert.Equal(1, await context.States.CountAsync());
        }

        [Fact]
        public async Task CourtRepository_UpsertDistrictAndComplex_ShouldEstablishHierarchy()
        {
            using var context = CreateDbContext();
            var repo = new CourtRepository(context);

            var state = await repo.UpsertStateAsync("PY", "Puducherry");
            var district = await repo.UpsertDistrictAsync(state.StateId, "10", "Puducherry District");
            var complex = await repo.UpsertComplexAsync(district.DistrictId, "1001", "Integrated Court Complex");

            Assert.NotNull(district);
            Assert.Equal(state.StateId, district.StateId);

            Assert.NotNull(complex);
            Assert.Equal(district.DistrictId, complex.DistrictId);

            var districts = (await repo.GetDistrictsByStateAsync(state.StateId)).ToList();
            Assert.Single(districts);

            var complexes = (await repo.GetComplexesByDistrictAsync(district.DistrictId)).ToList();
            Assert.Single(complexes);
            Assert.Equal("Integrated Court Complex", complexes[0].ComplexName);
        }

        [Fact]
        public async Task CourtRepository_AddSyncHistoryAsync_ShouldPersistAuditRecord()
        {
            using var context = CreateDbContext();
            var repo = new CourtRepository(context);

            var history = new CourtMasterSyncHistory
            {
                Id = Guid.NewGuid(),
                State = "TN",
                StartedAt = DateTimeOffset.UtcNow.AddMinutes(-5),
                CompletedAt = DateTimeOffset.UtcNow,
                Status = "Completed",
                RecordsRead = 200,
                RecordsInserted = 10,
                RecordsUpdated = 0,
                RecordsSkipped = 190,
                ApiRequests = 42,
                DurationMs = 3200
            };

            await repo.AddSyncHistoryAsync(history);

            var list = (await repo.GetSyncHistoriesAsync(10)).ToList();
            Assert.Single(list);
            Assert.Equal("TN", list[0].State);
            Assert.Equal(200, list[0].RecordsRead);
        }

        [Fact]
        public async Task CaseRepository_GetEligibleForHearingSyncAsync_ShouldPrioritizeImminentHearings()
        {
            using var context = CreateDbContext();
            var caseRepo = new CaseTrackerInfrastructure.Repositories.Cases.CaseRepository(context);

            var court = new Court
            {
                CourtId = Guid.NewGuid(),
                CourtName = "High Court of Madras",
                CourtTypeId = "HC",
                CourtComplexId = Guid.NewGuid(),
                CourtCode = "HC01"
            };
            context.Courts.Add(court);

            var caseWithNearHearing = new Case
            {
                CaseId = Guid.NewGuid(),
                CourtId = court.CourtId,
                CaseNumber = "WP/101/2025",
                CaseType = "Writ Petition",
                CaseTitle = "Petitioner vs State",
                CaseStage = "Hearing",
                CaseStatus = "Pending",
                CnrNumber = "MCHC010001012025",
                IsEcourtSynced = true,
                Status = "Active",
                LastSyncedAt = DateTimeOffset.UtcNow.AddDays(-2)
            };

            var nearHearing = new CaseHearing
            {
                HearingId = Guid.NewGuid(),
                CaseId = caseWithNearHearing.CaseId,
                HearingDate = DateTime.UtcNow.Date.AddDays(3),
                PurposeOfHearing = "Admission",
                HearingStatus = "Scheduled"
            };
            caseWithNearHearing.CaseHearings.Add(nearHearing);

            var caseWithoutHearing = new Case
            {
                CaseId = Guid.NewGuid(),
                CourtId = court.CourtId,
                CaseNumber = "WP/102/2025",
                CaseType = "Writ Petition",
                CaseTitle = "Citizen vs Corporation",
                CaseStage = "Notice",
                CaseStatus = "Pending",
                CnrNumber = "MCHC010001022025",
                IsEcourtSynced = true,
                Status = "Active",
                LastSyncedAt = DateTimeOffset.UtcNow.AddDays(-10)
            };

            var disposedCase = new Case
            {
                CaseId = Guid.NewGuid(),
                CourtId = court.CourtId,
                CaseNumber = "WP/103/2025",
                CaseType = "Writ Petition",
                CaseTitle = "Old vs Entity",
                CaseStage = "Disposed",
                CaseStatus = "Disposed",
                CnrNumber = "MCHC010001032025",
                IsEcourtSynced = true,
                Status = "Active"
            };

            context.Cases.AddRange(caseWithNearHearing, caseWithoutHearing, disposedCase);
            await context.SaveChangesAsync();

            // Act: request batch of 10 within 14 days
            var eligible = (await caseRepo.GetEligibleForHearingSyncAsync(10, 14)).ToList();

            // Assert
            Assert.Equal(2, eligible.Count); // only active, non-disposed cases
            Assert.Equal(caseWithNearHearing.CaseId, eligible[0].CaseId); // prioritized near hearing first
            Assert.Equal(caseWithoutHearing.CaseId, eligible[1].CaseId); // backfilled active case second
            Assert.DoesNotContain(eligible, c => c.CaseId == disposedCase.CaseId);
        }

        [Fact]
        public async Task CaseRepository_GetEligibleForSyncAsync_ShouldFilterCutoffAndDisposedCases()
        {
            using var context = CreateDbContext();
            var caseRepo = new CaseTrackerInfrastructure.Repositories.Cases.CaseRepository(context);

            var court = new Court
            {
                CourtId = Guid.NewGuid(),
                CourtName = "High Court of Madras",
                CourtTypeId = "HC",
                CourtComplexId = Guid.NewGuid(),
                CourtCode = "HC02"
            };
            context.Courts.Add(court);

            var eligibleCase = new Case
            {
                CaseId = Guid.NewGuid(),
                CourtId = court.CourtId,
                CaseNumber = "OS/201/2025",
                CaseType = "Original Suit",
                CaseTitle = "A vs B",
                CaseStage = "Pleadings",
                CaseStatus = "Pending",
                CnrNumber = "TNCH010002012025",
                IsEcourtSynced = true,
                Status = "Active",
                LastSyncedAt = DateTimeOffset.UtcNow.AddHours(-30) // Older than 24h cutoff
            };

            var recentlySyncedCase = new Case
            {
                CaseId = Guid.NewGuid(),
                CourtId = court.CourtId,
                CaseNumber = "OS/202/2025",
                CaseType = "Original Suit",
                CaseTitle = "C vs D",
                CaseStage = "Pleadings",
                CaseStatus = "Pending",
                CnrNumber = "TNCH010002022025",
                IsEcourtSynced = true,
                Status = "Active",
                LastSyncedAt = DateTimeOffset.UtcNow.AddHours(-2) // Within 24h cutoff
            };

            context.Cases.AddRange(eligibleCase, recentlySyncedCase);
            await context.SaveChangesAsync();

            // Act
            var eligible = (await caseRepo.GetEligibleForSyncAsync(10, 24)).ToList();

            // Assert
            Assert.Single(eligible);
            Assert.Equal(eligibleCase.CaseId, eligible[0].CaseId);
        }
    }
}
