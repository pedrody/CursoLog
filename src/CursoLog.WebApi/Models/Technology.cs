using CursoLog.WebApi.Models.Enums;

namespace CursoLog.WebApi.Models;

public class Technology : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public TechnologyType Type { get; set; }
    public string? LogoUrl { get; set; }
    public ICollection<Course> Courses { get; set; } = [];

    private Technology() { }

    public Technology(string name, TechnologyType type, string? logoUrl)
    {
        Id = Guid.NewGuid();
        Name = name;
        Type = type;
        LogoUrl = logoUrl;
        CreatedAt = DateTime.UtcNow;
    }
}
