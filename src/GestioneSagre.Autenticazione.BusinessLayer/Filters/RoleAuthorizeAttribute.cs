using Microsoft.AspNetCore.Authorization;

namespace GestioneSagre.Autenticazione.BusinessLayer.Filters;

public class RoleAuthorizeAttribute : AuthorizeAttribute
{
    public RoleAuthorizeAttribute(params string[] roles)
    {
        Roles = string.Join(",", roles);
    }
}