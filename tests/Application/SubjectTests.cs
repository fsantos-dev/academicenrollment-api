using FluentAssertions;
using Moq;
using MiApp.Application.Services.Subject;
using MiApp.Domain.Entities;
using MiApp.Domain.Interfaces;

namespace MiApp.Tests.Application;

public class SubjectTests
{
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllSubjects()
    {
        // Arrange
        var subjects = new List<Subject>
        {
            new()
            {
                Id = 1,
                Name = "Calculo",
                Credits = 3,
                Professor = new Professor
                {
                    Id = 1,
                    FirstName = "Carlos",
                    LastName = "Gomez"
                }
            },
            new()
            {
                Id = 2,
                Name = "Programación",
                Credits = 3,
                Professor = new Professor
                {
                    Id = 2,
                    FirstName = "Laura",
                    LastName = "Martínez"
                }
            }
        };

        var repository = new Mock<ISubjectRepository>();

        repository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(subjects);

        var service = new SubjectService(repository.Object);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);

        result.Should().Contain(x =>
            x.Id == 1 &&
            x.Name == "Calculo" &&
            x.Credits == 3 &&
            x.ProfessorName == "Carlos Gomez");

        result.Should().Contain(x =>
            x.Id == 2 &&
            x.Name == "Programación" &&
            x.Credits == 3 &&
            x.ProfessorName == "Laura Martínez");

        repository.Verify(
            x => x.GetAllAsync(),
            Times.Once);
    }
}
