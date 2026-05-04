using CursoLog.WebApi.DTOs;
using CursoLog.WebApi.Extensions.Mappings;
using CursoLog.WebApi.Models;
using CursoLog.WebApi.Models.Errors;
using CursoLog.WebApi.Repositories.Interfaces;
using CursoLog.WebApi.Services.Interfaces;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace CursoLog.WebApi.Services.Implementations;

public class InstructorService : IInstructorService
{
    private readonly IInstructorRepository _instructorRepository;
    private readonly ILogger<InstructorService> _logger;

    public InstructorService(IInstructorRepository instructorRepository, ILogger<InstructorService> logger)
    {
        _instructorRepository = instructorRepository;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<InstructorResponse>>> GetAllInstructorsAsync()
    {
        var instructors = await _instructorRepository.GetAllAsync();

        var instructorsResponse = instructors.Select(i => i.ToInstructorResponse());

        return Result.Ok(instructorsResponse);

    }

    public async Task<Result<InstructorResponse>> GetInstructorByIdAsync(Guid id)
    {
        var instructor = await _instructorRepository.GetByIdAsync(id);

        if (instructor is null)
            return Result.Fail(new NotFoundError("Instructor not found"));

        return Result.Ok(instructor.ToInstructorResponse());
    }

    public async Task<Result<InstructorResponse>> AddInstructorAsync(string name, string? bio, string? profileUrl)
    {
        var instructor = new Instructor(name, bio, profileUrl);

        await _instructorRepository.AddAsync(instructor);

        _logger.LogInformation("Instructor {InstructorName} created successfully with ID {InstructorId}.", instructor.Name, instructor.Id);

        return Result.Ok(instructor.ToInstructorResponse());
    }

    public async Task<Result> UpdateInstructorAsync(Guid id, string name, string? bio, string? profileUrl)
    {
        var instructor = await _instructorRepository.GetByIdAsync(id);

        if (instructor is null)
        {
            _logger.LogWarning("Failed to update instructor with ID {InstructorId}. Reason: instructor not found.", id);
            return Result.Fail(new NotFoundError("Instructor not found"));
        }

        instructor.Name = name;
        instructor.Bio = bio;
        instructor.ProfileUrl = profileUrl;

        await _instructorRepository.UpdateAsync(instructor);

        _logger.LogInformation(
            "Instructor {InstructorId} successfully updated to {InstructorName}, {InstructorBio}, {InstructorProfileUrl}.", 
            instructor.Id, instructor.Name, instructor.Bio, instructor.ProfileUrl);

        return Result.Ok();
    }

    public async Task<Result> DeleteInstructorAsync(Guid id)
    {
        var instructor = await _instructorRepository.GetByIdAsync(id);

        if (instructor is null)
        {
            _logger.LogWarning("Failed to delete instructor with ID {InstructorId}. Reason: instructor not found.", id);
            return Result.Fail(new NotFoundError("Instructor not found"));
        }

        try
        {
            await _instructorRepository.DeleteAsync(instructor);
        }
        catch (DbUpdateException)
        {
            _logger.LogError("Failed to delete instructor with ID {InstructorId}. Reason: being used by one or more courses.", instructor.Id);
            return Result.Fail($"Instructor '{instructor.Name}' cannot be deleted because it is being used by one or more courses");
        }

        _logger.LogInformation("Instructor with ID {InstructorId} successfully deleted.", instructor.Id);
        return Result.Ok();
    }
}
