using Microsoft.AspNetCore.Identity;

namespace GestioneSagre.Autenticazione.DataAccessLayer.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string RefreshToken { get; set; }
    public DateTime? RefreshTokenExpirationDate { get; set; }
    public DateTime PasswordChangeDate { get; set; }
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }

    public bool NotifyChangePassword
    {
        get
        {
            if (PasswordChangeDate == DateTime.MinValue)
            {
                return false;
            }

            return (DateTime.UtcNow - PasswordChangeDate).TotalDays >= 90;
        }
    }
}