using System;
using CaseTrackerDomain.Models;
using Xunit;

namespace CaseTracker.Tests.Domain
{
    public class ClientEntityTests
    {
        [Fact]
        public void ClientEntity_Initialization_ShouldHaveDefaultValues()
        {
            // Arrange & Act
            var client = new Client();

            // Assert
            Assert.Equal("Active", client.Status);
            Assert.NotNull(client.CaseClients);
            Assert.Empty(client.CaseClients);
        }

        [Fact]
        public void CaseClient_ShouldHoldCorrectRelationshipsAndPartyType()
        {
            // Arrange
            var caseId = Guid.NewGuid();
            var clientId = Guid.NewGuid();

            // Act
            var link = new CaseClient
            {
                CaseId = caseId,
                ClientId = clientId,
                PartyType = "Petitioner",
                IsPrimary = true
            };

            // Assert
            Assert.Equal(caseId, link.CaseId);
            Assert.Equal(clientId, link.ClientId);
            Assert.Equal("Petitioner", link.PartyType);
            Assert.True(link.IsPrimary);
        }
    }
}
