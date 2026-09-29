using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using MiApp.Application.DTOs.Auth;
using MiApp.Application.Interfaces;
using MiApp.Application.Services.Auth;
using MiApp.Domain.Entities;
using MiApp.Domain.Exceptions;
using MiApp.Domain.Interfaces;

namespace MiApp.Tests.Application;

public class AuthTests
{
    private readonly Mock<IStudentRepository> _studentRepository;
    private readonly Mock<IPasswordHasher> _passwordHasher;
    private readonly Mock<ITokenService> _tokenService;
    private readonly Mock<IValidator<LoginRequestDto>> _loginValidator;
    private readonly Mock<IValidator<RegisterRequestDto>> _registerValidator;

    private readonly AuthService _authService;

    public AuthTests()
    {
        _studentRepository = new Mock<IStudentRepository>();
        _passwordHasher = new Mock<IPasswordHasher>();
        _tokenService = new Mock<ITokenService>();
        _loginValidator = new Mock<IValidator<LoginRequestDto>>();
        _registerValidator = new Mock<IValidator<RegisterRequestDto>>();

        _authService = new AuthService(
            _studentRepository.Object,
            _passwordHasher.Object,
            _tokenService.Object,
            _loginValidator.Object,
            _registerValidator.Object);
    }

    // =========================================================
    // LOGIN
    // =========================================================

    [Fact]
    public async Task LoginAsync_ShouldReturnLoginResponse_WhenCredentialsAreValid()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Email = "student@test.com",
            Password = "Password123!"
        };

        var student = new Student(
            "Carlos",
            "Gómez",
            request.Email,
            "hashed-password");

        var expiresAt = DateTime.UtcNow.AddHours(1);

        _loginValidator
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _studentRepository
            .Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync(student);

        _passwordHasher
            .Setup(x => x.Verify(
                request.Password,
                student.PasswordHash))
            .Returns(true);

        _tokenService
            .Setup(x => x.GenerateToken(student))
            .Returns(("fake-token", expiresAt));

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("fake-token");
        result.FirstName.Should().Be("Carlos");
        result.LastName.Should().Be("Gómez");
        result.Email.Should().Be("student@test.com");
        result.ExpiresAt.Should().Be(expiresAt);

        _studentRepository.Verify(
            x => x.GetByEmailAsync(request.Email),
            Times.Once);

        _passwordHasher.Verify(
            x => x.Verify(
                request.Password,
                student.PasswordHash),
            Times.Once);

        _tokenService.Verify(
            x => x.GenerateToken(student),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowValidationException_WhenRequestIsInvalid()
    {
        // Arrange
        var request = new LoginRequestDto();

        var validationResult = new ValidationResult(
        [
            new ValidationFailure(
                "Email",
                "Email is required")
        ]);

        _loginValidator
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act
        Func<Task> act = () =>
            _authService.LoginAsync(request);

        // Assert
        await act.Should()
            .ThrowAsync<ValidationException>();

        _studentRepository.Verify(
            x => x.GetByEmailAsync(It.IsAny<string>()),
            Times.Never);

        _passwordHasher.Verify(
            x => x.Verify(
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);

        _tokenService.Verify(
            x => x.GenerateToken(It.IsAny<Student>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowInvalidCredentialsException_WhenStudentDoesNotExist()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Email = "student@test.com",
            Password = "Password123!"
        };

        _loginValidator
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _studentRepository
            .Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync((Student?)null);

        // Act
        Func<Task> act = () =>
            _authService.LoginAsync(request);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidCredentialsException>();

        _passwordHasher.Verify(
            x => x.Verify(
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);

        _tokenService.Verify(
            x => x.GenerateToken(It.IsAny<Student>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowInvalidCredentialException_WhenPasswordIsInvalid()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Email = "student@test.com",
            Password = "WrongPassword123!"
        };

        var student = new Student(
            "Carlos",
            "Gómez",
            request.Email,
            "hashed-password");

        _loginValidator
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _studentRepository
            .Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync(student);

        _passwordHasher
            .Setup(x => x.Verify(
                request.Password,
                student.PasswordHash))
            .Returns(false);

        // Act
        Func<Task> act = () =>
            _authService.LoginAsync(request);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidCredentialsException>();

        _tokenService.Verify(
            x => x.GenerateToken(It.IsAny<Student>()),
            Times.Never);
    }

    // =========================================================
    // REGISTER
    // =========================================================

    [Fact]
    public async Task RegisterAsync_ShouldCreateStudent_WhenRequestIsValid()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            FirstName = "Carlos",
            LastName = "Gómez",
            Email = "student@test.com",
            Password = "Password123!"
        };

        var createdStudent = new Student(
            request.FirstName,
            request.LastName,
            request.Email,
            "hashed-password")
        {
            Id = 1
        };

        _registerValidator
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _studentRepository
            .Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync((Student?)null);

        _passwordHasher
            .Setup(x => x.Hash(request.Password))
            .Returns("hashed-password");

        _studentRepository
            .Setup(x => x.CreateAsync(It.IsAny<Student>()))
            .ReturnsAsync(createdStudent);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.FirstName.Should().Be("Carlos");
        result.LastName.Should().Be("Gómez");

        _studentRepository.Verify(
            x => x.GetByEmailAsync(request.Email),
            Times.Once);

        _passwordHasher.Verify(
            x => x.Hash(request.Password),
            Times.Once);

        _studentRepository.Verify(
            x => x.CreateAsync(It.Is<Student>(student =>
                student.FirstName == request.FirstName &&
                student.LastName == request.LastName &&
                student.Email == request.Email &&
                student.PasswordHash == "hashed-password")),
            Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowValidationException_WhenRequestIsInvalid()
    {
        // Arrange
        var request = new RegisterRequestDto();

        var validationResult = new ValidationResult(
        [
            new ValidationFailure(
                "Email",
                "Email is required")
        ]);

        _registerValidator
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act
        Func<Task> act = () =>
            _authService.RegisterAsync(request);

        // Assert
        await act.Should()
            .ThrowAsync<ValidationException>();

        _studentRepository.Verify(
            x => x.GetByEmailAsync(It.IsAny<string>()),
            Times.Never);

        _passwordHasher.Verify(
            x => x.Hash(It.IsAny<string>()),
            Times.Never);

        _studentRepository.Verify(
            x => x.CreateAsync(It.IsAny<Student>()),
            Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowInvalidOperationException_WhenEmailAlreadyExists()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            FirstName = "Carlos",
            LastName = "Gómez",
            Email = "student@test.com",
            Password = "Password123!"
        };

        var existingStudent = new Student(
            "Laura",
            "Martínez",
            request.Email,
            "hashed-password");

        _registerValidator
            .Setup(x => x.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _studentRepository
            .Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync(existingStudent);

        // Act
        Func<Task> act = () =>
            _authService.RegisterAsync(request);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>();

        _passwordHasher.Verify(
            x => x.Hash(It.IsAny<string>()),
            Times.Never);

        _studentRepository.Verify(
            x => x.CreateAsync(It.IsAny<Student>()),
            Times.Never);
    }
}