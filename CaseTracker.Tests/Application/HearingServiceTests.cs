using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CaseTrackerApplication.DTOs.Hearings;
using CaseTrackerApplication.Exceptions;
using CaseTrackerApplication.Interfaces.Repositories;
using CaseTrackerApplication.Interfaces.Repositories.Hearings;
using CaseTrackerApplication.Services.Hearings;
using CaseTrackerDomain.Models;
using Moq;
using Xunit;

namespace CaseTracker.Tests.Application
{
    public class HearingServiceTests
    {
        [Fact]
        public async Task CreateHearingAsync_ShouldPersistHearingAndReturnDto()
        {
            // Arrange
            var mockHearingRepo = new Mock<IHearingRepository>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            var service = new HearingService(mockHearingRepo.Object, mockUnitOfWork.Object);

            var caseId = Guid.NewGuid();
            var hearingDate = DateTime.UtcNow.AddDays(7);

            var request = new CreateHearingRequest
            {
                CaseId = caseId,
                HearingDate = hearingDate,
                JudgeName = "Hon. Justice Anand",
                CourtHall = "Court 3",
                PurposeOfHearing = "Cross Examination"
            };

            var userId = Guid.NewGuid();

            // Act
            var result = await service.CreateHearingAsync(request, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(caseId, result.CaseId);
            Assert.Equal("Hon. Justice Anand", result.JudgeName);
            Assert.Equal("Cross Examination", result.PurposeOfHearing);
            mockHearingRepo.Verify(r => r.AddAsync(It.Is<CaseHearing>(h => h.CaseId == caseId)), Times.Once);
            mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetHearingByIdAsync_WhenHearingNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var mockHearingRepo = new Mock<IHearingRepository>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            var hearingId = Guid.NewGuid();
            mockHearingRepo.Setup(r => r.GetByIdAsync(hearingId))
                .ReturnsAsync((CaseHearing?)null);

            var service = new HearingService(mockHearingRepo.Object, mockUnitOfWork.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.GetHearingByIdAsync(hearingId));
        }
    }
}
