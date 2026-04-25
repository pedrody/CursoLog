using CursoLog.WebApi.DTOs;
using CursoLog.WebApi.Extensions.Mappings;
using CursoLog.WebApi.Models;
using CursoLog.WebApi.Repositories.Interfaces;
using CursoLog.WebApi.Services.Interfaces;
using FluentResults;

namespace CursoLog.WebApi.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<Result<CategoryResponse>> GetCategoryByIdAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category is null)
            return Result.Fail("Category not found");

        return Result.Ok(category.ToCategoryResponse());
    }

    public async Task<Result<IEnumerable<CategoryResponse>>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();

        return Result.Ok(categories.Select(c => c.ToCategoryResponse()));
    }

    public async Task<Result<CategoryResponse>> AddCategoryAsync(string name)
    {
        var category = new Category(name);

        await _categoryRepository.AddAsync(category);

        _logger.LogInformation("Category {CategoryName} created successfully with ID {CategoryId}", category.Name, category.Id);

        return Result.Ok(category.ToCategoryResponse());
    }

    public async Task<Result> DeleteCategoryAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category is null)
        {
            _logger.LogWarning("Failed to delete category with ID {CategoryId}. Reason: category not found", id);
         
            return Result.Fail("Category not found");
        }

        await _categoryRepository.DeleteAsync(category);
        _logger.LogInformation("Category with ID {CategoryId} successfully deleted.", category.Id);
        return Result.Ok();
    }

    public async Task<Result> UpdateCategoryAsync(Guid id, string name)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category is null)
        {
            _logger.LogWarning("Failed to update category with ID {CategoryId}. Reason: category not found", id);
            return Result.Fail("Category not found");
        }

        category.Name = name;
        await _categoryRepository.UpdateAsync(category);

        _logger.LogInformation("Category {CategoryId} successfully updated to {CategoryName}", category.Id, category.Name);

        return Result.Ok();
    }
}
