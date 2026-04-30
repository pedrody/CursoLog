using System.ComponentModel.DataAnnotations;

namespace CursoLog.WebApi.DTOs;

public record UserProfileUpdateRequest(
    [StringLength(30, MinimumLength = 3)] string Name, 
    [StringLength(200)] string? Bio, 
    [StringLength(200)] string? ProfilePictureUrl
);
