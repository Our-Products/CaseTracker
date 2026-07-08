namespace Application.DTOs
{
    public class RegisterRequest
    {
        public string FullName { get; set; } = null!;
        public string MobileNumber { get; set; } = null!;
        public string? Email { get; set; }
        public string Password { get; set; } = null!;
    }
}
