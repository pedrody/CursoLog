using System.ComponentModel.DataAnnotations;

namespace CursoLog.WebApi.DTOs;

public record PlatformUpdateRequest([StringLength(100)] string Name, [StringLength(200)] string Url);
