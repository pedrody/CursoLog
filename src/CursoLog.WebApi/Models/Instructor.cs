namespace CursoLog.WebApi.Models;

public class Instructor : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? ProfileUrl { get; set; }
    public ICollection<Course> Courses { get; set; } = [];

    private Instructor() { }

    public Instructor(string name, string? bio, string? profileUrl)
    {
        Id = Guid.NewGuid();
        Name = name;
        Bio = bio;
        ProfileUrl = profileUrl;
        CreatedAt = DateTime.UtcNow;
    }
}
