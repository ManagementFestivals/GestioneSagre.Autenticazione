using GestioneSagre.Autenticazione.Shared.Models;

namespace GestioneSagre.Autenticazione.BusinessLayer.Services;

public interface IIdentityService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<RegisterResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
}