using System.Security.Claims;

namespace FinFlow.WebApi.Extensions;

public static class ClaimsExtensions
{
    public static string? GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirst("sub")?.Value ?? 
               user.FindFirst("external_id")?.Value ??
               user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public static string? GetFirstName(this ClaimsPrincipal user)
    {
        return user.FindFirst("given_name")?.Value ?? 
               user.FindFirst("first_name")?.Value ??
               user.FindFirst(ClaimTypes.GivenName)?.Value;
    }

    public static string? GetLastName(this ClaimsPrincipal user)
    {
        return user.FindFirst("family_name")?.Value ?? 
               user.FindFirst("last_name")?.Value ??
               user.FindFirst(ClaimTypes.Surname)?.Value;
    }
}