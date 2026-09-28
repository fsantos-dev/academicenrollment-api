namespace MiApp.Domain.Entities;

public class Enrollment
{
    public int Id {get; set;}
    public DateTime EnrollmentDate {get; set;}
    public int StudentId {get; set;}
    public int SubjectId {get; set;}
    public Student Student {get; set;} = null!;
    public Subject Subject {get; set;} = null!;

}   