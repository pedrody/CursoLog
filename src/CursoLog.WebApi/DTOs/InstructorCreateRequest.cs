using System.ComponentModel.DataAnnotations;

namespace CursoLog.WebApi.DTOs;

public record InstructorCreateRequest(
    [StringLength(100)] string Name,
    [StringLength(500)] string? Bio,
    [StringLength(200)] string? ProfileUrl
);
