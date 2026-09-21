using System;

namespace CaseTrackerDomain.Models
{
    public class CaseOrder
    {
        public Guid OrderId { get; set; }

        public Guid CaseId { get; set; }

        public Guid? HearingId { get; set; }

        public DateTime OrderDate { get; set; }

        public string OrderType { get; set; } = null!;

        public string? OrderUrl { get; set; }

        public string? PdfStoragePath { get; set; }

        public bool IsCertified { get; set; } = true;

        public string? OrderMarkdownContent { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        // Navigation
        public Case? Case { get; set; }

        public CaseHearing? Hearing { get; set; }

        public User? CreatedByUser { get; set; }

        public User? UpdatedByUser { get; set; }
    }
}
