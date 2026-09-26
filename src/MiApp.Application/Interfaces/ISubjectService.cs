using MiApp.Application.DTOs;

namespace MiApp.Application.Interfaces;

public interface ISubjectService
{
    Task<IEnumerable<SubjectResponseDto>> GetAllAsync();
}