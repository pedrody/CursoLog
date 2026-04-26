using CursoLog.WebApi.Models.Enums;

namespace CursoLog.WebApi.DTOs.Course;

public record UpdateCourseRequest(
    string Name, 
    string? Description, 
    CoursePriceRange PriceRange,
    CourseDurationRange DurationRange,
    string? ThumbnailUrl,
    IEnumerable<Guid> TechnologiesId);
