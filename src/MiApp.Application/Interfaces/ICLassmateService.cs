using MiApp.Application.DTOs.Students;

namespace MiApp.Application.Interfaces;


public interface IClassmateService
{
    Task <IEnumerable<ClassmatesResponseDto>> GetAllAsync();
}