using CursoLog.WebApi.DTOs;
using CursoLog.WebApi.Extensions.Mappings;
using CursoLog.WebApi.Models;
using CursoLog.WebApi.Models.Errors;
using CursoLog.WebApi.Models.Exceptions;
using CursoLog.WebApi.Repositories.Interfaces;
using CursoLog.WebApi.Services.Interfaces;
using FluentResults;

namespace CursoLog.WebApi.Services.Implementations;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ILogger<ReviewService> _logger;
    private readonly IUserService _userService;

    public ReviewService(IReviewRepository reviewRepository, ICourseRepository courseRepository, ILogger<ReviewService> logger, IUserService userService)
    {
        _reviewRepository = reviewRepository;
        _courseRepository = courseRepository;
        _logger = logger;
        _userService = userService;
    }

    public async Task<Result<ReviewResponse>> GetReviewByIdAsync(Guid id)
    {
        var review = await _reviewRepository.GetByIdAsync(id);

        if (review is null)
            return Result.Fail(new NotFoundError("Review not found"));

        return Result.Ok(review.ToReviewResponse());
    }

    public async Task<Result<IEnumerable<ReviewResponse>>> GetReviewsByCourseIdAsync(Guid courseId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);

        if (course is null)
            return Result.Fail(new NotFoundError("Course not found"));

        var reviews = await _reviewRepository.GetByCourseIdAsync(courseId);

        return Result.Ok(reviews.Select(r => r.ToReviewResponse()));
    }

    public async Task<Result<IEnumerable<ReviewResponse>>> GetReviewsByUserIdAsync(Guid userId)
    {
        var user = await _userService.GetUserProfileByIdAsync(userId);
        
        if (user is null)
            return Result.Fail(new NotFoundError("User not found"));

        var reviews = await _reviewRepository.GetByUserIdAsync(userId);

        return Result.Ok(reviews.Select(r => r.ToReviewResponse()));
    }

    public async Task<Result<ReviewResponse>> AddReviewAsync(Guid userId, Guid courseId, decimal rating, string? comment)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course is null)
            return Result.Fail(new NotFoundError("Course not found"));

        var userHasReview = await _reviewRepository.UserHasReviewAsync(userId, courseId);
        if (userHasReview)
            return Result.Fail(new ConflictError("Course already reviewed by the user"));

        Review review;
        try
        {
            review = new Review(courseId, rating, comment, userId);
        }
        catch (DomainException ex)
        {
            return Result.Fail(ex.Message);
        }

         var createdReview = await _reviewRepository.AddAsync(review);

        _logger.LogInformation("New review created for the course {CourseId} by the user {UserId}.", courseId, userId);

        return Result.Ok(createdReview.ToReviewResponse());
    }

    public async Task<Result> UpdateReviewAsync(Guid userId, Guid reviewId, decimal rating, string? comment)
    {
        var review = await _reviewRepository.GetByIdAsync(reviewId);

        if (review is null)
            return Result.Fail(new NotFoundError("Review not found"));

        if (review.UserId != userId)
            return Result.Fail(new ForbidenError("User don't have permission to updated this review"));

        try
        {
            review.Update(rating, comment);
        }
        catch (DomainException ex)
        {
            return Result.Fail(ex.Message);
        }

        await _reviewRepository.UpdateAsync(review);

        _logger.LogInformation("Review {ReviewId} updated by the user {UserId}.", reviewId, userId);

        return Result.Ok();
    }

    public async Task<Result> DeleteReviewAsync(Guid userId, Guid reviewId)
    {
        var review = await _reviewRepository.GetByIdAsync(reviewId);

        if (review is null)
            return Result.Fail(new NotFoundError("Review not found"));

        if (review.UserId != userId)
            return Result.Fail(new ForbidenError("User don't have permission to delete this review"));

        await _reviewRepository.DeleteAsync(review);

        _logger.LogInformation("Review {ReviewId} deleted by the user {UserId}.", reviewId, userId);

        return Result.Ok();
    }
}
