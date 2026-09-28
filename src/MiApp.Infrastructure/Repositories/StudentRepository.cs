using MiApp.Domain.Entities;
using MiApp.Domain.Interfaces;
using MiApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MiApp.Infrastructure.Repositories;


public class StudentRepository(AppDbContext context) : IStudentRepository
{
    public async Task<Student?> GetByEmailAsync(string email)
    {
        return await context.Students
            .FirstOrDefaultAsync(s => s.Email == email);
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        return await context.Students
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.Subject)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Student> CreateAsync(Student student)
    {
        await context.Students.AddAsync(student);
        await context.SaveChangesAsync();
        return student;
    }

}