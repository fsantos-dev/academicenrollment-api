using MiApp.Domain.Entities;
using MiApp.Domain.Interfaces;
using MiApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MiApp.Infrastructure.Repositories;

public class EnrollmentRepository(AppDbContext context)
    : IEnrollmentRepository
{
    public async Task<IEnumerable<Enrollment>> GetAllByStudentAsync(
        int studentId)
    {
        return await context.Enrollments
            .Where(e => e.StudentId == studentId)
            .Include(e => e.Subject)
                .ThenInclude(s => s.Professor)
            .ToListAsync();
    }

    public async Task<IEnumerable<Enrollment>> GetClassmatesAsync(
        int studentId)
    {
        var subjectIds = await context.Enrollments
            .Where(e => e.StudentId == studentId)
            .Select(e => e.SubjectId)
            .ToListAsync();

        return await context.Enrollments
            .Where(e =>
                subjectIds.Contains(e.SubjectId) &&
                e.StudentId != studentId)
            .Include(e => e.Student)
            .Include(e => e.Subject)
            .ThenInclude(s => s.Professor)
            .ToListAsync();
    }

    public async Task<Enrollment?> GetByIdAsync(int id)
    {
        return await context.Enrollments
            .Include(e => e.Subject)
                .ThenInclude(s => s.Professor)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Enrollment> CreateAsync(Enrollment enrollment)
    {
        await context.Enrollments.AddAsync(enrollment);
        await context.SaveChangesAsync();

        return enrollment;
    }

    public async Task UpdateAsync(Enrollment enrollment)
    {
        context.Enrollments.Update(enrollment);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Enrollment enrollment)
    {
        context.Enrollments.Remove(enrollment);
        await context.SaveChangesAsync();
    }
}