using CursoLog.WebApi.Data;
using CursoLog.WebApi.Models;
using CursoLog.WebApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CursoLog.WebApi.Repositories.Implementations;

public class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext _context;
    public ReviewRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Review?> GetByIdAsync(Guid id)
    {
        return await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Course)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Review>> GetByCourseIdAsync(Guid courseId)
    {
        return await _context.Reviews
            .Include(r => r.User)
            .Where(r => r.CourseId == courseId)
            .ToListAsync() ;
    }

    public async Task<Review> AddAsync(Review review)
    {
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
        var createdReview = await _context.Reviews.Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == review.Id);

        return createdReview!;
    }

    public async Task UpdateAsync(Review review)
    {
        _context.Reviews.Update(review);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Review review)
    {
        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UserHasReviewAsync(Guid userId, Guid courseId)
    {
        return await _context.Reviews.Where(r => r.CourseId == courseId)
                                     .AnyAsync(r => r.UserId == userId);
    }
}
