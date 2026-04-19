namespace CursoLog.WebApi.DTOs;

public record UserProfileResponse(Guid Id, string Username, string Name, string? Bio, string? ProfilePictureUrl, DateTime RegisteredAt);
