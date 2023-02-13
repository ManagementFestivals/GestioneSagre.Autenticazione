namespace GestioneSagre.Autenticazione.Shared.Models;

public class AuthResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public bool RequireChangePassword { get; set; }
}