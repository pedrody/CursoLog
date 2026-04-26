using CursoLog.WebApi.Data;
using CursoLog.WebApi.Models;
using CursoLog.WebApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CursoLog.WebApi.Repositories.Implementations;

public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _context;

    public CourseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Course?> GetByIdAsync(Guid id)
    {
        return await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
    }
}
