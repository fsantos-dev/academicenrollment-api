namespace MiApp.Domain.Entities;

public class Student
{
    public int Id {get; set;}
    public string FirstName {get; private set;} = null!;
    public string LastName {get; private set;} = null!;
    public string Email {get; private set;} = null!;
    public string PasswordHash {get; private set;} = null!;
    public DateTime CreatedAt {get; private set;}
    public bool IsActive {get; private set;}

    // public List<Enrollment> Enrollments { get; set; } = [];

    public Student(string firstName, string lastName, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("Firstname is required");
        }
        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Lastname is required");
        }
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required");
        }
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password is required");
        }

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

}