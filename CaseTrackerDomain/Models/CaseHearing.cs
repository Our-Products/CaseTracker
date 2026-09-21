using System;
using System.Collections.Generic;

namespace CaseTrackerDomain.Models
{
    public class CaseHearing
    {
        public Guid HearingId { get; set; }

        public Guid CaseId { get; set; }

        public DateTime HearingDate { get; set; }

        public int? ItemNumber { get; set; }

        public string? CourtHall { get; set; }

        public string? JudgeName { get; set; }

        public string PurposeOfHearing { get; set; } = null!;

        public string? BusinessOnDate { get; set; }

        public DateTime? NextHearingDate { get; set; }

        public string? NextPurpose { get; set; }

        public string HearingStatus { get; set; } = "Scheduled";

        public string? DailyOrderSummary { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        // Navigation Properties
        public Case? Case { get; set; }

        public User? CreatedByUser { get; set; }

        public User? UpdatedByUser { get; set; }

        public ICollection<CaseOrder> CaseOrders { get; set; } = new List<CaseOrder>();

        public ICollection<CaseDocument> CaseDocuments { get; set; } = new List<CaseDocument>();
    }
}
