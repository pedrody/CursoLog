using CursoLog.WebApi.DTOs;
using CursoLog.WebApi.Models;

namespace CursoLog.WebApi.Extensions.Mappings;

public static class ReviewMappingExtensions
{
    public static ReviewResponse ToReviewResponse(this Review review)
    {
        return new ReviewResponse(
            review.Id, 
            review.CourseId, 
            review.Course.Name, 
            review.UserId, 
            review.User.UserName!,
            review.Rating, 
            review.Comment, 
            review.UpdatedAt, 
            review.CreatedAt);
    }
}
