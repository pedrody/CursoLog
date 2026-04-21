using CursoLog.WebApi.Models.Enums;

namespace CursoLog.WebApi.DTOs;

public record TechnologyResponse(Guid Id, string Name, TechnologyType Type, string? LogoUrl);
