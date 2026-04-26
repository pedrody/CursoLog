using CursoLog.WebApi.DTOs;
using CursoLog.WebApi.Models;

namespace CursoLog.WebApi.Extensions.Mappings;

public static class PlatformMappingExtensions
{
    public static PlatformResponse ToPlatformResponse(this Platform platform)
    {
        return new PlatformResponse(platform.Id, platform.Name, platform.Url);
    }
}
