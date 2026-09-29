using MiApp.Application.DTOs.Enrollment;
using MiApp.Domain.Entities;

namespace MiApp.Application.Mappings;

public static class EnrollmentMapper
{
    public static EnrollmentResponseDto ToEnrollmentResponseDto(Enrollment enrollment, Subject subject)
    {
        return new EnrollmentResponseDto
        {
            Id = enrollment.Id,
            SubjectId = subject.Id,
            SubjectName = subject.Name,
            Credits = subject.Credits,
            ProfessorId = subject.ProfessorId,
            ProfessorName = $"{subject.Professor.FirstName} {subject
            .Professor.LastName}" 
        };
    }
}