using CursoLog.WebApi.DTOs;
using FluentResults;

namespace CursoLog.WebApi.Services.Interfaces;

public interface IInstructorService
{
    Task<Result<InstructorResponse>> GetInstructorByIdAsync(Guid id);
    Task<Result<IEnumerable<InstructorResponse>>> GetAllInstructorsAsync();
    Task<Result<InstructorResponse>> AddInstructorAsync(string name, string? bio, string? profileUrl);
    Task<Result> DeleteInstructorAsync(Guid id);
    Task<Result> UpdateInstructorAsync(Guid id, string name, string? bio, string? profileUrl);
}
