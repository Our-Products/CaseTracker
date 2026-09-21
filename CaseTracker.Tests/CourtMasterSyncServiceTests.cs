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
using CaseTrackerDomain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CaseTracker.Tests
{
    public class CourtMasterSyncServiceTests
    {
        [Fact]
        public async Task SynchronizeStateAsync_NewDistrictAndComplex_ShouldInsertCorrectly()
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
            Assert.Equal(1, result.Districts.Received);
            Assert.Equal(1, result.Districts.Inserted);
            Assert.Equal(0, result.Districts.Updated);
            Assert.Equal(0, result.Districts.Skipped);

            Assert.Equal(1, result.CourtComplexes.Received);
            Assert.Equal(1, result.CourtComplexes.Inserted);
            Assert.Equal(0, result.CourtComplexes.Updated);
            Assert.Equal(0, result.CourtComplexes.Skipped);

            Assert.Equal(2, result.ApiRequests); // 1 for districts, 1 for complexes
            mockCourtRepo.Verify(r => r.AddSyncHistoryAsync(It.IsAny<CourtMasterSyncHistory>()), Times.Once);
        }

        [Fact]
        public async Task SynchronizeStateAsync_ExistingDistrictAndComplexUnchanged_ShouldSkipUpdates()
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
            Assert.NotNull(result);
            Assert.Equal("Completed", result.Status);
            Assert.Equal(1, result.Districts.Received);
            Assert.Equal(0, result.Districts.Inserted);
            Assert.Equal(0, result.Districts.Updated);
            Assert.Equal(1, result.Districts.Skipped); // Skipped because identical!

            Assert.Equal(1, result.CourtComplexes.Received);
            Assert.Equal(0, result.CourtComplexes.Inserted);
            Assert.Equal(0, result.CourtComplexes.Updated);
            Assert.Equal(1, result.CourtComplexes.Skipped); // Skipped because identical!

            // Verify no unnecessary upsert calls were made for district or complex
            mockCourtRepo.Verify(r => r.UpsertDistrictAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mockCourtRepo.Verify(r => r.UpsertComplexAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SynchronizeStateAsync_DistrictAndComplexChanged_ShouldUpdateCorrectly()
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
                    new ECourtsDistrictItem { DistrictCode = "01", DistrictName = "Chennai District New Name" }
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

            mockCourtRepo.Setup(r => r.UpsertDistrictAsync(state.StateId, "01", "Chennai District New Name"))
                .ReturnsAsync(existingDistrict);

            mockECourtClient.Setup(c => c.GetComplexesAsync("TN", "01"))
                .ReturnsAsync(new List<ECourtsComplexItem>
                {
                    new ECourtsComplexItem { CourtComplexCode = "0101", CourtComplexName = "City Civil Court Complex Renamed" }
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

            mockCourtRepo.Setup(r => r.UpsertComplexAsync(existingDistrict.DistrictId, "0101", "City Civil Court Complex Renamed"))
                .ReturnsAsync(existingComplex);

            var service = new CourtMasterSyncService(mockCourtRepo.Object, mockECourtClient.Object, mockLogger.Object);

            // Act
            var result = await service.SynchronizeStateAsync("TN");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Completed", result.Status);
            Assert.Equal(1, result.Districts.Updated);
            Assert.Equal(0, result.Districts.Inserted);
            Assert.Equal(0, result.Districts.Skipped);

            Assert.Equal(1, result.CourtComplexes.Updated);
            Assert.Equal(0, result.CourtComplexes.Inserted);
            Assert.Equal(0, result.CourtComplexes.Skipped);

            mockCourtRepo.Verify(r => r.UpsertDistrictAsync(state.StateId, "01", "Chennai District New Name"), Times.Once);
            mockCourtRepo.Verify(r => r.UpsertComplexAsync(existingDistrict.DistrictId, "0101", "City Civil Court Complex Renamed"), Times.Once);
        }

        [Fact]
        public async Task SynchronizeStateAsync_WhenApiFails_ShouldRecordFailureInHistoryGracefully()
        {
            // Arrange
            var mockCourtRepo = new Mock<ICourtRepository>();
            var mockECourtClient = new Mock<IECourtClient>();
            var mockLogger = new Mock<ILogger<CourtMasterSyncService>>();

            var state = new State { StateId = Guid.NewGuid(), StateCode = "PY", StateName = "Puducherry" };
            mockCourtRepo.Setup(r => r.UpsertStateAsync("PY", "Puducherry"))
                .ReturnsAsync(state);

            mockECourtClient.Setup(c => c.GetDistrictsAsync("PY"))
                .ThrowsAsync(new HttpRequestException("Gateway timeout while connecting to eCourts India API."));

            var service = new CourtMasterSyncService(mockCourtRepo.Object, mockECourtClient.Object, mockLogger.Object);

            // Act
            var result = await service.SynchronizeStateAsync("PY");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Partial", result.Status);
            Assert.Contains("Failed to fetch districts", result.ErrorMessage);

            mockCourtRepo.Verify(r => r.AddSyncHistoryAsync(It.Is<CourtMasterSyncHistory>(h =>
                h.State == "PY" && h.Status == "Failed")), Times.Once);
        }

        [Fact]
        public async Task SynchronizeAsync_WhenMaxApiRequestsReached_ShouldHaltSafelyWithPartialStatus()
        {
            // Arrange
            var mockCourtRepo = new Mock<ICourtRepository>();
            var mockECourtClient = new Mock<IECourtClient>();
            var mockLogger = new Mock<ILogger<CourtMasterSyncService>>();

            var stateTN = new State { StateId = Guid.NewGuid(), StateCode = "TN", StateName = "Tamil Nadu" };
            mockCourtRepo.Setup(r => r.UpsertStateAsync("TN", "Tamil Nadu"))
                .ReturnsAsync(stateTN);

            mockECourtClient.Setup(c => c.GetDistrictsAsync("TN"))
                .ReturnsAsync(new List<ECourtsDistrictItem>
                {
                    new ECourtsDistrictItem { DistrictCode = "01", DistrictName = "Chennai" },
                    new ECourtsDistrictItem { DistrictCode = "02", DistrictName = "Coimbatore" }
                });

            mockCourtRepo.Setup(r => r.GetDistrictsByStateAsync(stateTN.StateId))
                .ReturnsAsync(new List<District>());

            var settings = new CourtMasterSyncSettings
            {
                TargetStates = new List<string> { "TN", "PY" },
                MaxApiRequestsPerRun = 1 // Limit to 1 request
            };

            var service = new CourtMasterSyncService(mockCourtRepo.Object, mockECourtClient.Object, mockLogger.Object);

            // Act
            var result = await service.SynchronizeAsync(settings);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Partial", result.Status);
            Assert.Contains("Reached safety API request limit", result.ErrorMessage);
            Assert.Equal(1, result.ApiRequests);
            mockECourtClient.Verify(c => c.GetDistrictsAsync("PY"), Times.Never); // PY was skipped due to limit!
        }

        [Fact]
        public async Task SynchronizeAsync_BothTamilNaduAndPuducherry_ShouldSyncBothSuccessfully()
        {
            // Arrange
            var mockCourtRepo = new Mock<ICourtRepository>();
            var mockECourtClient = new Mock<IECourtClient>();
            var mockLogger = new Mock<ILogger<CourtMasterSyncService>>();

            var stateTN = new State { StateId = Guid.NewGuid(), StateCode = "TN", StateName = "Tamil Nadu" };
            var statePY = new State { StateId = Guid.NewGuid(), StateCode = "PY", StateName = "Puducherry" };

            mockCourtRepo.Setup(r => r.UpsertStateAsync("TN", "Tamil Nadu")).ReturnsAsync(stateTN);
            mockCourtRepo.Setup(r => r.UpsertStateAsync("PY", "Puducherry")).ReturnsAsync(statePY);

            mockECourtClient.Setup(c => c.GetDistrictsAsync("TN"))
                .ReturnsAsync(new List<ECourtsDistrictItem>
                {
                    new ECourtsDistrictItem { DistrictCode = "01", DistrictName = "Chennai" }
                });

            mockECourtClient.Setup(c => c.GetDistrictsAsync("PY"))
                .ReturnsAsync(new List<ECourtsDistrictItem>
                {
                    new ECourtsDistrictItem { DistrictCode = "10", DistrictName = "Puducherry" }
                });

            mockCourtRepo.Setup(r => r.GetDistrictsByStateAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<District>());

            mockCourtRepo.Setup(r => r.UpsertDistrictAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new District { DistrictId = Guid.NewGuid() });

            mockECourtClient.Setup(c => c.GetComplexesAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<ECourtsComplexItem>());

            mockCourtRepo.Setup(r => r.GetComplexesByDistrictAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<CourtComplex>());

            var settings = new CourtMasterSyncSettings
            {
                TargetStates = new List<string> { "TN", "PY" },
                MaxApiRequestsPerRun = 50
            };

            var service = new CourtMasterSyncService(mockCourtRepo.Object, mockECourtClient.Object, mockLogger.Object);

            // Act
            var result = await service.SynchronizeAsync(settings);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Completed", result.Status);
            Assert.Contains("TN", result.States);
            Assert.Contains("PY", result.States);
            Assert.Equal(2, result.Districts.Inserted);
            Assert.Equal(4, result.ApiRequests); // 1 dist + 1 comp for TN, 1 dist + 1 comp for PY

            mockCourtRepo.Verify(r => r.AddSyncHistoryAsync(It.Is<CourtMasterSyncHistory>(h => h.State == "TN")), Times.Once);
            mockCourtRepo.Verify(r => r.AddSyncHistoryAsync(It.Is<CourtMasterSyncHistory>(h => h.State == "PY")), Times.Once);
        }
    }
}
