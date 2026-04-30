using CursoLog.WebApi.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace CursoLog.WebApi.DTOs.Course;

public record UpdateCourseRequest(
    [StringLength(100)] string Name,
    [StringLength(10000)] string? Description, 
    CoursePriceRange PriceRange,
    CourseDurationRange DurationRange,
    [StringLength(200)] string? ThumbnailUrl,
    IEnumerable<Guid> TechnologiesId);
