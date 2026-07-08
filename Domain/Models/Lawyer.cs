using System;

namespace Domain.Models
{
    public class Lawyer
    {
        public Guid LawyerId { get; set; }

        public string FullName { get; set; } = null!;

        public string MobileNumber { get; set; } = null!;

        public string? Email { get; set; }

        public string PasswordHash { get; set; } = null!;

        public string Status { get; set; } = "active";

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
