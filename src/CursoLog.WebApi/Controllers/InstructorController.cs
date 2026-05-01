using CursoLog.WebApi.Constants;
using CursoLog.WebApi.DTOs;
using CursoLog.WebApi.Extensions;
using CursoLog.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CursoLog.WebApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class InstructorController : ControllerBase
{
    private readonly IInstructorService _instructorService;

    public InstructorController(IInstructorService instructorService)
    {
        _instructorService = instructorService;
    }

    [HttpGet("{id}", Name = "GetInstructor")]
    public async Task<ActionResult<InstructorResponse>> GetInstructorByIdAsync(Guid id)
    {
        var result = await _instructorService.GetInstructorByIdAsync(id);

        if (result.IsFailed)
            return result.ToActionResult(this);

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InstructorResponse>>> GetAllInstructorsAsync()
    {
        var result = await _instructorService.GetAllInstructorsAsync();

        return Ok(result.Value);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    public async Task<ActionResult<InstructorResponse>> AddInstructorAsync(
        [FromBody] InstructorCreateRequest instructor)
    {
        var result = await _instructorService.AddInstructorAsync(
            instructor.Name, instructor.Bio, instructor.ProfileUrl);

        return new CreatedAtRouteResult(
            "GetInstructor", new { id = result.Value.Id }, result.Value);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteInstructorAsync(Guid id)
    {
        var result = await _instructorService.DeleteInstructorAsync(id);

        if (result.IsFailed) 
            return result.ToActionResult(this);

        return NoContent();
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateInstructorAsync(
        Guid id, [FromBody] InstructorUpdateRequest instructor)
    {
        var result = await _instructorService.UpdateInstructorAsync(
            id, instructor.Name, instructor.Bio, instructor.ProfileUrl);

        if (result.IsFailed)
            return NotFound();

        return NoContent();
    }
}
