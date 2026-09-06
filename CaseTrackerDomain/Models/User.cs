using System;

namespace CaseTrackerDomain.Models
{
    public class User
    {
        public Guid UserId { get; set; }

        public string MobileNumber { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Status { get; set; } = null!;

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        // Navigation properties
        public User? CreatedByUser { get; set; }

        public User? UpdatedByUser { get; set; }
    }
}
