using System;

namespace CaseTrackerDomain.Models
{
    public class ECourtSyncLog
    {
        public Guid SyncId { get; set; }

        public Guid CaseId { get; set; }

        public string CnrNumber { get; set; } = null!;

        public string SyncStatus { get; set; } = null!;

        public string SyncSource { get; set; } = "AutoCron";

        public int HearingsUpdatedCount { get; set; }

        public int OrdersDownloadedCount { get; set; }

        public string? RawPayload { get; set; }

        public string? ErrorDetails { get; set; }

        public DateTimeOffset SyncedAt { get; set; }

        public Guid? TriggeredBy { get; set; }

        // Navigation
        public Case? Case { get; set; }

        public User? TriggeredByUser { get; set; }
    }
}
