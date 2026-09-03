using System;

namespace CaseTrackerDomain.Models
{
    public class UserRole
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public string CreatedBy { get; set; } = null!;
        public string UpdatedBy { get; set; } = null!;

        // Navigation properties (optional)
        public User? User { get; set; }
        public Role? Role { get; set; }
    }
}
