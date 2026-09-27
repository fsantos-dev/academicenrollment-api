using Microsoft.AspNetCore.Mvc;
using MiApp.Application.DTOs.Auth;
using MiApp.Application.Interfaces;

namespace MiApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponseDto>> Register(
        RegisterRequestDto request)
    {
        var response = await authService.RegisterAsync(request);

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(
        LoginRequestDto request)
    {
        var response = await authService.LoginAsync(request);

        return Ok(response);
    }
}

