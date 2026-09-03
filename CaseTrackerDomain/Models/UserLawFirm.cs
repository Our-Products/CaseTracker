using System;

namespace CaseTrackerDomain.Models
{
    public class UserLawFirm
    {
        public Guid UserId { get; set; }
        public Guid LawFirmId { get; set; }

        public DateTimeOffset JoinedAt { get; set; }

        public string Status { get; set; } = "Active";

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        // Navigation
        public User? User { get; set; }
        public LawFirm? LawFirm { get; set; }
    }
}
