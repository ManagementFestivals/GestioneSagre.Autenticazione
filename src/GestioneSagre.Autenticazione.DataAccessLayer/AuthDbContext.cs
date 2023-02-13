using GestioneSagre.Autenticazione.DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GestioneSagre.Autenticazione.DataAccessLayer;

public class AuthDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid, IdentityUserClaim<Guid>,
    ApplicationUserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    public AuthDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(user =>
        {
            user.Property(u => u.FirstName);
            user.Property(u => u.LastName);
        });

        builder.Entity<ApplicationUserRole>(userRole =>
        {
            userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

            userRole.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles).HasForeignKey(ur => ur.RoleId).IsRequired();

            userRole.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles).HasForeignKey(ur => ur.UserId).IsRequired();
        });
    }
}