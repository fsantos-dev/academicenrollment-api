using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using MiApp.Application.DTOs.Enrollment;
using MiApp.Application.Interfaces;
using MiApp.Application.Services.Enrollment;
using MiApp.Domain.Entities;
using MiApp.Domain.Interfaces;

namespace MiApp.Tests.Services;

public class EnrollmentTests
{
    private readonly Mock<IEnrollmentRepository> _enrollmentRepository;
    private readonly Mock<ISubjectRepository> _subjectRepository;
    private readonly Mock<IStudentRepository> _studentRepository;
    private readonly Mock<ICurrentUserService> _currentUserService;
    private readonly Mock<IValidator<EnrollmentRequestDto>> _validator;

    private readonly EnrollmentService _enrollmentService;

    public EnrollmentTests()
    {
        _enrollmentRepository = new Mock<IEnrollmentRepository>();
        _subjectRepository = new Mock<ISubjectRepository>();
        _studentRepository = new Mock<IStudentRepository>();
        _currentUserService = new Mock<ICurrentUserService>();
        _validator = new Mock<IValidator<EnrollmentRequestDto>>();

        _enrollmentService = new EnrollmentService(
            _enrollmentRepository.Object,
            _subjectRepository.Object,
            _currentUserService.Object,
            _studentRepository.Object,
            _validator.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnStudentEnrollments()
    {
        // Arrange
        var studentId = 1;

        var professor = new Professor
        {
            Id = 1,
            FirstName = "Carlos",
            LastName = "Gómez"
        };

        var subject = new Subject
        {
            Id = 1,
            Name = "Matemáticas",
            Credits = 3,
            Professor = professor,
            ProfessorId = professor.Id
        };

        var enrollment = new Enrollment
        {
            Id = 1,
            StudentId = studentId,
            SubjectId = subject.Id,
            Subject = subject
        };

        _currentUserService
            .Setup(x => x.UserId)
            .Returns(studentId);

        _enrollmentRepository
            .Setup(x => x.GetAllByStudentAsync(studentId))
            .ReturnsAsync([enrollment]);

        // Act
        var result = await _enrollmentService.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);

        var item = result.First();

        item.Id.Should().Be(1);
        item.SubjectId.Should().Be(1);
        item.SubjectName.Should().Be("Matemáticas");
        item.Credits.Should().Be(3);
        item.ProfessorName.Should().Be("Carlos Gómez");

        _enrollmentRepository.Verify(
            x => x.GetAllByStudentAsync(studentId),
            Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyResult_WhenStudentHasNoEnrollments()
    {
        // Arrange
        var studentId = 1;

        _currentUserService
            .Setup(x => x.UserId)
            .Returns(studentId);

        _enrollmentRepository
            .Setup(x => x.GetAllByStudentAsync(studentId))
            .ReturnsAsync([]);

        // Act
        var result = await _enrollmentService.GetAllAsync();

        // Assert
        result.Should().BeEmpty();

        _enrollmentRepository.Verify(
            x => x.GetAllByStudentAsync(studentId),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateEnrollment_WhenRequestIsValid()
    {
        // Arrange
        var studentId = 1;

        var request = new EnrollmentRequestDto
        {
            SubjectId = 1
        };

        var professor = new Professor
        {
            Id = 1,
            FirstName = "Carlos",
            LastName = "Gómez"
        };

        var subject = new Subject
        {
            Id = 1,
            Name = "Matemáticas",
            Credits = 3,
            ProfessorId = professor.Id,
            Professor = professor
        };

        var student = new Student(
            "Andres",
            "Santos",
            "student@test.com",
            "hashed-password")
        {
            Id = studentId
        };

        var createdEnrollment = new Enrollment
        {
            Id = 1,
            StudentId = studentId,
            SubjectId = subject.Id,
            Student = student,
            Subject = subject
        };

        _validator
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _currentUserService
            .Setup(x => x.UserId)
            .Returns(studentId);

        _studentRepository
            .Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync(student);

        _subjectRepository
            .Setup(x => x.GetByIdAsync(request.SubjectId))
            .ReturnsAsync(subject);

        _enrollmentRepository
            .Setup(x => x.CreateAsync(It.IsAny<Enrollment>()))
            .ReturnsAsync(createdEnrollment);

        // Act
        var result = await _enrollmentService.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.SubjectId.Should().Be(1);
        result.SubjectName.Should().Be("Matemáticas");
        result.Credits.Should().Be(3);
        result.ProfessorName.Should().Be("Carlos Gómez");

        _studentRepository.Verify(
            x => x.GetByIdAsync(studentId),
            Times.Once);

        _subjectRepository.Verify(
            x => x.GetByIdAsync(request.SubjectId),
            Times.Once);

        _enrollmentRepository.Verify(
            x => x.CreateAsync(It.IsAny<Enrollment>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenRequestIsInvalid()
    {
        // Arrange
        var request = new EnrollmentRequestDto();

        var validationResult = new ValidationResult(
        [
            new ValidationFailure(
                "SubjectId",
                "SubjectId must be greater than 0")
        ]);

        _validator
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act
        Func<Task> act = () =>
            _enrollmentService.CreateAsync(request);

        // Assert
        await act.Should()
            .ThrowAsync<ValidationException>();

        _studentRepository.Verify(
            x => x.GetByIdAsync(It.IsAny<int>()),
            Times.Never);

        _subjectRepository.Verify(
            x => x.GetByIdAsync(It.IsAny<int>()),
            Times.Never);

        _enrollmentRepository.Verify(
            x => x.CreateAsync(It.IsAny<Enrollment>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowKeyNotFoundException_WhenStudentDoesNotExist()
    {
        // Arrange
        var studentId = 1;

        var request = new EnrollmentRequestDto
        {
            SubjectId = 1
        };

        var subject = new Subject
        {
            Id = 1,
            Name = "Matemáticas",
            Credits = 3
        };

        _validator
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _currentUserService
            .Setup(x => x.UserId)
            .Returns(studentId);

        _studentRepository
            .Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync((Student?)null);

        _subjectRepository
            .Setup(x => x.GetByIdAsync(request.SubjectId))
            .ReturnsAsync(subject);

        // Act
        Func<Task> act = () =>
            _enrollmentService.CreateAsync(request);

        // Assert
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage("Student not found");

        _studentRepository.Verify(
            x => x.GetByIdAsync(studentId),
            Times.Once);

        _enrollmentRepository.Verify(
            x => x.CreateAsync(It.IsAny<Enrollment>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowKeyNotFoundException_WhenSubjectDoesNotExist()
    {
        // Arrange
        var studentId = 1;

        var request = new EnrollmentRequestDto
        {
            SubjectId = 1
        };

        var student = new Student(
            "Andres",
            "Santos",
            "student@test.com",
            "hashed-password")
        {
            Id = studentId
        };

        _validator
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _currentUserService
            .Setup(x => x.UserId)
            .Returns(studentId);

        _studentRepository
            .Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync(student);

        _subjectRepository
            .Setup(x => x.GetByIdAsync(request.SubjectId))
            .ReturnsAsync((Subject?)null);

        // Act
        Func<Task> act = () =>
            _enrollmentService.CreateAsync(request);

        // Assert
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage("Subject not found");

        _studentRepository.Verify(
            x => x.GetByIdAsync(studentId),
            Times.Once);

        _subjectRepository.Verify(
            x => x.GetByIdAsync(request.SubjectId),
            Times.Once);

        _enrollmentRepository.Verify(
            x => x.CreateAsync(It.IsAny<Enrollment>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEnrollment_WhenRequestIsValid()
    {
        // Arrange
        var studentId = 1;

        var request = new EnrollmentRequestDto
        {
            SubjectId = 2
        };

        var oldSubject = new Subject
        {
            Id = 1,
            Name = "Matemáticas",
            Credits = 3,
            ProfessorId = 1,
            Professor = new Professor
            {
                Id = 1,
                FirstName = "Carlos",
                LastName = "Gómez"
            }
        };

        var newSubject = new Subject
        {
            Id = 2,
            Name = "Programación",
            Credits = 3,
            ProfessorId = 2,
            Professor = new Professor
            {
                Id = 2,
                FirstName = "Laura",
                LastName = "Martínez"
            }
        };

        var enrollment = new Enrollment
        {
            Id = 1,
            StudentId = studentId,
            SubjectId = oldSubject.Id,
            Subject = oldSubject
        };

        var student = new Student(
            "Andres",
            "Santos",
            "student@test.com",
            "hashed-password")
        {
            Id = studentId
        };

        _validator
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _currentUserService
            .Setup(x => x.UserId)
            .Returns(studentId);

        _enrollmentRepository
            .Setup(x => x.GetByIdAsync(enrollment.Id))
            .ReturnsAsync(enrollment);

        _subjectRepository
            .Setup(x => x.GetByIdAsync(request.SubjectId))
            .ReturnsAsync(newSubject);

        _studentRepository
            .Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync(student);

        // Act
        var result = await _enrollmentService.UpdateAsync(
            enrollment.Id,
            request);

        // Assert
        result.Should().NotBeNull();
        result.SubjectId.Should().Be(2);
        result.SubjectName.Should().Be("Programación");
        result.Credits.Should().Be(3);
        result.ProfessorName.Should().Be("Laura Martínez");

        enrollment.SubjectId.Should().Be(2);
        enrollment.Subject.Should().Be(newSubject);

        _enrollmentRepository.Verify(
            x => x.UpdateAsync(enrollment),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowValidationException_WhenRequestIsInvalid()
    {
        // Arrange
        var request = new EnrollmentRequestDto();

        var validationResult = new ValidationResult(
        [
            new ValidationFailure(
                "SubjectId",
                "SubjectId must be greater than 0")
        ]);

        _validator
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act
        Func<Task> act = () =>
            _enrollmentService.UpdateAsync(1, request);

        // Assert
        await act.Should()
            .ThrowAsync<ValidationException>();

        _enrollmentRepository.Verify(
            x => x.GetByIdAsync(It.IsAny<int>()),
            Times.Never);

        _subjectRepository.Verify(
            x => x.GetByIdAsync(It.IsAny<int>()),
            Times.Never);

        _studentRepository.Verify(
            x => x.GetByIdAsync(It.IsAny<int>()),
            Times.Never);

        _enrollmentRepository.Verify(
            x => x.UpdateAsync(It.IsAny<Enrollment>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowKeyNotFoundException_WhenEnrollmentDoesNotExist()
    {
        // Arrange
        var studentId = 1;

        var request = new EnrollmentRequestDto
        {
            SubjectId = 2
        };

        _validator
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _currentUserService
            .Setup(x => x.UserId)
            .Returns(studentId);

        _enrollmentRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync((Enrollment?)null);

        // Act
        Func<Task> act = () =>
            _enrollmentService.UpdateAsync(1, request);

        // Assert
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage("Enrollment not found");

        _subjectRepository.Verify(
            x => x.GetByIdAsync(It.IsAny<int>()),
            Times.Never);

        _studentRepository.Verify(
            x => x.GetByIdAsync(It.IsAny<int>()),
            Times.Never);

        _enrollmentRepository.Verify(
            x => x.UpdateAsync(It.IsAny<Enrollment>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowUnauthorizedAccessException_WhenEnrollmentBelongsToAnotherStudent()
    {
        // Arrange
        var studentId = 1;

        var request = new EnrollmentRequestDto
        {
            SubjectId = 2
        };

        var enrollment = new Enrollment
        {
            Id = 1,
            StudentId = 2,
            SubjectId = 1
        };

        _validator
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _currentUserService
            .Setup(x => x.UserId)
            .Returns(studentId);

        _enrollmentRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(enrollment);

        // Act
        Func<Task> act = () =>
            _enrollmentService.UpdateAsync(1, request);

        // Assert
        await act.Should()
            .ThrowAsync<UnauthorizedAccessException>();

        _subjectRepository.Verify(
            x => x.GetByIdAsync(It.IsAny<int>()),
            Times.Never);

        _studentRepository.Verify(
            x => x.GetByIdAsync(It.IsAny<int>()),
            Times.Never);

        _enrollmentRepository.Verify(
            x => x.UpdateAsync(It.IsAny<Enrollment>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowKeyNotFoundException_WhenNewSubjectDoesNotExist()
    {
        // Arrange
        var studentId = 1;

        var request = new EnrollmentRequestDto
        {
            SubjectId = 2
        };

        var enrollment = new Enrollment
        {
            Id = 1,
            StudentId = studentId,
            SubjectId = 1
        };

        _validator
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _currentUserService
            .Setup(x => x.UserId)
            .Returns(studentId);

        _enrollmentRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(enrollment);

        _subjectRepository
            .Setup(x => x.GetByIdAsync(2))
            .ReturnsAsync((Subject?)null);

        // Act
        Func<Task> act = () =>
            _enrollmentService.UpdateAsync(1, request);

        // Assert
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage("Subject not found");

        _studentRepository.Verify(
            x => x.GetByIdAsync(It.IsAny<int>()),
            Times.Never);

        _enrollmentRepository.Verify(
            x => x.UpdateAsync(It.IsAny<Enrollment>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowKeyNotFoundException_WhenStudentDoesNotExist()
    {
        // Arrange
        var studentId = 1;

        var request = new EnrollmentRequestDto
        {
            SubjectId = 2
        };

        var enrollment = new Enrollment
        {
            Id = 1,
            StudentId = studentId,
            SubjectId = 1
        };

        var subject = new Subject
        {
            Id = 2,
            Name = "Programación",
            Credits = 3,
            ProfessorId = 2,
            Professor = new Professor
            {
                Id = 2,
                FirstName = "Laura",
                LastName = "Martínez"
            }
        };

        _validator
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _currentUserService
            .Setup(x => x.UserId)
            .Returns(studentId);

        _enrollmentRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(enrollment);

        _subjectRepository
            .Setup(x => x.GetByIdAsync(2))
            .ReturnsAsync(subject);

        _studentRepository
            .Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync((Student?)null);

        // Act
        Func<Task> act = () =>
            _enrollmentService.UpdateAsync(1, request);

        // Assert
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage("Student not found");

        _enrollmentRepository.Verify(
            x => x.UpdateAsync(It.IsAny<Enrollment>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteEnrollment_WhenEnrollmentBelongsToStudent()
    {
        // Arrange
        var studentId = 1;

        var enrollment = new Enrollment
        {
            Id = 1,
            StudentId = studentId,
            SubjectId = 1
        };

        _currentUserService
            .Setup(x => x.UserId)
            .Returns(studentId);

        _enrollmentRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(enrollment);

        // Act
        await _enrollmentService.DeleteAsync(1);

        // Assert
        _enrollmentRepository.Verify(
            x => x.DeleteAsync(enrollment),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowKeyNotFoundException_WhenEnrollmentDoesNotExist()
    {
        // Arrange
        _enrollmentRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync((Enrollment?)null);

        // Act
        Func<Task> act = () =>
            _enrollmentService.DeleteAsync(1);

        // Assert
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage("Enrollment not found");

        _enrollmentRepository.Verify(
            x => x.DeleteAsync(It.IsAny<Enrollment>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowUnauthorizedAccessException_WhenEnrollmentBelongsToAnotherStudent()
    {
        // Arrange
        var studentId = 1;

        var enrollment = new Enrollment
        {
            Id = 1,
            StudentId = 2,
            SubjectId = 1
        };

        _currentUserService
            .Setup(x => x.UserId)
            .Returns(studentId);

        _enrollmentRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(enrollment);

        // Act
        Func<Task> act = () =>
            _enrollmentService.DeleteAsync(1);

        // Assert
        await act.Should()
            .ThrowAsync<UnauthorizedAccessException>();

        _enrollmentRepository.Verify(
            x => x.DeleteAsync(It.IsAny<Enrollment>()),
            Times.Never);
    }
}