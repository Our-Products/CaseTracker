using System;
using System.Collections.Generic;

namespace CaseTrackerDomain.Models
{
    public class State
    {
        public Guid StateId { get; set; }

        public string StateName { get; set; } = null!;

        public string StateCode { get; set; } = null!;

        public string Status { get; set; } = "Active";

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        // Navigation
        public User? CreatedByUser { get; set; }

        public User? UpdatedByUser { get; set; }

        public ICollection<District> Districts { get; set; } = new List<District>();
    }
}
