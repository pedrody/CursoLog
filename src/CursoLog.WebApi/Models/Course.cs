using CursoLog.WebApi.Models.Enums;

namespace CursoLog.WebApi.Models;

public class Course : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Url { get; set; } = string.Empty;
    public CoursePriceRange PriceRange { get; set; }
    public CourseDurationRange DurationRange { get; set; }
    public CourseLanguage Language { get; set; }
    public string? ThumbnailUrl { get; set; }
    public Guid InstructorId { get; set; }
    public Instructor Instructor { get; set; } = null!;
    public Guid PlatformId { get; set; }
    public Platform Platform { get; set; } = null!;
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public ICollection<Technology> Technologies { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];

    private Course() { }

    public Course(
        string name,
        string? description,
        string url,
        CoursePriceRange price,
        CourseDurationRange duration,
        CourseLanguage language,
        string? thumbnailUrl,
        Guid instructorId,
        Guid platformId,
        Guid categoryId)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Url = url;
        PriceRange = price;
        DurationRange = duration;
        Language = language;
        ThumbnailUrl = thumbnailUrl;
        InstructorId = instructorId;
        PlatformId = platformId;
        CategoryId = categoryId;
        CreatedAt = DateTime.UtcNow;
    }
}
