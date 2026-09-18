using CaseTrackerDomain.Models;

namespace CaseTrackerApplication.Interfaces.Services.Auth;

public interface IRefreshTokenService
{
    Task<(string RefreshToken, RefreshToken RefreshTokenEntity)> CreateAsync(Guid userId);

    Task<RefreshToken?> GetValidTokenAsync(string refreshToken);

    Task RevokeAsync(RefreshToken refreshToken);
} 