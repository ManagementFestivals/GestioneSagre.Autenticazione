using GestioneSagre.Autenticazione.BusinessLayer.Services;
using GestioneSagre.Autenticazione.Controllers.Common;
using GestioneSagre.SharedKernel.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestioneSagre.Autenticazione.Controllers;

public class AutenticazioneController : BaseController
{
    private readonly ILogger<AutenticazioneController> logger;
    private readonly IIdentityService identityService;

    public AutenticazioneController(ILogger<AutenticazioneController> logger, IIdentityService identityService)
    {
        this.logger = logger;
        this.identityService = identityService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginRequest request)
    {
        var response = await identityService.LoginAsync(request);

        if (response != null)
        {
            return Ok(response);
        }

        return BadRequest();
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var response = await identityService.RefreshTokenAsync(request);

        if (response != null)
        {
            return Ok(response);
        }

        return BadRequest();
    }

    //[AllowAnonymous]
    //[HttpPost("register")]
    //public async Task<IActionResult> RegisterAsync(RegisterRequest request)
    //{
    //    var response = await identityService.RegisterAsync(request);

    //    if (response.Succeeded)
    //    {
    //        return Ok(response);
    //    }

    //    return BadRequest(response);
    //}
}