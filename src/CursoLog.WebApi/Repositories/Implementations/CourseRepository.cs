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
        return await _context.Courses
            .Include(c => c.Instructor)
            .Include(c => c.Platform)
            .Include(c => c.Category)
            .Include(c => c.Technologies)
            .Include(c => c.Reviews)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Course>> GetAllAsync()
    {
        return await _context.Courses
            .Include(c => c.Reviews)
            .ToListAsync();
    }

    public async Task<Course> AddAsync(Course course)
    {
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
        var createdCourse = await _context.Courses
            .Include(c => c.Instructor)
            .Include(c => c.Platform)
            .Include(c => c.Category)
            .Include(c => c.Technologies)
            .Include(c => c.Reviews)
            .FirstOrDefaultAsync(c => c.Id == course.Id);

        return createdCourse!;
    }

    public async Task UpdateAsync(Course course)
    {
        _context.Courses.Update(course);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Course course)
    {
        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();
    }
}