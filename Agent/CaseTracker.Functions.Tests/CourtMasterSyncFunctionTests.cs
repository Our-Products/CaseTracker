using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CaseTracker.Functions.Functions;
using CaseTrackerApplication.DTOs.Courts;
using CaseTrackerApplication.Interfaces.Services.Courts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

namespace CaseTracker.Functions.Tests
{
    public class CourtMasterSyncFunctionTests
    {
        [Fact]
        public async Task RunTimerAsync_ShouldInvokeSynchronizeAsyncAndLogSummary()
        {
            // Arrange
            var mockSyncService = new Mock<ICourtMasterSyncService>();
            var mockLogger = new Mock<ILogger<CourtMasterSyncFunction>>();

            var expectedSummary = new CourtMasterSyncExecutionSummaryDto
            {
                Status = "Completed",
                States = new List<string> { "TN", "PY" },
                ApiRequests = 41
            };

            mockSyncService.Setup(s => s.SynchronizeAsync(null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedSummary);

            var function = new CourtMasterSyncFunction(mockSyncService.Object, mockLogger.Object);

            // Act
            await function.RunTimerAsync(null!, CancellationToken.None);

            // Assert
            mockSyncService.Verify(s => s.SynchronizeAsync(null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RunManualAsync_WithStateQuery_ShouldInvokeSynchronizeStateAsyncAndReturnOk()
        {
            // Arrange
            var mockSyncService = new Mock<ICourtMasterSyncService>();
            var mockLogger = new Mock<ILogger<CourtMasterSyncFunction>>();

            var expectedSummary = new CourtMasterSyncExecutionSummaryDto
            {
                Status = "Completed",
                States = new List<string> { "TN" },
                ApiRequests = 39
            };

            mockSyncService.Setup(s => s.SynchronizeStateAsync("TN", It.IsAny<CourtMasterSyncSettings>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedSummary);

            var context = new DefaultHttpContext();
            context.Request.Query = new QueryCollection(new Dictionary<string, StringValues>
            {
                { "state", "TN" },
                { "maxRequests", "50" }
            });

            var function = new CourtMasterSyncFunction(mockSyncService.Object, mockLogger.Object);

            // Act
            var actionResult = await function.RunManualAsync(context.Request, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            Assert.NotNull(okResult.Value);
            mockSyncService.Verify(s => s.SynchronizeStateAsync("TN", It.Is<CourtMasterSyncSettings>(opt => opt.MaxApiRequestsPerRun == 50), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
