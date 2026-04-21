using CursoLog.WebApi.DTOs;
using CursoLog.WebApi.Models;
using CursoLog.WebApi.Models.Enums;
using CursoLog.WebApi.Repositories.Interfaces;
using CursoLog.WebApi.Services.Interfaces;
using FluentResults;

namespace CursoLog.WebApi.Services.Implementations;

public class TechnologyService : ITechnologyService
{
    private readonly ITechnologyRepository _technologyRepository;
    private readonly ILogger<TechnologyService> _logger;

    public TechnologyService(ITechnologyRepository technologyRepository, ILogger<TechnologyService> logger)
    {
        _technologyRepository = technologyRepository;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<TechnologyResponse>>> GetAllTechnologiesAsync()
    {
        var technologies = await _technologyRepository.GetAllAsync();

        var technologiesResponse = technologies.Select(t => new TechnologyResponse(t.Id, t.Name, t.Type));

        return Result.Ok(technologiesResponse);
    }

    public async Task<Result<TechnologyResponse>> GetTechnologyByIdAsync(Guid id)
    {
        var technology = await _technologyRepository.GetByIdAsync(id); 

        if (technology is null)
            return Result.Fail("Technology not found");

        return Result.Ok(new TechnologyResponse(technology.Id, technology.Name, technology.Type));
    }

    public async Task<Result<TechnologyResponse>> AddTechnologyAsync(string name, TechnologyType type)
    {
        var technology = new Technology(name, type);

        await _technologyRepository.AddAsync(technology);

        _logger.LogInformation("Technology {TechnologyName} created successfully with ID {TechnologyId}.", technology.Name, technology.Id);

        return Result.Ok(new TechnologyResponse(technology.Id, technology.Name, technology.Type));

    }

    public async Task<Result> UpdateTechnologyAsync(Guid id, string name, TechnologyType type)
    {
        var technology = await _technologyRepository.GetByIdAsync(id);

        if (technology is null)
        {
            _logger.LogWarning("Failed to update technology with ID {TechnologyId}. Reason: technology not found.", id);
            return Result.Fail("Technology not found");
        }

        technology.Name = name;
        technology.Type = type;

        await _technologyRepository.UpdateAsync(technology);

        _logger.LogInformation("Technology {TechnologyId} successfully updated to {TechnologyName}, {TechnologyType}", technology.Id, technology.Name, technology.Type);

        return Result.Ok();
    }

    public async Task<Result> DeleteTechnologyAsync(Guid id)
    {
        var technology = await _technologyRepository.GetByIdAsync(id);

        if (technology is null)
        {
            _logger.LogWarning("Failed to delete technology with ID {TechnologyId}. Reason: technology not found.", id);
            return Result.Fail("Technology not found");
        }

        await _technologyRepository.DeleteAsync(technology);
        _logger.LogInformation("Technology with ID {TechnologyId} successfully deleted.", technology.Id);
        return Result.Ok();
    }
}
