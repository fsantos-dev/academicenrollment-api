namespace MiApp.Application.DTOs.Students;

public class ClassmatesResponseDto
{
    public string SubjectName {get; set;} = null!;
    public List<string> ClassMates { get; set;} = [];
}