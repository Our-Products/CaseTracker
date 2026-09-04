using System;

namespace CaseTrackerApplication.DTOs
{
    public class AuthResult
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }

        // Optional IDs returned after registration
        public Guid? UserId { get; set; }
        public Guid? LawFirmId { get; set; }
    }
}
