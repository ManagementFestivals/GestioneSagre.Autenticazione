using Microsoft.AspNetCore.Identity;

namespace GestioneSagre.Autenticazione.DataAccessLayer.Entities;

public class ApplicationUserRole : IdentityUserRole<Guid>
{
    public virtual ApplicationUser User { get; set; }
    public virtual ApplicationRole Role { get; set; }
}