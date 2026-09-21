using System;
using CaseTrackerDomain.Models;
using Xunit;

namespace CaseTracker.Functions.Tests
{
    public class SharedDomainTests
    {
        [Fact]
        public void State_Initialization_ShouldRetainValuesAndDefaults()
        {
            var id = Guid.NewGuid();
            var state = new State
            {
                StateId = id,
                StateCode = "TN",
                StateName = "Tamil Nadu",
                Status = "Active"
            };

            Assert.Equal(id, state.StateId);
            Assert.Equal("TN", state.StateCode);
            Assert.Equal("Tamil Nadu", state.StateName);
            Assert.Equal("Active", state.Status);
            Assert.NotNull(state.Districts);
            Assert.Empty(state.Districts);
        }

        [Fact]
        public void District_Initialization_ShouldLinkToState()
        {
            var distId = Guid.NewGuid();
            var stateId = Guid.NewGuid();

            var district = new District
            {
                DistrictId = distId,
                StateId = stateId,
                DistrictCode = "01",
                DistrictName = "Chennai",
                Status = "Active"
            };

            Assert.Equal(distId, district.DistrictId);
            Assert.Equal(stateId, district.StateId);
            Assert.Equal("01", district.DistrictCode);
            Assert.Equal("Chennai", district.DistrictName);
            Assert.NotNull(district.CourtComplexes);
            Assert.Empty(district.CourtComplexes);
        }

        [Fact]
        public void CourtComplex_Initialization_ShouldLinkToDistrict()
        {
            var complexId = Guid.NewGuid();
            var distId = Guid.NewGuid();

            var complex = new CourtComplex
            {
                CourtComplexId = complexId,
                DistrictId = distId,
                ComplexCode = "0101",
                ComplexName = "City Civil Court",
                Status = "Active"
            };

            Assert.Equal(complexId, complex.CourtComplexId);
            Assert.Equal(distId, complex.DistrictId);
            Assert.Equal("0101", complex.ComplexCode);
            Assert.Equal("City Civil Court", complex.ComplexName);
        }

        [Fact]
        public void CourtMasterSyncHistory_ShouldStoreAuditMetrics()
        {
            var id = Guid.NewGuid();
            var start = DateTimeOffset.UtcNow.AddMinutes(-2);
            var end = DateTimeOffset.UtcNow;

            var history = new CourtMasterSyncHistory
            {
                Id = id,
                State = "PY",
                StartedAt = start,
                CompletedAt = end,
                Status = "Completed",
                RecordsRead = 30,
                RecordsInserted = 2,
                RecordsUpdated = 1,
                RecordsSkipped = 27,
                RecordsFailed = 0,
                ApiRequests = 2,
                DurationMs = 1200
            };

            Assert.Equal(id, history.Id);
            Assert.Equal("PY", history.State);
            Assert.Equal("Completed", history.Status);
            Assert.Equal(30, history.RecordsRead);
            Assert.Equal(2, history.RecordsInserted);
            Assert.Equal(1, history.RecordsUpdated);
            Assert.Equal(27, history.RecordsSkipped);
            Assert.Equal(2, history.ApiRequests);
            Assert.Equal(1200, history.DurationMs);
        }
    }
}
