using CursoLog.WebApi.Models.Exceptions;

namespace CursoLog.WebApi.Models;

public class Review : BaseEntity
{
    public decimal Rating { get; private set; }
    public string? Comment { get; private set; }
    public Guid CourseId { get; private set; }
    public Course Course { get; private set; } = null!;
    public DateTime? UpdatedAt { get; private set; }
    public Guid UserId { get; private set; }
    public ApplicationUser User { get; private set; } = null!;

    private Review() { }

    public Review(Guid courseId, decimal rating, string? comment, Guid userId)
    {
        ValidateRating(rating);
        Id = Guid.NewGuid();
        CourseId = courseId;
        Rating = rating;
        Comment = comment;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(decimal rating, string? comment)
    {
        ValidateRating(rating);

        Rating = rating;
        Comment = comment;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateRating(decimal rating)
    {
        if (rating < 0 || rating > 5)
        {
            throw new DomainException("Rating must be between 0 and 5");
        }
        if (rating % 0.5m != 0)
        {
            throw new DomainException("Rating must be in increments of 0.5");
        }
    }
}
