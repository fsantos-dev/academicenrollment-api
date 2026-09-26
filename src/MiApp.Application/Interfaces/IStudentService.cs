using MiApp.Application.DTOs.Students;

namespace MiApp.Application.Interfaces;


public interface IStudentService
{
    Task <IEnumerable<ClassmatesResponseDto>> GetAsync();
}