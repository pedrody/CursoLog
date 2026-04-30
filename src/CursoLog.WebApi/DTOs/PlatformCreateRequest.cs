using System.ComponentModel.DataAnnotations;

namespace CursoLog.WebApi.DTOs;

public record PlatformCreateRequest([StringLength(100)] string Name, [StringLength(200)] string Url);
