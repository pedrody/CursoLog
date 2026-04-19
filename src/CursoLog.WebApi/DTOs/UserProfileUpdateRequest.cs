namespace CursoLog.WebApi.DTOs;

public record UserProfileUpdateRequest(string Name, string? Bio, string? ProfilePictureUrl);
