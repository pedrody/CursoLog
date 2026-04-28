using CursoLog.WebApi.DTOs;
using CursoLog.WebApi.Models;
using FluentResults;

namespace CursoLog.WebApi.Services.Interfaces;

public interface IReviewService
{
    Task<Result<ReviewResponse>> GetReviewByIdAsync(Guid id);
    Task<Result<IEnumerable<ReviewResponse>>> GetReviewsByCourseIdAsync(Guid courseId);
    Task<Result<IEnumerable<ReviewResponse>>> GetReviewsByUserIdAsync(Guid userId);
    Task<Result<ReviewResponse>> AddReviewAsync(Guid userId, Guid courseId, decimal rating, string? comment);
    Task<Result> UpdateReviewAsync(Guid userId, Guid reviewId, decimal rating, string? comment);
    Task<Result> DeleteReviewAsync(Guid userId, Guid reviewId);
}
