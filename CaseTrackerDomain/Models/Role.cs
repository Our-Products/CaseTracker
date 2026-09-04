using System;

namespace CaseTrackerDomain.Models
{
    public class Role
    {
        // Role id is a string (e.g. "R001") per requirements
        public required string RoleId { get; set; }

        public string RoleName { get; set; } = null!;

        public string? Description { get; set; }

        public string Status { get; set; } = null!;

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}
