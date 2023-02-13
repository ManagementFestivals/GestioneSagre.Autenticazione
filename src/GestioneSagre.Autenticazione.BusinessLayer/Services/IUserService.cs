using System.Security.Claims;

namespace GestioneSagre.Autenticazione.BusinessLayer.Services;

public interface IUserService
{
    string GetUserName();
    public ClaimsIdentity GetIdentity();
}