using CursoLog.WebApi.Data;
using CursoLog.WebApi.Models;
using CursoLog.WebApi.Models.Enums;
using CursoLog.WebApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CursoLog.WebApi.Repositories.Implementations;

public class CourseRepository : Repository<Course>, ICourseRepository
{
    public CourseRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Course>> GetByFilterAsync(
        string? name, CoursePriceRange? priceRange, CourseDurationRange? durationRange,
        CourseLanguage? language, Guid? instructorId, Guid? platformId,
        Guid? categoryId, IEnumerable<Guid>? technologiesId, decimal? averageRating)
    {
        var query = _context.Courses.Include(c => c.Reviews).AsNoTracking().AsQueryable();

        if (!String.IsNullOrEmpty(name))
            query = query.Where(c => c.Name.ToLower().Contains(name.ToLower()));

        if (priceRange is not null)
            query = query.Where(c => c.PriceRange == priceRange);

        if (durationRange is not null)
            query = query.Where(c => c.DurationRange == durationRange);

        if (language is not null)
            query = query.Where(c => c.Language == language);

        if (instructorId is not null)
            query = query.Where(c => c.InstructorId == instructorId);

        if (platformId is not null)
            query = query.Where(c => c.PlatformId == platformId);

        if (categoryId is not null)
            query = query.Where(c => c.CategoryId == categoryId);

        if (technologiesId is not null)
            foreach (var technologyId in technologiesId)
                query = query.Where(c => c.Technologies.Select(t => t.Id).Contains(technologyId));

        if (averageRating is not null)
            query = query.Where(c => c.Reviews.Average(r => r.Rating) >= averageRating);

        return await query.ToListAsync();
    }
}