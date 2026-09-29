using MiApp.Application.DTOs.Enrollment;

namespace MiApp.Application.Interfaces;

public interface IEnrollmentService{
    Task <IEnumerable<EnrollmentResponseDto>> GetAllAsync();
    //1. necesita consultar las materias
    //1. Consultar la lista inscripciones
    Task<EnrollmentResponseDto> CreateAsync(EnrollmentRequestDto request);
    //1. Registrar la inscripcion

    
    Task DeleteAsync(int id);
    //1. Localizar la inscripcion
    //2. Eliminar la inscripcion
}