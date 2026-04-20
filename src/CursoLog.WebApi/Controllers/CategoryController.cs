using CursoLog.WebApi.Constants;
using CursoLog.WebApi.DTOs;
using CursoLog.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CursoLog.WebApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("{id}", Name = "GetCategory")]
    public async Task<ActionResult<CategoryResponse>> GetCategoryByIdAsync(Guid id)
    {
        var result = await _categoryService.GetCategoryByIdAsync(id);

        if (result.IsFailed)
            return NotFound();

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetAllCategoriesAsync()
    {
        var result = await _categoryService.GetAllCategoriesAsync();
        return Ok(result.Value);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> AddCategoryAsync([FromBody] CategoryCreateRequest category)
    {
        var result = await _categoryService.AddCategoryAsync(category.Name);

        if (result.IsFailed)
            return BadRequest();

        return new CreatedAtRouteResult("GetCategory", new { id = result.Value.Id }, result.Value);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCategoryAsync(Guid id)
    {
        var result = await _categoryService.DeleteCategoryAsync(id);

        if (result.IsFailed)
            return NotFound();

        return NoContent();
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCategoryAsync(Guid id, [FromBody] CategoryUpdateRequest category)
    {
        var result = await _categoryService.UpdateCategoryAsync(id, category.Name);

        if (result.IsFailed)
            return NotFound();

        return NoContent();
    }
}
