using System;
using System.Collections.Generic;

namespace CaseTrackerDomain.Models
{
    public class Court
    {
        public Guid CourtId { get; set; }

        public string CourtName { get; set; } = null!;

        public string? CourtCode { get; set; }

        public Guid CourtComplexId { get; set; }

        public string CourtTypeId { get; set; } = null!;

        public string Status { get; set; } = "Active";

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        // Navigation
        public CourtComplex? CourtComplex { get; set; }

        public CourtType? CourtType { get; set; }

        public User? CreatedByUser { get; set; }

        public User? UpdatedByUser { get; set; }

        public ICollection<Case> Cases { get; set; } = new List<Case>();
    }
}
