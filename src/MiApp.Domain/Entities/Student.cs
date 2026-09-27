namespace MiApp.Domain.Entities;

public class Student
{
    public int Id { get; set; }
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public bool IsActive { get; private set; }

    public List<Enrollment> Enrollments { get; private set; } = [];

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


    public void EnrollInSubject(Subject subject)
    {
        if (Enrollments.Count >= 3)
        {
            throw new ArgumentException("The maximum number of allowed subjects has been exceeded");
        }
        if (Enrollments.Any(e => e.Subject.ProfessorId == subject.ProfessorId))
        {
            throw new ArgumentException("The student cannot enroll in two subjects with the same professor");
        }

        var enrollment = new Enrollment
        {
            StudentId = Id,
            SubjectId = subject.Id,
            Student = this,
            Subject = subject
        };

        Enrollments.Add(enrollment);
    }

    public void ChangeSubject(Enrollment enrollment, Subject newSubject)
    {
        if (Enrollments
            //Check my enrollments, but exclude the one I'm editing.
            .Where(e => e.Id != enrollment.Id)
            //Do any of the other classes have the same teacher as the new one?
            .Any(e => e.Subject.ProfessorId == newSubject.ProfessorId))
        {
            throw new ArgumentException(
                "The student cannot enroll in two subjects with the same professor");
        }

        enrollment.SubjectId = newSubject.Id;
        enrollment.Subject = newSubject;
    }
}