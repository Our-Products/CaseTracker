using System;
using System.Collections.Generic;

namespace CaseTrackerDomain.Models
{
    public class CourtComplex
    {
        public Guid CourtComplexId { get; set; }

        public string ComplexName { get; set; } = null!;

        public Guid DistrictId { get; set; }

        public string? ComplexCode { get; set; }

        public string Status { get; set; } = "Active";

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        // Navigation
        public District? District { get; set; }

        public User? CreatedByUser { get; set; }

        public User? UpdatedByUser { get; set; }

        public ICollection<Court> Courts { get; set; } = new List<Court>();
    }
}
