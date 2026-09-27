using System.Security.Authentication;
using FluentValidation;
using MiApp.Application.DTOs.Auth;
using MiApp.Application.Interfaces;
using MiApp.Application.Mappings;
using MiApp.Domain.Entities;
using MiApp.Domain.Interfaces;
using MiApp.Domain.Exceptions;

namespace MiApp.Application.Services.Auth;

public class AuthService(
    IStudentRepository studentRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IValidator<LoginRequestDto> loginRequestValidator,
    IValidator<RegisterRequestDto> registerRequestValidator
) : IAuthService
{
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var validationResult = await loginRequestValidator.ValidateAsync(request);
        if(!validationResult.IsValid) throw new ValidationException(validationResult.Errors);
        var user = await studentRepository.GetByEmailAsync(request.Email);
        if(user == null) throw new InvalidCredentialsException("invalid credentials");
        if(!passwordHasher.Verify(request.Password, user.PasswordHash)) throw new InvalidCredentialException("invalid credentials");
        
        var token = tokenService.GenerateToken(user);

        var response = AuthMapper.ToLoginResponseDto(user, token.Token, token.ExpiresAt);

        return response;
    }

    public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var validationResult = await registerRequestValidator.ValidateAsync(request);
        if(!validationResult.IsValid) throw new ValidationException(validationResult.Errors);
         var existing = await studentRepository.GetByEmailAsync(request.Email);
        if(existing != null) throw new InvalidOperationException($"The email {request.Email} is already in use");
        
        var student = new Student(
            request.FirstName, 
            request.LastName, 
            request.Email, 
            passwordHasher.Hash(request.Password)
        );

        var created = await studentRepository.CreateAsync(student);
        var response = AuthMapper.ToRegisterResponseDto(created);
        return response;
    }
}