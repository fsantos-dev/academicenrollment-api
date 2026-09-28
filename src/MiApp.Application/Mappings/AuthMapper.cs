using MiApp.Application.DTOs.Auth;
using MiApp.Domain.Entities;

namespace MiApp.Application.Mappings;

public static class AuthMapper
{
    public static RegisterResponseDto ToRegisterResponseDto(Student student)
    {
        return new RegisterResponseDto
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName
        };
    }

    public static LoginResponseDto ToLoginResponseDto(Student student, string token, DateTime expiresAt)
    {
        return new LoginResponseDto
        {
            FirstName = student.FirstName,
            LastName = student.LastName,
            Email = student.Email,
            IsActive = student.IsActive,
            Token = token,
            ExpiresAt = expiresAt,
           
        };
    }

}