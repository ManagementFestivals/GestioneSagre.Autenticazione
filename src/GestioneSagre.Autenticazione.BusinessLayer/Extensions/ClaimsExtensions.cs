using System.Security.Claims;
using System.Security.Principal;

namespace GestioneSagre.Autenticazione.BusinessLayer.Extensions;

public static class ClaimsExtensions
{
    public static Guid GetId(this IPrincipal user)
    {
        var value = user.GetClaimValue(ClaimTypes.NameIdentifier);
        return Guid.Parse(value);
    }

    public static string GetFirstName(this IPrincipal user) => user.GetClaimValue(ClaimTypes.GivenName);
    public static string GetLastName(this IPrincipal user) => user.GetClaimValue(ClaimTypes.Surname);
    public static string GetEmail(this IPrincipal user) => user.GetClaimValue(ClaimTypes.Email);

    public static IEnumerable<string> GetRoles(this IPrincipal user)
    {
        var values = ((ClaimsPrincipal)user).FindAll(ClaimTypes.Role).Select(c => c.Value);
        return values;
    }

    public static string GetClaimValue(this IPrincipal user, string claimType)
    {
        var value = ((ClaimsPrincipal)user).FindFirst(claimType)?.Value;
        return value;
    }
}