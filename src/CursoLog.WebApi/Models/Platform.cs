namespace CursoLog.WebApi.Models;

public class Platform : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public ICollection<Course> Courses { get; set; } = [];

    private Platform() { }

    public Platform(string name, string url)
    {
        Id = Guid.NewGuid();
        Name = name;
        Url = url;
        CreatedAt = DateTime.UtcNow;
    }
}
