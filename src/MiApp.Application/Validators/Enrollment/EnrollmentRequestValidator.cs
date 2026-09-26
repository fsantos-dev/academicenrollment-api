using FluentValidation;
using MiApp.Application.DTOs.Enrollment;

namespace MiApp.Application.Validators.Enrollment;

public class EnrollmentRequestValidator : AbstractValidator<EnrollmentRequestDto>
{
    public EnrollmentRequestValidator()
    {
        RuleFor(x => x.SubjectId)
            .GreaterThan(0).WithMessage("SubjectId must be greater than 0");

    }
}