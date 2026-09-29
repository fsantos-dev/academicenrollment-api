using MiApp.Application.DTOs;
using MiApp.Domain.Entities;

namespace MiApp.Application.Mappings;

public static class SubjectMapper
{
    public static SubjectResponseDto ToSubjectResponseDto(Subject subject)
    {
        return new SubjectResponseDto
        {
            Id = subject.Id,
            Name = subject.Name,
            Credits = subject.Credits,
            ProfessorId = subject.ProfessorId,
            ProfessorName = $"{subject.Professor.FirstName} {subject.Professor.LastName}"
        };
    }

}