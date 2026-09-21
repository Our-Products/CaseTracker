using System;

namespace CaseTrackerDomain.Models
{
    public class CourtMasterSyncHistory
    {
        public Guid Id { get; set; }

        public string State { get; set; } = null!;

        public DateTimeOffset StartedAt { get; set; }

        public DateTimeOffset CompletedAt { get; set; }

        public string Status { get; set; } = "Completed";

        public int RecordsRead { get; set; }

        public int RecordsInserted { get; set; }

        public int RecordsUpdated { get; set; }

        public int RecordsSkipped { get; set; }

        public int RecordsFailed { get; set; }

        public int ApiRequests { get; set; }

        public string? ErrorMessage { get; set; }

        public long DurationMs { get; set; }
    }
}
