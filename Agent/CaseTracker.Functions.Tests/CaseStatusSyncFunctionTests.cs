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
    public class CaseStatusSyncFunctionTests
    {
        [Fact]
        public async Task RunTimerAsync_ShouldInvokeRunScheduledCaseStatusSyncAsyncAndLog()
        {
            // Arrange
            var mockSyncService = new Mock<IECourtSyncService>();
            var mockLogger = new Mock<ILogger<CaseStatusSyncFunction>>();

            var expectedResult = new ECourtSyncResultDto
            {
                JobName = "CaseStatusSync",
                TotalEligible = 5,
                Processed = 5,
                Succeeded = 5,
                Failed = 0
            };

            mockSyncService.Setup(s => s.RunScheduledCaseStatusSyncAsync(null))
                .ReturnsAsync(expectedResult);

            var function = new CaseStatusSyncFunction(mockSyncService.Object, mockLogger.Object);

            // Act
            await function.RunTimerAsync(null!, CancellationToken.None);

            // Assert
            mockSyncService.Verify(s => s.RunScheduledCaseStatusSyncAsync(null), Times.Once);
        }

        [Fact]
        public async Task RunManualAsync_WithBatchSize_ShouldInvokeServiceAndReturnOk()
        {
            // Arrange
            var mockSyncService = new Mock<IECourtSyncService>();
            var mockLogger = new Mock<ILogger<CaseStatusSyncFunction>>();

            var expectedResult = new ECourtSyncResultDto
            {
                JobName = "CaseStatusSync",
                TotalEligible = 10,
                Processed = 10,
                Succeeded = 9,
                Failed = 1,
                Message = "Completed batch sync."
            };

            mockSyncService.Setup(s => s.RunScheduledCaseStatusSyncAsync(10))
                .ReturnsAsync(expectedResult);

            var context = new DefaultHttpContext();
            context.Request.Query = new QueryCollection(new Dictionary<string, StringValues>
            {
                { "batchSize", "10" }
            });

            var function = new CaseStatusSyncFunction(mockSyncService.Object, mockLogger.Object);

            // Act
            var actionResult = await function.RunManualAsync(context.Request, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            Assert.NotNull(okResult.Value);
            mockSyncService.Verify(s => s.RunScheduledCaseStatusSyncAsync(10), Times.Once);
        }

        [Fact]
        public async Task RunTimerAsync_WhenExceptionOccurs_ShouldRethrow()
        {
            // Arrange
            var mockSyncService = new Mock<IECourtSyncService>();
            var mockLogger = new Mock<ILogger<CaseStatusSyncFunction>>();

            mockSyncService.Setup(s => s.RunScheduledCaseStatusSyncAsync(null))
                .ThrowsAsync(new InvalidOperationException("API connection timeout"));

            var function = new CaseStatusSyncFunction(mockSyncService.Object, mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                function.RunTimerAsync(null!, CancellationToken.None));
        }
    }
}
