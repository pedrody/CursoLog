using CursoLog.WebApi.DTOs;
using CursoLog.WebApi.Models.Enums;
using FluentResults;

namespace CursoLog.WebApi.Services.Interfaces;

public interface ITechnologyService
{
    Task<Result<TechnologyResponse>> GetTechnologyByIdAsync(Guid id);
    Task<Result<IEnumerable<TechnologyResponse>>> GetAllTechnologiesAsync();
    Task<Result<TechnologyResponse>> AddTechnologyAsync(string name, TechnologyType type, string? logoUrl);
    Task<Result> DeleteTechnologyAsync(Guid id);
    Task<Result> UpdateTechnologyAsync(Guid id, string name, TechnologyType type, string? logoUrl);

}
