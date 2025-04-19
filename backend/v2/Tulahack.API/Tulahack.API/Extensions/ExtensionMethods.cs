using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Tulahack.API.Models;
using Tulahack.API.Static;

namespace Tulahack.API.Extensions;

public static class ExtensionMethods
{
    public static Guid GetUserId(this ClaimsPrincipal claims)
    {
        Claim ssoUserClaim = claims.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier);
        var result = Guid.TryParse(ssoUserClaim.Value, out Guid userId);

        return result ? userId : default;
    }

    public static TulahackRole GetRole(this string token)
    {
        JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var claims = jwt.Claims
            .Where(claim => claim.Type == "group")
            .Select(claim => claim.Value)
            .ToList();

        if (claims.Count == 0)
        {
            return TulahackRole.Visitor;
        }

        // checking group membership from strongest to weakest
        if (claims.Contains(Groups.Superuser, StringComparer.InvariantCultureIgnoreCase))
        {
            return TulahackRole.Superuser;
        }

        if (claims.Contains(Groups.Managers, StringComparer.InvariantCultureIgnoreCase))
        {
            return TulahackRole.Manager;
        }

        return TulahackRole.Visitor;
    }

    public static string GetStoragePath(this Guid id, TulahackRole role) =>
        role switch
        {
            TulahackRole.Manager => Path.Combine(Directory.GetCurrentDirectory(), "appData", "storage", $"team_{id}"),
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null),
        };

    public static string GetStoragePath(this Guid id, TulahackRole role, FilePurposeType purpose, string filename) =>
        role switch
        {
            TulahackRole.Manager => Path.Combine(Directory.GetCurrentDirectory(), "appData", "storage", $"team_{id}",
                $"{purpose.ToString()}", filename),
            _ => string.Empty,
        };

    public static string GetStoragePath(this Guid id, TulahackRole role, FilePurposeType purpose) =>
        role switch
        {
            TulahackRole.Manager => Path.Combine(Directory.GetCurrentDirectory(), "appData", "storage", $"team_{id}",
                $"{purpose.ToString()}"),
            _ => string.Empty,
        };

    public static string GetStoragePath(this Guid id, TulahackRole role, string filename) =>
        role switch
        {
            TulahackRole.Manager => Path.Combine(Directory.GetCurrentDirectory(), "appData", "storage", $"team_{id}", filename),
            _ => string.Empty,
        };
}
