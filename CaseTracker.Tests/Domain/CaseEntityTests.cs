using System;
using System.Collections.Generic;
using CaseTrackerDomain.Models;
using Xunit;

namespace CaseTracker.Tests.Domain
{
    public class CaseEntityTests
    {
        [Fact]
        public void CaseEntity_Initialization_ShouldHaveDefaultValuesAndInitializedCollections()
        {
            // Arrange & Act
            var entity = new Case();

            // Assert
            Assert.Equal("Active", entity.Status);
            Assert.NotNull(entity.CaseClients);
            Assert.Empty(entity.CaseClients);
            Assert.NotNull(entity.CaseHearings);
            Assert.Empty(entity.CaseHearings);
            Assert.NotNull(entity.CaseOrders);
            Assert.Empty(entity.CaseOrders);
            Assert.NotNull(entity.CaseDocuments);
            Assert.Empty(entity.CaseDocuments);
            Assert.NotNull(entity.CaseLawyers);
            Assert.Empty(entity.CaseLawyers);
        }

        [Fact]
        public void CaseEntity_SetProperties_ShouldRetainValues()
        {
            // Arrange
            var caseId = Guid.NewGuid();
            var lawFirmId = Guid.NewGuid();
            var now = DateTimeOffset.UtcNow;

            // Act
            var entity = new Case
            {
                CaseId = caseId,
                LawFirmId = lawFirmId,
                CaseNumber = "WP/100/2026",
                CnrNumber = "TNHC010001232026",
                CaseType = "Writ Petition",
                CaseTitle = "Petitioner vs State",
                CaseStage = "Hearing",
                CaseStatus = "Pending",
                Status = "Active",
                CreatedAt = now,
                UpdatedAt = now,
                LastSyncedAt = now,
                SyncStatus = "Success"
            };

            // Assert
            Assert.Equal(caseId, entity.CaseId);
            Assert.Equal(lawFirmId, entity.LawFirmId);
            Assert.Equal("WP/100/2026", entity.CaseNumber);
            Assert.Equal("TNHC010001232026", entity.CnrNumber);
            Assert.Equal("Pending", entity.CaseStatus);
            Assert.Equal("Success", entity.SyncStatus);
        }
    }
}
