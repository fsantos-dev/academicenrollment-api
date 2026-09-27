namespace MiApp.Domain.Entities;

public class Professor
{
    public int Id{get; set;}
    public string FirstName {get;  set;} = null!;
    public string LastName {get;  set;} = null!;
    public DateTime CreatedAt {get;  set;}
    public List<Subject> Subjects {get;  set;} = [];
    
}
    