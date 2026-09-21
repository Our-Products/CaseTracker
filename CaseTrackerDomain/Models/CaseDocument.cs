using System;

namespace CaseTrackerDomain.Models
{
    public class CaseDocument
    {
        public Guid DocumentId { get; set; }

        public Guid CaseId { get; set; }

        public Guid? HearingId { get; set; }

        public string Category { get; set; } = null!;

        public string DocumentTitle { get; set; } = null!;

        public string FileName { get; set; } = null!;

        public string FileUrl { get; set; } = null!;

        public string MimeType { get; set; } = null!;

        public long FileSizeBytes { get; set; }

        public string Status { get; set; } = "Active";

        public DateTimeOffset UploadedAt { get; set; }

        public Guid? UploadedBy { get; set; }

        // Navigation
        public Case? Case { get; set; }

        public CaseHearing? Hearing { get; set; }

        public User? UploadedByUser { get; set; }
    }
}
