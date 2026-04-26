using CursoLog.WebApi.Models.Enums;

namespace CursoLog.WebApi.DTOs.Course;

public record CreateCourseRequest(
    string Name,
    string? Description,
    string Url,
    CoursePriceRange PriceRange,
    CourseDurationRange DurationRange,
    CourseLanguage Language,
    string? ThumbnailUrl,
    Guid InstructorId,
    Guid PlatformId,
    Guid CategoryId,
    IEnumerable<Guid> TechnologiesId
);
