using GestioneSagre.Autenticazione.BusinessLayer.Authentication;
using GestioneSagre.Autenticazione.BusinessLayer.Options;
using GestioneSagre.Autenticazione.DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace GestioneSagre.Autenticazione.StartupTasks;

public class AuthenticationStartupTask : IHostedService
{
    private readonly IServiceProvider serviceProvider;
    private readonly IOptionsMonitor<AdminOptions> adminOptions;

    public AuthenticationStartupTask(IServiceProvider serviceProvider, IOptionsMonitor<AdminOptions> adminOptions)
    {
        this.serviceProvider = serviceProvider;
        this.adminOptions = adminOptions;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var roleNames = new string[] { RoleNames.Administrator, RoleNames.PowerUser, RoleNames.User };

        foreach (var roleName in roleNames)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await roleManager.CreateAsync(new ApplicationRole(roleName));
            }
        }

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var administratorUser = new ApplicationUser
        {
            UserName = adminOptions.CurrentValue.Email,
            Email = adminOptions.CurrentValue.Email,
            FirstName = adminOptions.CurrentValue.FirstName,
            LastName = adminOptions.CurrentValue.LastName,
            PasswordChangeDate = DateTime.UtcNow.AddDays(adminOptions.CurrentValue.PasswordChangeDate),
            EmailConfirmed = true
        };

        await CheckCreateUserAsync(administratorUser, adminOptions.CurrentValue.Password, RoleNames.Administrator);

        async Task CheckCreateUserAsync(ApplicationUser user, string password, params string[] roles)
        {
            var dbUser = await userManager.FindByEmailAsync(user.Email);
            if (dbUser == null)
            {
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRolesAsync(user, roles);
                }
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}