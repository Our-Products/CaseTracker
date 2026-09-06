using System;

namespace CaseTrackerDomain.Models
{
    public class UserRole
    {
        public Guid UserId { get; set; }

        public string RoleId { get; set; } = null!;

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }


        // Navigation Properties

        public User? User { get; set; }

        public Role? Role { get; set; }

        public User? CreatedByUser { get; set; }

        public User? UpdatedByUser { get; set; }
    }
}
