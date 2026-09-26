using FluentValidation;
using MiApp.Application.DTOs.Auth;

namespace MiApp.Application.Validators.Auth;

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be in a valid format");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}