using CursoLog.WebApi.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace CursoLog.WebApi.DTOs.Course;

public record CreateCourseRequest(
    [StringLength(100)] string Name,
    [StringLength(10000)] string? Description,
    [StringLength(200)] string Url,
    CoursePriceRange PriceRange,
    CourseDurationRange DurationRange,
    CourseLanguage Language,
    [StringLength(200)] string? ThumbnailUrl,
    Guid InstructorId,
    Guid PlatformId,
    Guid CategoryId,
    IEnumerable<Guid> TechnologiesId
);
