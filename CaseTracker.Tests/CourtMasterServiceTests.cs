using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CaseTrackerApplication.DTOs.ECourts;
using CaseTrackerApplication.Interfaces.Repositories.Courts;
using CaseTrackerApplication.Interfaces.Services.ECourts;
using CaseTrackerApplication.Services.Courts;
using CaseTrackerDomain.Models;
using Moq;
using Xunit;

namespace CaseTracker.Tests
{
    public class CourtMasterServiceTests
    {
        [Fact]
        public async Task SynchronizeMasterDataAsync_ShouldUpsertStateDistrictsAndCourts()
        {
            // Arrange
            var mockCourtRepo = new Mock<ICourtRepository>();
            var mockECourtClient = new Mock<IECourtClient>();

            var stateEntity = new State { StateId = Guid.NewGuid(), StateCode = "TN", StateName = "Tamil Nadu" };
            var districtEntity = new District { DistrictId = Guid.NewGuid(), StateId = stateEntity.StateId, DistrictCode = "01", DistrictName = "Chennai" };
            var complexEntity = new CourtComplex { CourtComplexId = Guid.NewGuid(), DistrictId = districtEntity.DistrictId, ComplexCode = "0101", ComplexName = "City Civil Court" };
            var courtEntity = new Court { CourtId = Guid.NewGuid(), CourtComplexId = complexEntity.CourtComplexId, CourtCode = "1", CourtName = "I Assistant Judge" };

            mockCourtRepo.Setup(r => r.UpsertStateAsync("TN", "Tamil Nadu"))
                .ReturnsAsync(stateEntity);

            mockECourtClient.Setup(c => c.GetDistrictsAsync("TN"))
                .ReturnsAsync(new List<ECourtsDistrictItem>
                {
                    new ECourtsDistrictItem { DistrictCode = "01", DistrictName = "Chennai" }
                });

            mockCourtRepo.Setup(r => r.UpsertDistrictAsync(stateEntity.StateId, "01", "Chennai"))
                .ReturnsAsync(districtEntity);

            mockECourtClient.Setup(c => c.GetComplexesAsync("TN", "01"))
                .ReturnsAsync(new List<ECourtsComplexItem>
                {
                    new ECourtsComplexItem { CourtComplexCode = "0101", CourtComplexName = "City Civil Court" }
                });

            mockCourtRepo.Setup(r => r.UpsertComplexAsync(districtEntity.DistrictId, "0101", "City Civil Court"))
                .ReturnsAsync(complexEntity);

            mockECourtClient.Setup(c => c.GetCourtsAsync("TN", "01", "0101"))
                .ReturnsAsync(new List<ECourtsCourtItem>
                {
                    new ECourtsCourtItem { Court = "1", CourtName = "I Assistant Judge", CourtNo = "1", JudgeName = "Hon. Judge A" }
                });

            mockCourtRepo.Setup(r => r.UpsertCourtAsync(complexEntity.CourtComplexId, "1", "I Assistant Judge", "1", "Hon. Judge A"))
                .ReturnsAsync(courtEntity);

            var service = new CourtMasterService(mockCourtRepo.Object, mockECourtClient.Object);

            // Act
            var result = await service.SynchronizeMasterDataAsync("TN");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TN", result.TargetState);
            Assert.Equal(1, result.StatesProcessed);
            Assert.Equal(1, result.DistrictsProcessed);
            Assert.Equal(1, result.ComplexesProcessed);
            Assert.Equal(1, result.CourtsProcessed);
            Assert.Contains("Successfully synchronized", result.Message);

            mockCourtRepo.Verify(r => r.UpsertStateAsync("TN", "Tamil Nadu"), Times.Once);
            mockCourtRepo.Verify(r => r.UpsertDistrictAsync(stateEntity.StateId, "01", "Chennai"), Times.Once);
            mockCourtRepo.Verify(r => r.UpsertComplexAsync(districtEntity.DistrictId, "0101", "City Civil Court"), Times.Once);
            mockCourtRepo.Verify(r => r.UpsertCourtAsync(complexEntity.CourtComplexId, "1", "I Assistant Judge", "1", "Hon. Judge A"), Times.Once);
        }

        [Fact]
        public async Task SynchronizeMasterDataAsync_ShouldHandleApiFailureGracefully()
        {
            // Arrange
            var mockCourtRepo = new Mock<ICourtRepository>();
            var mockECourtClient = new Mock<IECourtClient>();

            var stateEntity = new State { StateId = Guid.NewGuid(), StateCode = "PY", StateName = "Puducherry" };

            mockCourtRepo.Setup(r => r.UpsertStateAsync("PY", "Puducherry"))
                .ReturnsAsync(stateEntity);

            mockECourtClient.Setup(c => c.GetDistrictsAsync("PY"))
                .ThrowsAsync(new System.Net.Http.HttpRequestException("eCourts API Gateway Timeout"));

            var service = new CourtMasterService(mockCourtRepo.Object, mockECourtClient.Object);

            // Act
            var result = await service.SynchronizeMasterDataAsync("PY");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("PY", result.TargetState);
            Assert.Equal(1, result.StatesProcessed);
            Assert.Equal(0, result.DistrictsProcessed);
            Assert.Contains("partially completed with notice", result.Message);
        }
    }
}
