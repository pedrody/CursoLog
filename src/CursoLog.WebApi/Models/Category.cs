namespace CursoLog.WebApi.Models;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public ICollection<Course> Courses { get; set; } = [];

    private Category() { }

    public Category(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
        CreatedAt = DateTime.UtcNow;
    }
}
