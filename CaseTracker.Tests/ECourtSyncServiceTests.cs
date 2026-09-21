using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CaseTrackerApplication.DTOs.ECourts;
using CaseTrackerApplication.Interfaces.Repositories;
using CaseTrackerApplication.Interfaces.Repositories.Cases;
using CaseTrackerApplication.Interfaces.Repositories.ECourts;
using CaseTrackerApplication.Interfaces.Repositories.Hearings;
using CaseTrackerApplication.Interfaces.Services.ECourts;
using CaseTrackerApplication.Services.ECourts;
using CaseTrackerDomain.Models;
using Moq;
using Xunit;

namespace CaseTracker.Tests
{
    public class ECourtSyncServiceTests
    {
        [Fact]
        public async Task RunScheduledCaseStatusSyncAsync_WhenBatchHasSuccessAndFailures_ShouldIsolateErrorsAndContinueBatch()
        {
            // Arrange
            var mockCaseRepo = new Mock<ICaseRepository>();
            var mockHearingRepo = new Mock<IHearingRepository>();
            var mockECourtClient = new Mock<IECourtClient>();
            var mockApiLogRepo = new Mock<IECourtApiLogRepository>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            var case1 = new Case
            {
                CaseId = Guid.NewGuid(),
                CnrNumber = "TNHC010001232026",
                CaseNumber = "WP/123/2026",
                Status = "Active"
            };

            var case2 = new Case
            {
                CaseId = Guid.NewGuid(),
                CnrNumber = "TNHC010004562026",
                CaseNumber = "WP/456/2026",
                Status = "Active"
            };

            var case3 = new Case
            {
                CaseId = Guid.NewGuid(),
                CnrNumber = "TNHC010007892026",
                CaseNumber = "WP/789/2026",
                Status = "Active"
            };

            var eligibleCases = new List<Case> { case1, case2, case3 };

            mockCaseRepo.Setup(r => r.GetEligibleForSyncAsync(25, 24))
                .ReturnsAsync(eligibleCases);

            // Case 1 succeeds
            mockECourtClient.Setup(c => c.GetCaseDetailAsync(case1.CnrNumber))
                .ReturnsAsync(new ECourtsCaseDetailPayload
                {
                    CourtCaseData = new ECourtsCourtCaseData
                    {
                        Cnr = case1.CnrNumber,
                        CaseStatus = "Disposed",
                        HistoryOfCaseHearings = new List<ECourtsHearingHistoryItem>
                        {
                            new ECourtsHearingHistoryItem
                            {
                                HearingDate = "2026-10-15",
                                PurposeOfListing = "Final Arguments",
                                Judge = "Hon. Justice Raman"
                            }
                        }
                    }
                });

            mockHearingRepo.Setup(r => r.GetHearingsByCaseAsync(case1.CaseId))
                .ReturnsAsync(new List<CaseHearing>());

            // Case 2 throws network/timeout error
            mockECourtClient.Setup(c => c.GetCaseDetailAsync(case2.CnrNumber))
                .ThrowsAsync(new HttpRequestException("Connection timed out to eCourts gateway"));

            // Case 3 succeeds
            mockECourtClient.Setup(c => c.GetCaseDetailAsync(case3.CnrNumber))
                .ReturnsAsync(new ECourtsCaseDetailPayload
                {
                    CourtCaseData = new ECourtsCourtCaseData
                    {
                        Cnr = case3.CnrNumber,
                        CaseStatus = "Hearing",
                        CaseNumber = "WP/789/2026"
                    }
                });

            mockHearingRepo.Setup(r => r.GetHearingsByCaseAsync(case3.CaseId))
                .ReturnsAsync(new List<CaseHearing>());

            var syncService = new ECourtSyncService(
                mockCaseRepo.Object,
                mockHearingRepo.Object,
                mockECourtClient.Object,
                mockApiLogRepo.Object,
                mockUnitOfWork.Object);

            // Act
            var result = await syncService.RunScheduledCaseStatusSyncAsync(25);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.TotalEligible);
            Assert.Equal(3, result.Processed);
            Assert.Equal(2, result.Succeeded);
            Assert.Equal(1, result.Failed);
            Assert.Equal(2, result.CreditsCharged); // 1 credit for each successful case

            // Case 1 verified updated
            Assert.Equal("Disposed", case1.CaseStatus);
            Assert.Equal("Success", case1.SyncStatus);
            Assert.Null(case1.SyncError);
            Assert.NotNull(case1.LastSuccessfulSyncAt);

            // Case 2 verified failure recorded and isolated
            Assert.Equal("Failed", case2.SyncStatus);
            Assert.Contains("Connection timed out", case2.SyncError);
            Assert.NotNull(case2.LastSyncAttemptAt);

            // Case 3 verified updated
            Assert.Equal("Hearing", case3.CaseStatus);
            Assert.Equal("Success", case3.SyncStatus);
            Assert.Null(case3.SyncError);

            // Verify UnitOfWork saved changes for all 3 cases
            mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.AtLeast(3));
        }

        [Fact]
        public async Task RunScheduledCaseStatusSyncAsync_WhenNoCasesEligible_ShouldReturnZeroCountsWithoutCallingApi()
        {
            // Arrange
            var mockCaseRepo = new Mock<ICaseRepository>();
            var mockHearingRepo = new Mock<IHearingRepository>();
            var mockECourtClient = new Mock<IECourtClient>();
            var mockApiLogRepo = new Mock<IECourtApiLogRepository>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            mockCaseRepo.Setup(r => r.GetEligibleForSyncAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<Case>());

            var syncService = new ECourtSyncService(
                mockCaseRepo.Object,
                mockHearingRepo.Object,
                mockECourtClient.Object,
                mockApiLogRepo.Object,
                mockUnitOfWork.Object);

            // Act
            var result = await syncService.RunScheduledCaseStatusSyncAsync(10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.TotalEligible);
            Assert.Equal(0, result.Processed);
            Assert.Equal(0, result.Succeeded);
            Assert.Equal(0, result.Failed);
            Assert.Equal(0, result.CreditsCharged);

            mockECourtClient.Verify(c => c.GetCaseDetailAsync(It.IsAny<string>()), Times.Never);
        }
    }
}
