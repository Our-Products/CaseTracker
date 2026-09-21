using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CaseTrackerApplication.DTOs.Clients;
using CaseTrackerApplication.Exceptions;
using CaseTrackerApplication.Interfaces.Repositories;
using CaseTrackerApplication.Interfaces.Repositories.Cases;
using CaseTrackerApplication.Interfaces.Repositories.Clients;
using CaseTrackerApplication.Services.Clients;
using CaseTrackerDomain.Models;
using Moq;
using Xunit;

namespace CaseTracker.Tests.Application
{
    public class ClientServiceTests
    {
        [Fact]
        public async Task CreateClientAsync_ShouldPersistClientAndReturnDto()
        {
            // Arrange
            var mockClientRepo = new Mock<IClientRepository>();
            var mockCaseRepo = new Mock<ICaseRepository>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            var service = new ClientService(
                mockClientRepo.Object,
                mockCaseRepo.Object,
                mockUnitOfWork.Object);

            var request = new CreateClientRequest
            {
                FullName = "Ramesh Kumar",
                PrimaryPhone = "+919876543210",
                Email = "ramesh@example.com",
                ClientType = "Individual",
                City = "Chennai",
                State = "Tamil Nadu"
            };

            var userId = Guid.NewGuid();
            var lawFirmId = Guid.NewGuid();

            // Act
            var result = await service.CreateClientAsync(request, userId, lawFirmId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Ramesh Kumar", result.FullName);
            Assert.Equal("+919876543210", result.PrimaryPhone);
            Assert.Equal("Chennai", result.City);
            Assert.Equal(lawFirmId, result.LawFirmId);
            mockClientRepo.Verify(r => r.AddAsync(It.Is<Client>(c => c.FullName == "Ramesh Kumar")), Times.Once);
            mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetClientByIdAsync_WhenClientExists_ShouldReturnClientDto()
        {
            // Arrange
            var mockClientRepo = new Mock<IClientRepository>();
            var mockCaseRepo = new Mock<ICaseRepository>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            var clientId = Guid.NewGuid();
            var client = new Client
            {
                ClientId = clientId,
                FullName = "Suresh Raina",
                PrimaryPhone = "+919876500000",
                Status = "Active"
            };

            mockClientRepo.Setup(r => r.GetByIdAsync(clientId))
                .ReturnsAsync(client);

            var service = new ClientService(
                mockClientRepo.Object,
                mockCaseRepo.Object,
                mockUnitOfWork.Object);

            // Act
            var result = await service.GetClientByIdAsync(clientId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(clientId, result.ClientId);
            Assert.Equal("Suresh Raina", result.FullName);
        }

        [Fact]
        public async Task GetClientByIdAsync_WhenClientNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var mockClientRepo = new Mock<IClientRepository>();
            var mockCaseRepo = new Mock<ICaseRepository>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            var clientId = Guid.NewGuid();
            mockClientRepo.Setup(r => r.GetByIdAsync(clientId))
                .ReturnsAsync((Client?)null);

            var service = new ClientService(
                mockClientRepo.Object,
                mockCaseRepo.Object,
                mockUnitOfWork.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.GetClientByIdAsync(clientId));
        }
    }
}
