using System.ComponentModel.DataAnnotations;

namespace CursoLog.WebApi.DTOs;

public record CategoryUpdateRequest([StringLength(100)] string Name);

