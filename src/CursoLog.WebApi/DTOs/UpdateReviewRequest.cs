using System.ComponentModel.DataAnnotations;

namespace CursoLog.WebApi.DTOs;

public record UpdateReviewRequest(decimal rating, [StringLength(5000)]  string? comment);