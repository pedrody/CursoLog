using CursoLog.WebApi.DTOs;
using FluentResults;

namespace CursoLog.WebApi.Services.Interfaces;

public interface IPlatformService
{
    Task<Result<PlatformResponse>> GetPlatformByIdAsync(Guid id);
    Task<Result<IEnumerable<PlatformResponse>>> GetAllPlatformsAsync();
    Task<Result<PlatformResponse>> AddPlatformAsync(string name, string url);
    Task<Result> DeletePlatformAsync(Guid id);
    Task<Result> UpdatePlatformAsync(Guid id, string name, string url);
}
