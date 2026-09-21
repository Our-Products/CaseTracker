using System;
using CaseTrackerDomain.Models;
using Xunit;

namespace CaseTracker.Tests.Domain
{
    public class CourtMasterSyncHistoryTests
    {
        [Fact]
        public void CourtMasterSyncHistory_ShouldInitializeAndRetainAuditProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var start = DateTimeOffset.UtcNow.AddSeconds(-10);
            var end = DateTimeOffset.UtcNow;

            // Act
            var history = new CourtMasterSyncHistory
            {
                Id = id,
                State = "TN",
                StartedAt = start,
                CompletedAt = end,
                Status = "Completed",
                RecordsRead = 100,
                RecordsInserted = 5,
                RecordsUpdated = 2,
                RecordsSkipped = 93,
                RecordsFailed = 0,
                ApiRequests = 39,
                DurationMs = 10000,
                ErrorMessage = null
            };

            // Assert
            Assert.Equal(id, history.Id);
            Assert.Equal("TN", history.State);
            Assert.Equal("Completed", history.Status);
            Assert.Equal(100, history.RecordsRead);
            Assert.Equal(5, history.RecordsInserted);
            Assert.Equal(2, history.RecordsUpdated);
            Assert.Equal(93, history.RecordsSkipped);
            Assert.Equal(39, history.ApiRequests);
            Assert.Equal(10000, history.DurationMs);
            Assert.Null(history.ErrorMessage);
        }
    }
}
