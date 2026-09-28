namespace MiApp.Domain.Entities;

public class Subject
{
    public int Id {get; set;}
    public string Name {get; set;} = null!;
    public int Credits {get; set;}
    public int ProfessorId {get; set;} //FK
    public DateTime CreatedAt {get; set;}
    public Professor Professor {get; set;} = null!;//Navegacion //FK Reference to Professor
    public List<Enrollment> Enrollments { get; set; } = [];
}