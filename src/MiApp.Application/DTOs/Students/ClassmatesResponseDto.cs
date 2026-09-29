namespace MiApp.Application.DTOs.Students;

public class ClassmatesResponseDto
{
    public string SubjectName {get; set;} = null!;
    public string ProfessorName {get; set;} = null!;
    public List<string> Classmates { get; set;} = [];
}