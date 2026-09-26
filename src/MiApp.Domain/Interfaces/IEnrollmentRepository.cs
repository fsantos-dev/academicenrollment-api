using MiApp.Domain.Entities;

namespace MiApp.Domain.Interfaces;

public interface IEnrollmentRepository
{
    Task<IEnumerable<Enrollment>> GetAllByStudentAsync(int studentId);
    Task<Enrollment?> GetByIdAsync(int id);
    Task<Enrollment> CreateAsync(Enrollment enrollment);
    Task UpdateAsync (Enrollment enrollment);
    Task DeleteAsync(Enrollment enrollment);

}