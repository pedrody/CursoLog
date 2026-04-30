using System.ComponentModel.DataAnnotations;

namespace CursoLog.WebApi.DTOs;

public record CreateReviewRequest(
    Guid courseId, 
    decimal rating, 
    [StringLength(5000)] string? comment
);
