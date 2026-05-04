using CursoLog.WebApi.Constants;
using CursoLog.WebApi.DTOs.Course;
using CursoLog.WebApi.Extensions.Mappings;
using CursoLog.WebApi.Models;
using CursoLog.WebApi.Models.Enums;
using CursoLog.WebApi.Models.Errors;
using CursoLog.WebApi.Repositories.Interfaces;
using CursoLog.WebApi.Services.Interfaces;
using FluentResults;
using Microsoft.AspNetCore.Authorization;

namespace CursoLog.WebApi.Services.Implementations;

[Authorize]
public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IInstructorRepository _instructorRepository;
    private readonly IPlatformRepository _platformRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITechnologyRepository _technologyRepository;
    private readonly ILogger<CourseService> _logger;

    public CourseService(
        ICourseRepository courseRepository,
        IInstructorRepository instructorRepository,
        IPlatformRepository platformRepository,
        ICategoryRepository categoryRepository,
        ITechnologyRepository technologyRepository,
        ILogger<CourseService> logger)
    {
        _courseRepository = courseRepository;
        _instructorRepository = instructorRepository;
        _platformRepository = platformRepository;
        _categoryRepository = categoryRepository;
        _technologyRepository = technologyRepository;
        _logger = logger;
    }

    public async Task<Result<CourseDetailResponse>> GetCourseByIdAsync(Guid id)
    {
        var course = await _courseRepository.GetByIdAsync(id);

        if (course is null)
            return Result.Fail(new NotFoundError("Course not found"));

        return Result.Ok(course.ToCourseDetailResponse());
    }
    
    public async Task<Result<IEnumerable<CourseListResponse>>> GetAllCoursesAsync()
    {
        var courses = await _courseRepository.GetAllAsync();

        return Result.Ok(courses.Select(c => c.ToCourseListResponse()));
    }

    [Authorize(Roles = Roles.Admin)]
    public async Task<Result<CourseDetailResponse>> AddCourseAsync(string name, string? description, string url, CoursePriceRange priceRange, CourseDurationRange durationRange, CourseLanguage language, string? thumbnailUrl, Guid instructorId, Guid platformId, Guid categoryId, IEnumerable<Guid> technologiesId)
    {
        if (!await _instructorRepository.ExistsAsync(instructorId))
            return Result.Fail(new NotFoundError("Instructor not found"));

        if (!await _platformRepository.ExistsAsync(platformId))
            return Result.Fail(new NotFoundError("Platform not found"));

        if (!await _categoryRepository.ExistsAsync(categoryId))
            return Result.Fail(new NotFoundError("Category not found"));

        var technologies = new List<Technology>();
        foreach (Guid id in technologiesId)
        {
            var technology = await _technologyRepository.GetByIdAsync(id);
            if (technology is not null)
                technologies.Add(technology);
        }

        var course = new Course(name, description, url, priceRange, durationRange, language, thumbnailUrl, instructorId, platformId, categoryId)
        {
            Technologies = technologies
        };

        var createdCourse = await _courseRepository.AddAsync(course);

        _logger.LogInformation("Course \"{CourseId}\" created successfully.", course.Id);

        return Result.Ok(createdCourse.ToCourseDetailResponse());
    }

    public async Task<Result> UpdateCourseAsync(Guid id, string name, string? description, CoursePriceRange priceRange, CourseDurationRange durationRange, IEnumerable<Guid> technologiesId)
    {
        var course = await _courseRepository.GetByIdAsync(id);

        if (course is null)
            return Result.Fail(new NotFoundError("Course not found"));

        course.Name = name;
        course.Description = description;
        course.PriceRange = priceRange;
        course.DurationRange = durationRange;
        course.Technologies.Clear();
        foreach (Guid technologyId in technologiesId)
        {
            var technology = await _technologyRepository.GetByIdAsync(technologyId);
            if (technology is not null)
                course.Technologies.Add(technology);
        }

        await _courseRepository.UpdateAsync(course);

        _logger.LogInformation("Course \"{CourseId}\" updated successfully.", course.Id);

        return Result.Ok();
    }

    public async Task<Result> DeleteCourseAsync(Guid id)
    {
        var course = await _courseRepository.GetByIdAsync(id);

        if (course is null)
            return Result.Fail(new NotFoundError("Course not found"));

        await _courseRepository.DeleteAsync(course);

        _logger.LogInformation("Course \"{CourseId}\" deleted successfully.", course.Id);

        return Result.Ok();
    }

    public async Task<IEnumerable<CourseListResponse>> GetCoursesByFilterAsync(
        string? name, CoursePriceRange? priceRange, CourseDurationRange? durationRange, 
        CourseLanguage? language, Guid? instructorId, Guid? platformId, 
        Guid? categoryId, IEnumerable<Guid>? technologiesId, decimal? averageRating)
    {
        var courses = await _courseRepository.GetByFilterAsync(
            name, priceRange, durationRange, language, instructorId,
            platformId, categoryId, technologiesId, averageRating);

        return courses.Select(c => c.ToCourseListResponse());
    }
}
