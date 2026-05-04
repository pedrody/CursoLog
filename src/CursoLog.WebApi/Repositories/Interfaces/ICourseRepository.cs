using CursoLog.WebApi.Models;
using CursoLog.WebApi.Models.Enums;

namespace CursoLog.WebApi.Repositories.Interfaces;

public interface ICourseRepository : IRepository<Course>
{
    Task<IEnumerable<Course>> GetByFilterAsync(
        string? name, CoursePriceRange? priceRange, CourseDurationRange? durationRange,
        CourseLanguage? language, Guid? instructorId, Guid? platformId,
        Guid? categoryId, IEnumerable<Guid>? technologiesId, decimal? averageRating);
}
