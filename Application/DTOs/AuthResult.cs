using System;

namespace Application.DTOs
{
    public class AuthResult
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
    }
}
