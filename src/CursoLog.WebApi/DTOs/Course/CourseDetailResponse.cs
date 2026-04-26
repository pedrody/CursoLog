using CursoLog.WebApi.Models;
using CursoLog.WebApi.Models.Enums;

namespace CursoLog.WebApi.DTOs.Course;

public record CourseDetailResponse(
    Guid Id,
    string Name,
    string? Description,
    string Url,
    CoursePriceRange PriceRange,
    CourseDurationRange DurationRange,
    CourseLanguage Language,
    string? ThumbnailUrl,
    Guid InstructorId,
    string InstructorName,
    Guid PlatformId,
    string PlatformName,
    Guid CategoryId,
    string CategoryName,
    int ReviewsCount,
    decimal AverageRating,
    IEnumerable<TechnologyResponse> Technologies
);
