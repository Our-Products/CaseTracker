using System;

namespace CaseTrackerDomain.Models
{
    public class CaseClient
    {
        public Guid CaseId { get; set; }

        public Guid ClientId { get; set; }

        public string PartyType { get; set; } = "Petitioner";

        public int PartySequence { get; set; } = 1;

        public bool IsPrimary { get; set; } = true;

        public string Status { get; set; } = "Active";

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        // Navigation Properties
        public Case? Case { get; set; }

        public Client? Client { get; set; }

        public User? CreatedByUser { get; set; }

        public User? UpdatedByUser { get; set; }
    }
}
