namespace MiApp.Application.DTOs.Auth;

public class RegisterResponseDto
{
    public int Id {get; set;}
    public string FirstName {get; set;} = null!;
    public string LastName {get; set;} = null!;
}