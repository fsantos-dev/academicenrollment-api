using MiApp.Application.DTOs.Auth;

namespace MiApp.Application.Interfaces;

public interface IAuthService
{
    Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request);
    //repositorio
    //1. validar si existe el email
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    //repositorio
    //1. validar si existe el email
    //2. crear el registro en DB
}