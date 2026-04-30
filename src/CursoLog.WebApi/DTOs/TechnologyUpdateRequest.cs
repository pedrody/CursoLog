using CursoLog.WebApi.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace CursoLog.WebApi.DTOs;

public record TechnologyUpdateRequest([StringLength(100)] string Name, TechnologyType Type);
