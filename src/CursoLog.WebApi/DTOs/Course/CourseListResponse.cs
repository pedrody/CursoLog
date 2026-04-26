namespace CursoLog.WebApi.DTOs.Course;

public record CourseListResponse(
    Guid Id, 
    string Name, 
    string? ThumbnailUrl, 
    int ReviewsCount, 
    decimal AverageRating);
