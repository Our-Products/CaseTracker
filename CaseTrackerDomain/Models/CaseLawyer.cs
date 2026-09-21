using System;

namespace CaseTrackerDomain.Models
{
    public class CaseLawyer
    {
        public Guid CaseId { get; set; }

        public Guid LawyerId { get; set; }

        public string LawyerRoleId { get; set; } = null!;

        public string Status { get; set; } = "Active";

        public DateTimeOffset AssignedAt { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        // Navigation
        public Case? Case { get; set; }

        public Lawyer? Lawyer { get; set; }

        public CaseLawyerRole? LawyerRole { get; set; }

        public User? CreatedByUser { get; set; }

        public User? UpdatedByUser { get; set; }
    }
}
