using FluentValidation;
using MiApp.Application.DTOs.Enrollment;
using MiApp.Application.Interfaces;
using MiApp.Application.Mappings;
using MiApp.Domain.Entities;
using MiApp.Domain.Interfaces;

namespace MiApp.Application.Services.Enrollment;


public class EnrollmentService(
    IEnrollmentRepository enrollmentRepository,
    ISubjectRepository subjectRepository,
    ICurrentUserService currentUserService,
    IStudentRepository studentRepository,
    IValidator<EnrollmentRequestDto> enrollmentRequestValidator) : IEnrollmentService
{
    public async Task<EnrollmentResponseDto> CreateAsync(EnrollmentRequestDto request)
    {

        var validationResult = await enrollmentRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

        var studentId = currentUserService.UserId;
        var student = await studentRepository.GetByIdAsync(studentId);
        var subject = await subjectRepository.GetByIdAsync(request.SubjectId);
        if (student == null) throw new KeyNotFoundException("Student not found");
        if (subject == null) throw new KeyNotFoundException("Subject not found");
        student.EnrollInSubject(subject);
        var enrollment = student.Enrollments.Last();
        var created = await enrollmentRepository.CreateAsync(enrollment);
        return EnrollmentMapper.ToEnrollmentResponseDto(created, subject);

    }

    public async Task DeleteAsync(int id)
{
    var studentId = currentUserService.UserId;

    var enrollment = await enrollmentRepository.GetByIdAsync(id);

    if (enrollment == null)
        throw new KeyNotFoundException("Enrollment not found");

    if (enrollment.StudentId != studentId)
        throw new UnauthorizedAccessException(
            "You are not allowed to delete this enrollment");

    await enrollmentRepository.DeleteAsync(enrollment);
}

    public async Task<IEnumerable<EnrollmentResponseDto>> GetAllAsync()
    {
        var studentId = currentUserService.UserId;
        var enrollments = await enrollmentRepository.GetAllByStudentAsync(studentId);
        return enrollments.Select(enrollment => EnrollmentMapper.ToEnrollmentResponseDto(enrollment, enrollment.Subject));
    }

   
}