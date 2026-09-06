using System;

namespace CaseTrackerDomain.Models
{
    public class Role
    {
        public string RoleId { get; set; } = null!;

        public string RoleName { get; set; } = null!;

        public string Status { get; set; } = null!;

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }


        // Navigation Properties

        public User? CreatedByUser { get; set; }

        public User? UpdatedByUser { get; set; }
    }
}
