using CursoLog.WebApi.DTOs.Course;
using CursoLog.WebApi.Models.Enums;
using FluentResults;

namespace CursoLog.WebApi.Services.Interfaces;

public interface ICourseService
{
    Task<Result<CourseDetailResponse>> GetCourseByIdAsync(Guid id);
    Task<Result<IEnumerable<CourseListResponse>>> GetAllCoursesAsync();
    Task<Result<CourseDetailResponse>> AddCourseAsync(
        string name,
        string? description,
        string url,
        CoursePriceRange priceRange,
        CourseDurationRange durationRange,
        CourseLanguage language,
        string? thumbnailUrl,
        Guid instructorId,
        Guid platformId,
        Guid categoryId,
        IEnumerable<Guid> technologiesId);
    Task<Result> UpdateCourseAsync(
        Guid id,
        string name, 
        string? description, 
        CoursePriceRange priceRange, 
        CourseDurationRange durationRange,
        IEnumerable<Guid> technologiesId);

    Task<Result> DeleteCourseAsync(Guid id);
}
