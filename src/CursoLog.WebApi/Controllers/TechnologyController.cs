using CursoLog.WebApi.Constants;
using CursoLog.WebApi.DTOs;
using CursoLog.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CursoLog.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("/api/[controller]")]
public class TechnologyController : ControllerBase
{
    private readonly ITechnologyService _technologyService;

    public TechnologyController(ITechnologyService technologyService)
    {
        _technologyService = technologyService;
    }

    [HttpGet("{id}", Name = "GetTechnology")]
    public async Task<ActionResult<TechnologyResponse>> GetTechnologyByIdAsync(Guid id)
    {
        var result = await _technologyService.GetTechnologyByIdAsync(id);

        if (result is null)
            return NotFound();

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TechnologyResponse>>> GetAllTechnologiesAsync()
    {
        var result = await _technologyService.GetAllTechnologiesAsync();

        return Ok(result.Value);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    public async Task<ActionResult<TechnologyResponse>> AddTechnologyAsync([FromBody] TechnologyCreateRequest technology)
    {
        var result = await _technologyService.AddTechnologyAsync(technology.Name, technology.Type);

        if (result.IsFailed)
            return BadRequest();

        return new CreatedAtRouteResult("GetTechnology", new { id = result.Value.Id }, result.Value);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{Id}")]
    public async Task<ActionResult> DeleteTechnologyAsync(Guid Id)
    {
        var result = await _technologyService.DeleteTechnologyAsync(Id);

        if (result.IsFailed)
            return NotFound();

        return NoContent();
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCategoryAsync(Guid id, [FromBody] TechnologyUpdateRequest technology)
    {
        var result = await _technologyService.UpdateTechnologyAsync(id, technology.Name, technology.Type);

        if (result.IsFailed)
            return NotFound();

        return NoContent();
    }
}
