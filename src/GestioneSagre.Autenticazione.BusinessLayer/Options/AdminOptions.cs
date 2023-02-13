namespace GestioneSagre.Autenticazione.BusinessLayer.Options;

public class AdminOptions
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public string Password { get; init; }
    public int PasswordChangeDate { get; init; }
}
