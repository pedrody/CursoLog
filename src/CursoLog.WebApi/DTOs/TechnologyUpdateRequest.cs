using CursoLog.WebApi.Models.Enums;

namespace CursoLog.WebApi.DTOs;

public record TechnologyUpdateRequest(string Name, TechnologyType Type);
