using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CaseTrackerApplication.DTOs.Courts;
using CaseTrackerApplication.DTOs.ECourts;
using CaseTrackerApplication.Interfaces.Repositories.Courts;
using CaseTrackerApplication.Interfaces.Services.Courts;
using CaseTrackerApplication.Interfaces.Services.ECourts;
using CaseTrackerApplication.Services.Courts;
using CaseTrackerApplication.Services.ECourts;
using CaseTrackerDomain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CaseTracker.Functions.Tests
{
    public class SharedApplicationTests
    {
        [Fact]
        public async Task CourtMasterSyncService_NewDistrictAndComplex_ShouldInsertCorrectly()
        {
            // Arrange
            var mockCourtRepo = new Mock<ICourtRepository>();
            var mockECourtClient = new Mock<IECourtClient>();
            var mockLogger = new Mock<ILogger<CourtMasterSyncService>>();

            var state = new State { StateId = Guid.NewGuid(), StateCode = "TN", StateName = "Tamil Nadu" };
            mockCourtRepo.Setup(r => r.UpsertStateAsync("TN", "Tamil Nadu"))
                .ReturnsAsync(state);

            mockECourtClient.Setup(c => c.GetDistrictsAsync("TN"))
                .ReturnsAsync(new List<ECourtsDistrictItem>
                {
                    new ECourtsDistrictItem { DistrictCode = "01", DistrictName = "Chennai" }
                });

            mockCourtRepo.Setup(r => r.GetDistrictsByStateAsync(state.StateId))
                .ReturnsAsync(new List<District>());

            var insertedDistrict = new District
            {
                DistrictId = Guid.NewGuid(),
                StateId = state.StateId,
                DistrictCode = "01",
                DistrictName = "Chennai",
                Status = "Active"
            };

            mockCourtRepo.Setup(r => r.UpsertDistrictAsync(state.StateId, "01", "Chennai"))
                .ReturnsAsync(insertedDistrict);

            mockECourtClient.Setup(c => c.GetComplexesAsync("TN", "01"))
                .ReturnsAsync(new List<ECourtsComplexItem>
                {
                    new ECourtsComplexItem { CourtComplexCode = "0101", CourtComplexName = "City Civil Court" }
                });

            mockCourtRepo.Setup(r => r.GetComplexesByDistrictAsync(insertedDistrict.DistrictId))
                .ReturnsAsync(new List<CourtComplex>());

            var insertedComplex = new CourtComplex
            {
                CourtComplexId = Guid.NewGuid(),
                DistrictId = insertedDistrict.DistrictId,
                ComplexCode = "0101",
                ComplexName = "City Civil Court",
                Status = "Active"
            };

            mockCourtRepo.Setup(r => r.UpsertComplexAsync(insertedDistrict.DistrictId, "0101", "City Civil Court"))
                .ReturnsAsync(insertedComplex);

            var service = new CourtMasterSyncService(mockCourtRepo.Object, mockECourtClient.Object, mockLogger.Object);

            // Act
            var result = await service.SynchronizeStateAsync("TN");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Completed", result.Status);
            Assert.Equal(1, result.Districts.Inserted);
            Assert.Equal(1, result.CourtComplexes.Inserted);
            Assert.Equal(2, result.ApiRequests);
            mockCourtRepo.Verify(r => r.AddSyncHistoryAsync(It.IsAny<CourtMasterSyncHistory>()), Times.Once);
        }

        [Fact]
        public async Task CourtMasterSyncService_ExistingUnchangedRecords_ShouldSkipDatabaseWrites()
        {
            // Arrange
            var mockCourtRepo = new Mock<ICourtRepository>();
            var mockECourtClient = new Mock<IECourtClient>();
            var mockLogger = new Mock<ILogger<CourtMasterSyncService>>();

            var state = new State { StateId = Guid.NewGuid(), StateCode = "TN", StateName = "Tamil Nadu" };
            mockCourtRepo.Setup(r => r.UpsertStateAsync("TN", "Tamil Nadu"))
                .ReturnsAsync(state);

            mockECourtClient.Setup(c => c.GetDistrictsAsync("TN"))
                .ReturnsAsync(new List<ECourtsDistrictItem>
                {
                    new ECourtsDistrictItem { DistrictCode = "01", DistrictName = "Chennai" }
                });

            var existingDistrict = new District
            {
                DistrictId = Guid.NewGuid(),
                StateId = state.StateId,
                DistrictCode = "01",
                DistrictName = "Chennai",
                Status = "Active"
            };

            mockCourtRepo.Setup(r => r.GetDistrictsByStateAsync(state.StateId))
                .ReturnsAsync(new List<District> { existingDistrict });

            mockECourtClient.Setup(c => c.GetComplexesAsync("TN", "01"))
                .ReturnsAsync(new List<ECourtsComplexItem>
                {
                    new ECourtsComplexItem { CourtComplexCode = "0101", CourtComplexName = "City Civil Court" }
                });

            var existingComplex = new CourtComplex
            {
                CourtComplexId = Guid.NewGuid(),
                DistrictId = existingDistrict.DistrictId,
                ComplexCode = "0101",
                ComplexName = "City Civil Court",
                Status = "Active"
            };

            mockCourtRepo.Setup(r => r.GetComplexesByDistrictAsync(existingDistrict.DistrictId))
                .ReturnsAsync(new List<CourtComplex> { existingComplex });

            var service = new CourtMasterSyncService(mockCourtRepo.Object, mockECourtClient.Object, mockLogger.Object);

            // Act
            var result = await service.SynchronizeStateAsync("TN");

            // Assert
            Assert.Equal(1, result.Districts.Skipped);
            Assert.Equal(1, result.CourtComplexes.Skipped);
            Assert.Equal(0, result.Districts.Inserted);
            Assert.Equal(0, result.Districts.Updated);
            Assert.Equal(0, result.CourtComplexes.Inserted);
            Assert.Equal(0, result.CourtComplexes.Updated);

            // Verified zero upsert calls
            mockCourtRepo.Verify(r => r.UpsertDistrictAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mockCourtRepo.Verify(r => r.UpsertComplexAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task CourtMasterSyncService_MaxApiRequestsExceeded_ShouldHaltSafely()
        {
            // Arrange
            var mockCourtRepo = new Mock<ICourtRepository>();
            var mockECourtClient = new Mock<IECourtClient>();
            var mockLogger = new Mock<ILogger<CourtMasterSyncService>>();

            var stateTN = new State { StateId = Guid.NewGuid(), StateCode = "TN", StateName = "Tamil Nadu" };
            mockCourtRepo.Setup(r => r.UpsertStateAsync("TN", "Tamil Nadu")).ReturnsAsync(stateTN);

            mockECourtClient.Setup(c => c.GetDistrictsAsync("TN"))
                .ReturnsAsync(new List<ECourtsDistrictItem>
                {
                    new ECourtsDistrictItem { DistrictCode = "01", DistrictName = "Chennai" }
                });

            mockCourtRepo.Setup(r => r.GetDistrictsByStateAsync(stateTN.StateId))
                .ReturnsAsync(new List<District>());

            var settings = new CourtMasterSyncSettings
            {
                TargetStates = new List<string> { "TN", "PY" },
                MaxApiRequestsPerRun = 1 // Limit is 1 request!
            };

            var service = new CourtMasterSyncService(mockCourtRepo.Object, mockECourtClient.Object, mockLogger.Object);

            // Act
            var result = await service.SynchronizeAsync(settings);

            // Assert
            Assert.Equal("Partial", result.Status);
            Assert.Equal(1, result.ApiRequests);
            mockECourtClient.Verify(c => c.GetDistrictsAsync("PY"), Times.Never);
        }

        [Fact]
        public async Task ECourtSyncService_RunScheduledCaseStatusSyncAsync_ShouldUpdateStatusAndLog()
        {
            // Arrange
            var mockCaseRepo = new Mock<CaseTrackerApplication.Interfaces.Repositories.Cases.ICaseRepository>();
            var mockHearingRepo = new Mock<CaseTrackerApplication.Interfaces.Repositories.Hearings.IHearingRepository>();
            var mockECourtClient = new Mock<IECourtClient>();
            var mockLogRepo = new Mock<CaseTrackerApplication.Interfaces.Repositories.ECourts.IECourtApiLogRepository>();
            var mockUow = new Mock<CaseTrackerApplication.Interfaces.Repositories.IUnitOfWork>();

            var caseId = Guid.NewGuid();
            var testCase = new Case
            {
                CaseId = caseId,
                CnrNumber = "TNCH010000012025",
                CaseStatus = "Pending",
                IsEcourtSynced = true,
                Status = "Active"
            };

            mockCaseRepo.Setup(r => r.GetEligibleForSyncAsync(25, 24))
                .ReturnsAsync(new List<Case> { testCase });

            mockECourtClient.Setup(c => c.GetCaseDetailAsync("TNCH010000012025"))
                .ReturnsAsync(new ECourtsCaseDetailPayload
                {
                    CourtCaseData = new ECourtsCourtCaseData
                    {
                        Cnr = "TNCH010000012025",
                        CaseStatus = "Hearing",
                        CaseNumber = "OS/123/2025",
                        RegistrationNumber = "REG/456/2025",
                        HistoryOfCaseHearings = new List<ECourtsHearingHistoryItem>()
                    }
                });

            var syncService = new ECourtSyncService(
                mockCaseRepo.Object,
                mockHearingRepo.Object,
                mockECourtClient.Object,
                mockLogRepo.Object,
                mockUow.Object);

            // Act
            var result = await syncService.RunScheduledCaseStatusSyncAsync(25);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalEligible);
            Assert.Equal(1, result.Processed);
            Assert.Equal(1, result.Succeeded);
            Assert.Equal(0, result.Failed);
            Assert.Equal("Hearing", testCase.CaseStatus);
            Assert.Equal("OS/123/2025", testCase.CaseNumber);
            Assert.Equal("REG/456/2025", testCase.RegistrationNumber);
            Assert.Equal("Success", testCase.SyncStatus);
            mockCaseRepo.Verify(r => r.UpdateAsync(testCase), Times.Once);
            mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ECourtSyncService_RunScheduledHearingSyncAsync_ShouldSyncNewAndExistingHearings()
        {
            // Arrange
            var mockCaseRepo = new Mock<CaseTrackerApplication.Interfaces.Repositories.Cases.ICaseRepository>();
            var mockHearingRepo = new Mock<CaseTrackerApplication.Interfaces.Repositories.Hearings.IHearingRepository>();
            var mockECourtClient = new Mock<IECourtClient>();
            var mockLogRepo = new Mock<CaseTrackerApplication.Interfaces.Repositories.ECourts.IECourtApiLogRepository>();
            var mockUow = new Mock<CaseTrackerApplication.Interfaces.Repositories.IUnitOfWork>();

            var caseId = Guid.NewGuid();
            var testCase = new Case
            {
                CaseId = caseId,
                CnrNumber = "TNCH010000022025",
                CaseStatus = "Pending",
                IsEcourtSynced = true,
                Status = "Active"
            };

            mockCaseRepo.Setup(r => r.GetEligibleForHearingSyncAsync(10, 14))
                .ReturnsAsync(new List<Case> { testCase });

            var existingHearingDate = DateTime.UtcNow.Date.AddDays(2);
            var existingHearing = new CaseHearing
            {
                HearingId = Guid.NewGuid(),
                CaseId = caseId,
                HearingDate = existingHearingDate,
                PurposeOfHearing = "Preliminary Hearing",
                JudgeName = "Original Judge"
            };

            mockHearingRepo.Setup(r => r.GetHearingsByCaseAsync(caseId))
                .ReturnsAsync(new List<CaseHearing> { existingHearing });

            var futureDateString = existingHearingDate.ToString("yyyy-MM-dd");
            var nextHearingDate = DateTime.UtcNow.Date.AddDays(7);
            var nextDateString = nextHearingDate.ToString("yyyy-MM-dd");

            mockECourtClient.Setup(c => c.GetCaseDetailAsync("TNCH010000022025"))
                .ReturnsAsync(new ECourtsCaseDetailPayload
                {
                    CourtCaseData = new ECourtsCourtCaseData
                    {
                        Cnr = "TNCH010000022025",
                        HistoryOfCaseHearings = new List<ECourtsHearingHistoryItem>
                        {
                            new ECourtsHearingHistoryItem
                            {
                                HearingDate = futureDateString,
                                Judge = "Updated Chief Judge",
                                PurposeOfListing = "Arguments",
                                BusinessOnDate = "Matter taken up"
                            },
                            new ECourtsHearingHistoryItem
                            {
                                HearingDate = nextDateString,
                                Judge = "Next Judge",
                                PurposeOfListing = "Final Orders",
                                BusinessOnDate = "Awaiting compliance"
                            }
                        }
                    }
                });

            var syncService = new ECourtSyncService(
                mockCaseRepo.Object,
                mockHearingRepo.Object,
                mockECourtClient.Object,
                mockLogRepo.Object,
                mockUow.Object);

            // Act
            var result = await syncService.RunScheduledHearingSyncAsync(10, 14);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("HearingSync", result.JobName);
            Assert.Equal(1, result.Processed);
            Assert.Equal(1, result.Succeeded);
            Assert.Equal(0, result.Failed);

            // Existing hearing updated
            Assert.Equal("Updated Chief Judge", existingHearing.JudgeName);
            Assert.Equal("Arguments", existingHearing.PurposeOfHearing);
            Assert.Equal("Matter taken up", existingHearing.BusinessOnDate);
            mockHearingRepo.Verify(r => r.UpdateAsync(existingHearing), Times.Once);

            // New hearing added
            mockHearingRepo.Verify(r => r.AddAsync(It.Is<CaseHearing>(h =>
                h.CaseId == caseId &&
                h.HearingDate.Date == nextHearingDate.Date &&
                h.PurposeOfHearing == "Final Orders")), Times.Once);

            mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ECourtSyncService_RunScheduledHearingSyncAsync_BlankCnr_ShouldSkip()
        {
            // Arrange
            var mockCaseRepo = new Mock<CaseTrackerApplication.Interfaces.Repositories.Cases.ICaseRepository>();
            var mockHearingRepo = new Mock<CaseTrackerApplication.Interfaces.Repositories.Hearings.IHearingRepository>();
            var mockECourtClient = new Mock<IECourtClient>();
            var mockLogRepo = new Mock<CaseTrackerApplication.Interfaces.Repositories.ECourts.IECourtApiLogRepository>();
            var mockUow = new Mock<CaseTrackerApplication.Interfaces.Repositories.IUnitOfWork>();

            var testCase = new Case
            {
                CaseId = Guid.NewGuid(),
                CnrNumber = "   ", // Blank CNR
                IsEcourtSynced = true,
                Status = "Active"
            };

            mockCaseRepo.Setup(r => r.GetEligibleForHearingSyncAsync(10, 14))
                .ReturnsAsync(new List<Case> { testCase });

            var syncService = new ECourtSyncService(
                mockCaseRepo.Object,
                mockHearingRepo.Object,
                mockECourtClient.Object,
                mockLogRepo.Object,
                mockUow.Object);

            // Act
            var result = await syncService.RunScheduledHearingSyncAsync(10, 14);

            // Assert
            Assert.Equal(1, result.Skipped);
            Assert.Equal(0, result.Processed);
            mockECourtClient.Verify(c => c.GetCaseDetailAsync(It.IsAny<string>()), Times.Never);
        }
    }
}
