using CursoLog.WebApi.Models;

namespace CursoLog.WebApi.Repositories.Interfaces;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(Guid id);
    Task<IEnumerable<Review>> GetByCourseIdAsync(Guid courseId);
    Task<IEnumerable<Review>> GetByUserIdAsync(Guid userId);
    Task<Review> AddAsync(Review review);
    Task UpdateAsync(Review review);

    Task DeleteAsync(Review review);
    Task<bool> UserHasReviewAsync(Guid userId, Guid courseId);
}
