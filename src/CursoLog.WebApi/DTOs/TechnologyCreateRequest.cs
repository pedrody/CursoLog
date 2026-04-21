using CursoLog.WebApi.Models.Enums;

namespace CursoLog.WebApi.DTOs;

public record TechnologyCreateRequest(string Name, TechnologyType Type);
