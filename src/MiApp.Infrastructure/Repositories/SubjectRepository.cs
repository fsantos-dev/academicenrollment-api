using MiApp.Domain.Entities;
using MiApp.Domain.Interfaces;
using MiApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MiApp.Infrastructure.Repositories;

public class SubjectRepository(AppDbContext context)
    : ISubjectRepository
{
    public async Task<IEnumerable<Subject>> GetAllAsync()
    {
        return await context.Subjects
            .Include(s => s.Professor)
            .ToListAsync();
    }

    public async Task<Subject?> GetByIdAsync(int id)
    {
        return await context.Subjects
            .Include(s => s.Professor)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
}