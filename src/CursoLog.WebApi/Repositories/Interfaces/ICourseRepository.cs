using CursoLog.WebApi.Models;
using CursoLog.WebApi.Models.Enums;

namespace CursoLog.WebApi.Repositories.Interfaces;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(Guid id);
    Task<IEnumerable<Course>> GetAllAsync();
    Task<Course> AddAsync(Course course);
    Task UpdateAsync(Course course);
    Task DeleteAsync(Course course);
    Task<IEnumerable<Course>> GetByFilterAsync(
        string? name, CoursePriceRange? priceRange, CourseDurationRange? durationRange,
        CourseLanguage? language, Guid? instructorId, Guid? platformId,
        Guid? categoryId, IEnumerable<Guid>? technologiesId, decimal? averageRating);
}
