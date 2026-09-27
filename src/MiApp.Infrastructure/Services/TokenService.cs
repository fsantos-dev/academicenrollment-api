using System.Security.Claims;
using MiApp.Application.Interfaces;
using MiApp.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;


namespace MiApp.Infrastructure.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public (string Token, DateTime ExpiresAt) GenerateToken(Student student)
    {
        var secretKey = configuration["Jwt:Key"]!;
        var issuer = configuration["Jwt:Issuer"]!;
        var audience = configuration["Jwt:Audience"]!;
        var expiryMinutes = int.Parse(configuration["Jwt:ExpiryMinutes"] ?? "15") ;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, student.Id.ToString()),
        };

        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience : audience,
            claims : claims,
            expires: expiresAt,
            signingCredentials: credenciales
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return (tokenString, expiresAt);
        
    }
}