using MiApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MiApp.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Subject> Subjects  => Set<Subject>();
    public DbSet<Professor> Professors  => Set<Professor>();
    public DbSet<Enrollment> Enrollments  => Set<Enrollment>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}