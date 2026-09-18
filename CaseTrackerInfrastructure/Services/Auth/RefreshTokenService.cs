using System.Security.Cryptography;
using System.Text;
using CaseTrackerApplication.Interfaces.Services.Auth;
using CaseTrackerDomain.Models;
using CaseTrackerInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CaseTrackerInfrastructure.Services.Auth;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(string RefreshToken, RefreshToken RefreshTokenEntity)> CreateAsync(Guid userId)
    {
        var refreshToken = Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64));

        var tokenHash = HashToken(refreshToken);

        var entity = new RefreshToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        _context.RefreshTokens.Add(entity);

        await _context.SaveChangesAsync();

        return (refreshToken, entity);
    }

    public async Task<RefreshToken?> GetValidTokenAsync(string refreshToken)
    {
        var tokenHash = HashToken(refreshToken);

        return await _context.RefreshTokens
            .FirstOrDefaultAsync(x =>
                x.TokenHash == tokenHash &&
                x.RevokedAt == null &&
                x.ExpiresAt > DateTime.UtcNow);
    }

    public async Task RevokeAsync(RefreshToken refreshToken)
    {
        refreshToken.RevokedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(
            Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(bytes);
    }
}