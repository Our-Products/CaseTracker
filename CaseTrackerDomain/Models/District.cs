using System;
using System.Collections.Generic;

namespace CaseTrackerDomain.Models
{
    public class District
    {
        public Guid DistrictId { get; set; }

        public string DistrictName { get; set; } = null!;

        public Guid StateId { get; set; }

        public string? DistrictCode { get; set; }

        public string Status { get; set; } = "Active";

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        // Navigation
        public State? State { get; set; }

        public User? CreatedByUser { get; set; }

        public User? UpdatedByUser { get; set; }

        public ICollection<CourtComplex> CourtComplexes { get; set; } = new List<CourtComplex>();
    }
}
