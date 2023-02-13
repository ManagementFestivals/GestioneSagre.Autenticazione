namespace GestioneSagre.Autenticazione.BusinessLayer.Options;

public class JwtOptions
{
    public string Issuer { get; init; }
    public string Audience { get; init; }
    public string SecurityKey { get; init; }
    public int AccessTokenExpirationMinutes { get; init; }
    public int RefreshTokenExpirationMinutes { get; init; }
}