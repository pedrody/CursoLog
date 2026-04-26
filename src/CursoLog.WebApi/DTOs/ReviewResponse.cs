namespace CursoLog.WebApi.DTOs;

public record ReviewResponse(
    Guid Id,
    Guid CourseId, 
    string CourseName,
    Guid UserId,
    string UserName,
    decimal Rating,
    string? Comment,
    DateTime? UpdatedAt,
    DateTime CreatedAt
);

