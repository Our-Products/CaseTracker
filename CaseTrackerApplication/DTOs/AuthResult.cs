using System;

namespace CaseTrackerApplication.DTOs
{
    public class AuthResult
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
    }
}
