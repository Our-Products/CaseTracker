using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CaseTracker.Functions.Functions;
using CaseTrackerApplication.DTOs.ECourts;
using CaseTrackerApplication.Interfaces.Services.ECourts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

namespace CaseTracker.Functions.Tests
{
    public class HearingSyncFunctionTests
    {
        [Fact]
        public async Task RunTimerAsync_ShouldInvokeRunScheduledHearingSyncAsyncAndLog()
        {
            // Arrange
            var mockSyncService = new Mock<IECourtSyncService>();
            var mockLogger = new Mock<ILogger<HearingSyncFunction>>();

            var expectedResult = new ECourtSyncResultDto
            {
                JobName = "HearingSync",
                TotalEligible = 3,
                Processed = 3,
                Succeeded = 3,
                Failed = 0
            };

            mockSyncService.Setup(s => s.RunScheduledHearingSyncAsync(null, 14))
                .ReturnsAsync(expectedResult);

            var function = new HearingSyncFunction(mockSyncService.Object, mockLogger.Object);

            // Act
            await function.RunTimerAsync(null!, CancellationToken.None);

            // Assert
            mockSyncService.Verify(s => s.RunScheduledHearingSyncAsync(null, 14), Times.Once);
        }

        [Fact]
        public async Task RunManualAsync_WithBatchAndDays_ShouldInvokeServiceAndReturnOk()
        {
            // Arrange
            var mockSyncService = new Mock<IECourtSyncService>();
            var mockLogger = new Mock<ILogger<HearingSyncFunction>>();

            var expectedResult = new ECourtSyncResultDto
            {
                JobName = "HearingSync",
                TotalEligible = 7,
                Processed = 7,
                Succeeded = 7,
                Failed = 0,
                Message = "Completed hearing sync."
            };

            mockSyncService.Setup(s => s.RunScheduledHearingSyncAsync(15, 30))
                .ReturnsAsync(expectedResult);

            var context = new DefaultHttpContext();
            context.Request.Query = new QueryCollection(new Dictionary<string, StringValues>
            {
                { "batchSize", "15" },
                { "daysAhead", "30" }
            });

            var function = new HearingSyncFunction(mockSyncService.Object, mockLogger.Object);

            // Act
            var actionResult = await function.RunManualAsync(context.Request, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            Assert.NotNull(okResult.Value);
            mockSyncService.Verify(s => s.RunScheduledHearingSyncAsync(15, 30), Times.Once);
        }

        [Fact]
        public async Task RunTimerAsync_WhenExceptionOccurs_ShouldRethrow()
        {
            // Arrange
            var mockSyncService = new Mock<IECourtSyncService>();
            var mockLogger = new Mock<ILogger<HearingSyncFunction>>();

            mockSyncService.Setup(s => s.RunScheduledHearingSyncAsync(null, 14))
                .ThrowsAsync(new InvalidOperationException("eCourts gateway unreachable"));

            var function = new HearingSyncFunction(mockSyncService.Object, mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                function.RunTimerAsync(null!, CancellationToken.None));
        }
    }
}
