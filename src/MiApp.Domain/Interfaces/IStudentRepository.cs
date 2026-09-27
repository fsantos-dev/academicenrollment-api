using MiApp.Domain.Entities;

namespace MiApp.Domain.Interfaces;


public interface IStudentRepository
{
    Task<Student?> GetByEmailAsync(string email);
    Task<Student> CreateAsync(Student student);
    Task<Student?> GetByIdAsync(int id);
}