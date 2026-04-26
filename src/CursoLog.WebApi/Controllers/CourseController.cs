using CursoLog.WebApi.Constants;
using CursoLog.WebApi.DTOs.Course;
using CursoLog.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CursoLog.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("/api/[controller]")]
public class CourseController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CourseController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CourseDetailResponse>> GetCourseByIdAsync(Guid id)
    {
        var result = await _courseService.GetCourseByIdAsync(id);

        if (result.IsFailed)
            return NotFound();

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseListResponse>>> GetAllCoursesAsync()
    {
        var result = await _courseService.GetAllCoursesAsync();

        if (result.IsFailed) 
            return BadRequest();

        return Ok(result.Value);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    public async Task<ActionResult<CourseDetailResponse>> AddCourseAsync([FromBody] CreateCourseRequest request)
    {
        var result = await _courseService.AddCourseAsync(
            request.Name,
            request.Description,
            request.Url,
            request.PriceRange,
            request.DurationRange,
            request.Language,
            request.ThumbnailUrl,
            request.InstructorId,
            request.PlatformId,
            request.CategoryId,
            request.TechnologiesId);

        if (result.IsFailed)
            return BadRequest(new { errors = result.Errors.Select(e => e.Message) });

        return Ok(result.Value);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateCourseAsync(Guid id, [FromBody] UpdateCourseRequest request)
    {
        var result = await _courseService.UpdateCourseAsync(
            id,
            request.Name,
            request.Description,
            request.PriceRange,
            request.DurationRange,
            request.TechnologiesId
        );

        if (result.IsFailed)
            return BadRequest(new { errors = result.Errors.Select(e => e.Message) });

        return NoContent();
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteCourseAsync(Guid id)
    {
        var result = await _courseService.DeleteCourseAsync(id);

        if (result.IsFailed)
            return NotFound();

        return NoContent();
    }
}
