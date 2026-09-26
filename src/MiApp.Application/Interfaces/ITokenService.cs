using MiApp.Domain.Entities;

namespace MiApp.Application.Interfaces;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(Student student);
}