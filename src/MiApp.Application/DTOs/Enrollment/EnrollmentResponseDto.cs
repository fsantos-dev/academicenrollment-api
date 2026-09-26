namespace MiApp.Application.DTOs.Enrollment;

public class EnrollmentResponseDto
{
    public int Id {get; set;}
    public int SubjectId {get; set;}
    public string SubjectName {get; set;} = null!;
    public int Credits {get; set;}
    public string ProfessorName {get; set;} = null!;
}