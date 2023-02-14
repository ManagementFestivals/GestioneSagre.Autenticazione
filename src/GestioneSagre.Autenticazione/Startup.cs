using System.Text;
using GestioneSagre.Autenticazione.BusinessLayer.Authentication;
using GestioneSagre.Autenticazione.BusinessLayer.Options;
using GestioneSagre.Autenticazione.BusinessLayer.Requirements;
using GestioneSagre.Autenticazione.BusinessLayer.Services;
using GestioneSagre.Autenticazione.DataAccessLayer;
using GestioneSagre.Autenticazione.DataAccessLayer.Entities;
using GestioneSagre.Autenticazione.Services;
using GestioneSagre.Autenticazione.StartupTasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

namespace GestioneSagre.Autenticazione;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddMemoryCache();

        services.AddHttpContextAccessor();
        services.AddCors(options =>
        {
            options.AddPolicy("GestioneSagre.Autenticazione", policy =>
            {
                policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(config =>
        {
            config.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Gestione Sagre Autenticazione",
                Version = "v1"
            });

            config.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Insert the Bearer Token",
                Name = HeaderNames.Authorization,
                Type = SecuritySchemeType.ApiKey
            });

            config.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference= new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        services.AddDbContextPool<AuthDbContext>(optionsBuilder =>
        {
            var connectionString = Configuration.GetSection("ConnectionStrings").GetValue<string>("Default");

            optionsBuilder.UseSqlServer(connectionString, options =>
            {
                //Creazione migration: Add-Migration InitialMigration -Project GestioneSagre.Autenticazione.Migrations
                //Esecuzione migration: Update-Database
                options.MigrationsAssembly("GestioneSagre.Autenticazione.Migrations");

                // Abilito il connection resiliency per gestire le connessioni perse
                // Info su: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
                options.EnableRetryOnFailure(3);
            });
        });

        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
        })
        .AddEntityFrameworkStores<AuthDbContext>()
        .AddDefaultTokenProviders();

        var issuer = Configuration.GetSection("JwtSettings").GetValue<string>("Issuer");
        var audience = Configuration.GetSection("JwtSettings").GetValue<string>("Audience");
        var securityKey = Configuration.GetSection("JwtSettings").GetValue<string>("SecurityKey");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey)),
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddScoped<IAuthorizationHandler, UserActiveHandler>();

        services.AddAuthorization(options =>
        {
            var policyBuilder = new AuthorizationPolicyBuilder().RequireAuthenticatedUser();
            policyBuilder.Requirements.Add(new UserActiveRequirement());
            options.FallbackPolicy = options.DefaultPolicy = policyBuilder.Build();

            options.AddPolicy("Administrator", policy =>
            {
                policy.RequireRole(RoleNames.Administrator);
            });

            options.AddPolicy("PowerUser", policy =>
            {
                policy.RequireRole(RoleNames.PowerUser);
            });

            options.AddPolicy("User", policy =>
            {
                policy.RequireRole(RoleNames.User);
            });
        });

        // Uncomment if you want to use the old password hashing format for both login and registration.
        //services.Configure<PasswordHasherOptions>(options =>
        //{
        //    options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV2;
        //});

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IUserService, HttpUserService>();

        services.AddHostedService<AuthenticationStartupTask>();

        services.Configure<JwtOptions>(Configuration.GetSection("JwtSettings"));
        services.Configure<AdminOptions>(Configuration.GetSection("AdminSettings"));
    }

    public void Configure(WebApplication app)
    {
        IWebHostEnvironment env = app.Environment;

        //app.UseHttpsRedirection();
        app.UseCors("GestioneSagre.Autenticazione");

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Gestione Sagre Autenticazione v1");
        });

        app.UseRouting();
        app.UseAuthentication();

        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}