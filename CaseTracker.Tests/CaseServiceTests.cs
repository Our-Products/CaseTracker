using System;
using System.Threading.Tasks;
using CaseTrackerApplication.DTOs.Cases;
using CaseTrackerApplication.Exceptions;
using CaseTrackerApplication.Interfaces.Repositories;
using CaseTrackerApplication.Interfaces.Repositories.Cases;
using CaseTrackerApplication.Interfaces.Repositories.Hearings;
using CaseTrackerApplication.Interfaces.Services.ECourts;
using CaseTrackerApplication.Services.Cases;
using CaseTrackerDomain.Models;
using Moq;
using Xunit;

namespace CaseTracker.Tests
{
    public class CaseServiceTests
    {
        [Fact]
        public async Task CreateCaseAsync_WhenCnrIsUnique_ShouldCreateAndReturnCaseDto()
        {
            // Arrange
            var mockCaseRepo = new Mock<ICaseRepository>();
            var mockHearingRepo = new Mock<IHearingRepository>();
            var mockECourtClient = new Mock<IECourtClient>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            mockCaseRepo.Setup(r => r.GetByCnrAsync("TNHC010001232026"))
                .ReturnsAsync((Case?)null);

            var service = new CaseService(
                mockCaseRepo.Object,
                mockHearingRepo.Object,
                mockECourtClient.Object,
                mockUnitOfWork.Object);

            var request = new CreateCaseRequest
            {
                CaseNumber = "WP/1001/2026",
                CaseType = "Writ Petition",
                CaseTitle = "ABC vs State of TN",
                CaseStage = "Admission",
                CnrNumber = "TNHC010001232026"
            };

            var userId = Guid.NewGuid();
            var lawFirmId = Guid.NewGuid();

            // Act
            var result = await service.CreateCaseAsync(request, userId, lawFirmId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("WP/1001/2026", result.CaseNumber);
            Assert.Equal("TNHC010001232026", result.CnrNumber);
            Assert.Equal(lawFirmId, result.LawFirmId);
            mockCaseRepo.Verify(r => r.AddAsync(It.Is<Case>(c => c.CaseNumber == "WP/1001/2026")), Times.Once);
            mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateCaseAsync_WhenCnrAlreadyExists_ShouldThrowConflictException()
        {
            // Arrange
            var mockCaseRepo = new Mock<ICaseRepository>();
            var mockHearingRepo = new Mock<IHearingRepository>();
            var mockECourtClient = new Mock<IECourtClient>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            mockCaseRepo.Setup(r => r.GetByCnrAsync("TNHC010001232026"))
                .ReturnsAsync(new Case { CaseId = Guid.NewGuid(), CnrNumber = "TNHC010001232026" });

            var service = new CaseService(
                mockCaseRepo.Object,
                mockHearingRepo.Object,
                mockECourtClient.Object,
                mockUnitOfWork.Object);

            var request = new CreateCaseRequest
            {
                CaseNumber = "WP/1001/2026",
                CaseType = "Writ Petition",
                CaseTitle = "ABC vs State of TN",
                CaseStage = "Admission",
                CnrNumber = "TNHC010001232026"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() =>
                service.CreateCaseAsync(request, Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async Task GetCaseByIdAsync_WhenCaseDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var mockCaseRepo = new Mock<ICaseRepository>();
            var mockHearingRepo = new Mock<IHearingRepository>();
            var mockECourtClient = new Mock<IECourtClient>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            var caseId = Guid.NewGuid();
            mockCaseRepo.Setup(r => r.GetWithDetailsAsync(caseId))
                .ReturnsAsync((Case?)null);

            var service = new CaseService(
                mockCaseRepo.Object,
                mockHearingRepo.Object,
                mockECourtClient.Object,
                mockUnitOfWork.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                service.GetCaseByIdAsync(caseId));
        }
    }
}
