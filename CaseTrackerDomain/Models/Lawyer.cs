using System;

namespace CaseTrackerDomain.Models
{
    public class Lawyer
    {
        public Guid LawyerId { get; set; }

        public Guid UserId { get; set; }

        public Guid? LawFirmId { get; set; }

        public string FullName { get; set; } = null!;

        public string? BarCouncilId { get; set; }

        public string? BarCouncilName { get; set; }

        public DateTime? EnrollmentDate { get; set; }

        public string Status { get; set; } = "Active";

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public LawFirm? LawFirm { get; set; }
    }
}
