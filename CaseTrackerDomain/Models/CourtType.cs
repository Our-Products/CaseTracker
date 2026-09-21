using System;
using System.Collections.Generic;

namespace CaseTrackerDomain.Models
{
    public class CourtType
    {
        public string CourtTypeId { get; set; } = null!;

        public string CourtTypeName { get; set; } = null!;

        public string Status { get; set; } = "Active";

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        // Navigation
        public User? CreatedByUser { get; set; }

        public User? UpdatedByUser { get; set; }

        public ICollection<Court> Courts { get; set; } = new List<Court>();
    }
}
