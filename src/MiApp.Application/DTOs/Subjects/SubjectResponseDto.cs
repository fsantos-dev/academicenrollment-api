namespace MiApp.Application.DTOs;

public class SubjectResponseDto
{
    public int Id {get; set;}
    public string Name {get; set;} = null!;
    public int Credits {get; set;}
    public int ProfessorId {get; set;}
    public string ProfessorName {get; set;} = null!;
}