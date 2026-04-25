using CursoLog.WebApi.DTOs;
using CursoLog.WebApi.Models;

namespace CursoLog.WebApi.Extensions.Mappings;

public static class TechnologyMappingExtensions
{
    public static TechnologyResponse ToTechnologyResponse(this Technology technology)
    {
        return new TechnologyResponse(technology.Id, technology.Name, technology.Type);
    }
}
