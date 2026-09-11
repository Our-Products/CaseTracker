using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user, IEnumerable<string> roles);

        int GetExpiryMinutes();
    }
}