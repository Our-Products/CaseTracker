using System;
using System.Collections.Generic;

namespace CaseTrackerDomain.Models
{
    public class Case
    {
        public Guid CaseId { get; set; }

        public Guid? LawFirmId { get; set; }

        public Guid CourtId { get; set; }

        public string? CnrNumber { get; set; }

        public string CaseNumber { get; set; } = null!;

        public string CaseType { get; set; } = null!;

        public string? FilingNumber { get; set; }

        public DateTime? FilingDate { get; set; }

        public string? RegistrationNumber { get; set; }

        public DateTime? RegistrationDate { get; set; }

        public string CaseTitle { get; set; } = null!;

        public string CaseStage { get; set; } = null!;

        public string CaseStatus { get; set; } = "Pending";

        public string? ActsSections { get; set; }

        public string? PoliceStation { get; set; }

        public string? FirNumber { get; set; }

        public int? FirYear { get; set; }

        public bool IsEcourtSynced { get; set; }

        public DateTimeOffset? LastSyncedAt { get; set; }

        public DateTimeOffset? LastSuccessfulSyncAt { get; set; }

        public DateTimeOffset? LastSyncAttemptAt { get; set; }

        public string? SyncStatus { get; set; }

        public string? SyncError { get; set; }

        public string Status { get; set; } = "Active";

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        // Navigation Properties
        public LawFirm? LawFirm { get; set; }

        public Court? Court { get; set; }

        public User? CreatedByUser { get; set; }

        public User? UpdatedByUser { get; set; }

        public ICollection<CaseClient> CaseClients { get; set; } = new List<CaseClient>();

        public ICollection<CaseLawyer> CaseLawyers { get; set; } = new List<CaseLawyer>();

        public ICollection<CaseHearing> CaseHearings { get; set; } = new List<CaseHearing>();

        public ICollection<CaseOrder> CaseOrders { get; set; } = new List<CaseOrder>();

        public ICollection<ECourtSyncLog> ECourtSyncLogs { get; set; } = new List<ECourtSyncLog>();

        public ICollection<CaseDocument> CaseDocuments { get; set; } = new List<CaseDocument>();
    }
}
