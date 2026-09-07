namespace CaseTrackerApplication.DTOs.Auth
{
    public class LoginRequest
    {
        public string MobileNumber { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
