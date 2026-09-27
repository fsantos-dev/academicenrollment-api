using MiApp.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MiApp.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor  httpContextAccessor) : ICurrentUserService
{
    public int UserId  {
        get{
            var userClaim = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if(userClaim == null) throw new UnauthorizedAccessException();
            if(!int.TryParse(userClaim.Value, out var UserId)) throw new UnauthorizedAccessException();
            return UserId;
        }
    }
}

