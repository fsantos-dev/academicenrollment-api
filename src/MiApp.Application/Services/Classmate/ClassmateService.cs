using MiApp.Application.DTOs.Students;
using MiApp.Application.Interfaces;
using MiApp.Domain.Interfaces;

namespace MiApp.Application.Services;

public class ClassmateService(
    IEnrollmentRepository enrollmentRepository,
    ICurrentUserService currentUserService) : IClassmateService
{
    public async Task<IEnumerable<ClassmatesResponseDto>> GetAllAsync()
    {
        var studentId = currentUserService.UserId;

        var enrollments = await enrollmentRepository
            .GetClassmatesAsync(studentId);

        return enrollments
            .GroupBy(e => new
            {
                e.SubjectId,
                e.Subject.Name
            })
            .Select(group => new ClassmatesResponseDto
            {
                SubjectName = group.Key.Name,
                Classmates = group
                    .Select(e => $"{e.Student.FirstName} {e.Student.LastName}")
                    .Distinct()
                    .ToList()
            });
    }
}