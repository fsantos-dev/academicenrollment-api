using MiApp.Application.DTOs.Enrollment;

namespace MiApp.Application.Interfaces;

public interface IEnrollmentService{
    Task <IEnumerable<EnrollmentResponseDto>> GetAsync();
    Task<EnrollmentResponseDto> CreateAsync(EnrollmentRequestDto request);
    Task<EnrollmentResponseDto> UpdateAsync(int id, EnrollmentRequestDto request);
    Task DeleteAsync(int id);
}