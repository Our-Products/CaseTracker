using CaseTracker.Constants;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CaseTracker.Extensions
{
    /// <summary>
    /// Extension methods for ClaimsPrincipal to simplify user identification and ownership checks.
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Retrieves the current authenticated user's Guid ID from claims.
        /// </summary>
        public static Guid? GetUserId(this ClaimsPrincipal principal)
        {
            var claim = principal.FindFirst(ClaimTypes.NameIdentifier)
                     ?? principal.FindFirst(JwtRegisteredClaimNames.Sub)
                     ?? principal.FindFirst("sub");

            if (claim != null && Guid.TryParse(claim.Value, out var userId))
            {
                return userId;
            }

            return null;
        }

        /// <summary>
        /// Retrieves all assigned roles for the authenticated user.
        /// </summary>
        public static IEnumerable<string> GetRoles(this ClaimsPrincipal principal)
        {
            return principal.FindAll(ClaimTypes.Role)
                .Concat(principal.FindAll("role"))
                .Select(c => c.Value)
                .Distinct();
        }

        /// <summary>
        /// Checks if the authenticated user has the SuperAdmin (Company level) role.
        /// </summary>
        public static bool IsSuperAdmin(this ClaimsPrincipal principal)
        {
            return principal.IsInRole(AppRoles.SuperAdmin);
        }

        /// <summary>
        /// Checks if the authenticated user has the Admin (Firm level) role.
        /// </summary>
        public static bool IsFirmAdmin(this ClaimsPrincipal principal)
        {
            return principal.IsInRole(AppRoles.Admin);
        }

        /// <summary>
        /// Checks if the authenticated user has either SuperAdmin (Company) or Admin (Firm) role.
        /// </summary>
        public static bool IsAdmin(this ClaimsPrincipal principal)
        {
            return principal.IsInRole(AppRoles.SuperAdmin) || principal.IsInRole(AppRoles.Admin);
        }

        /// <summary>
        /// Checks if the authenticated user has the Lawyer role.
        /// </summary>
        public static bool IsLawyer(this ClaimsPrincipal principal)
        {
            return principal.IsInRole(AppRoles.Lawyer);
        }

        /// <summary>
        /// Determines if the caller has permission to access or modify a specific user's resource.
        /// Allowed if the caller is a SuperAdmin, Firm Admin, or if the caller's ID matches targetUserId.
        /// </summary>
        public static bool CanAccessUser(this ClaimsPrincipal principal, Guid targetUserId)
        {
            if (principal.IsAdmin())
            {
                return true;
            }

            var currentUserId = principal.GetUserId();
            return currentUserId.HasValue && currentUserId.Value == targetUserId;
        }
    }
}
