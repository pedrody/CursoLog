using System.ComponentModel.DataAnnotations;

namespace CursoLog.WebApi.DTOs;

public record CategoryCreateRequest([StringLength(100)] string Name);
