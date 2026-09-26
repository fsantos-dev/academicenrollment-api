namespace MiApp.Domain.Entities;

public class Professor
{
    public int Id{get; set;}
    public string FirstName {get; private set;} = null!;
    public string LastName {get; private set;} = null!;
    public DateTime CreatedAt {get; private set;}
    public List<Subject> Subjects {get; private set;} = [];
    
    // public Professor(string firstName, string lastName)
    // {
    //     if (string.IsNullOrWhiteSpace(firstName))
    //     {
    //         throw new ArgumentException("Firstname is required");
    //     }
    //     if (string.IsNullOrWhiteSpace(lastName))
    //     {
    //         throw new ArgumentException("Lastname is required");
    //     }
    // }
}
    