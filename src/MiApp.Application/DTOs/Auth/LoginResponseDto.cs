namespace MiApp.Application.DTOs.Auth;

public class LoginResponseDto
{
    public string FirstName {get; set;} = null!;
    public string LastName {get; set;} = null!;
    public string Email {get; set;} = null!;
    public string Token {get; set;} = null!;
    public DateTime ExpiresAt {get; set;}

}