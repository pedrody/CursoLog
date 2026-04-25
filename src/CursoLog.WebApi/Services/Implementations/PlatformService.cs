using CursoLog.WebApi.DTOs;
using CursoLog.WebApi.Extensions.Mappings;
using CursoLog.WebApi.Models;
using CursoLog.WebApi.Repositories.Interfaces;
using CursoLog.WebApi.Services.Interfaces;
using FluentResults;

namespace CursoLog.WebApi.Services.Implementations;

public class PlatformService : IPlatformService
{
    private readonly IPlatformRepository _platformRepository;
    private readonly ILogger<PlatformService> _logger;

    public PlatformService(IPlatformRepository platformRepository, ILogger<PlatformService> logger)
    {
        _platformRepository = platformRepository;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<PlatformResponse>>> GetAllPlatformsAsync()
    {
        var platforms = await _platformRepository.GetAllAsync();

        var platformsResponse = platforms.Select(p => p.ToPlatformResponse());

        return Result.Ok(platformsResponse);
    }
    
    public async Task<Result<PlatformResponse>> GetPlatformByIdAsync(Guid id)
    {
        var platform = await _platformRepository.GetByIdAsync(id);

        if (platform is null)
            return Result.Fail("Platform not found");

        return Result.Ok(platform.ToPlatformResponse());
    }
    
    public async Task<Result<PlatformResponse>> AddPlatformAsync(string name, string url)
    {
        var platform = new Platform(name, url);

        await _platformRepository.AddAsync(platform);

        _logger.LogInformation("Platform {PlatformName} created successfully with ID {PlatformId}.", platform.Name, platform.Id);

        return Result.Ok(platform.ToPlatformResponse());
    }
    
    public async Task<Result> UpdatePlatformAsync(Guid id, string name, string url)
    {
        var platform = await _platformRepository.GetByIdAsync(id);

        if (platform is null)
        {
            _logger.LogWarning("Failed to update platform with ID {PlatformId}. Reason: platform not found.", id);
            return Result.Fail("Platform not found");
        }

        platform.Name = name;
        platform.Url = url;

        await _platformRepository.UpdateAsync(platform);

        _logger.LogInformation("Platform {PlatformId} successfully updated to {PlatformName}, {PlatformUrl}.", platform.Id, platform.Name, platform.Url);

        return Result.Ok();
    }

    public async Task<Result> DeletePlatformAsync(Guid id)
    {
        var platform = await _platformRepository.GetByIdAsync(id);

        if (platform is null)
        {
            _logger.LogWarning("Failed to delete platform with ID {PlatformId}. Reason: platform not found.", id);
            return Result.Fail("Platform not found");
        }

        await _platformRepository.DeleteAsync(platform);
        _logger.LogInformation("Platform with ID {PlatformId} successfully deleted.", platform.Id);
        return Result.Ok();
    }
}
