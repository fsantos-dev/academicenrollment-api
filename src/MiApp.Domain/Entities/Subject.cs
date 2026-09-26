namespace MiApp.Domain.Entities;

public class Subject
{
    public int Id {get; set;}
    public string Name {get; private set;} = null!;
    public int Credits {get; private set;}
    public int ProfessorId {get; private set;} //FK
    public DateTime CreatedAt {get; private set;}
    public Professor Professor {get; private set;} = null!;//Navegacion //FK Reference to Professor
}