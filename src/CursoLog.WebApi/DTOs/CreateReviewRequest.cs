namespace CursoLog.WebApi.DTOs;

public record CreateReviewRequest(Guid courseId, decimal rating, string? comment);
