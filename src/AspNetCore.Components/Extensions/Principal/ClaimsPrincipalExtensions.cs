using System.Security.Claims;

namespace AspNetCore.Components.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Int64 Id(this ClaimsPrincipal principal)
    {
        String? id = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        return id?.Length > 0 ? Int64.Parse(id, CultureInfo.InvariantCulture) : 0;
    }

    public static void UpdateClaim(this ClaimsPrincipal principal, String type, String value)
    {
        ClaimsIdentity? identity = (ClaimsIdentity?)principal.Identity;
        identity?.TryRemoveClaim(identity.FindFirst(type));
        identity?.AddClaim(new Claim(type, value));
    }
}
