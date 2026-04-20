using CursoLog.WebApi.DTOs;
using CursoLog.WebApi.Models;
using CursoLog.WebApi.Repositories.Interfaces;
using CursoLog.WebApi.Services.Interfaces;
using FluentResults;

namespace CursoLog.WebApi.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<CategoryResponse>> GetCategoryByIdAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category is null)
            return Result.Fail("Category not found");

        return Result.Ok(new CategoryResponse(category.Id, category.Name));
    }

    public async Task<Result<IEnumerable<CategoryResponse>>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();

        var categoriesResponse = categories.Select(c => new CategoryResponse(c.Id, c.Name));

        return Result.Ok(categoriesResponse);
    }

    public async Task<Result<CategoryResponse>> AddCategoryAsync(string name)
    {
        var category = new Category(name);

        await _categoryRepository.AddAsync(category);

        return Result.Ok(new CategoryResponse(category.Id, category.Name));
    }

    public async Task<Result> DeleteCategoryAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category is null)
            return Result.Fail("Category not found");

        await _categoryRepository.DeleteAsync(category);
        return Result.Ok();
    }

    public async Task<Result> UpdateCategoryAsync(Guid id, string name)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category is null)
            return Result.Fail("Category not found");

        category.Name = name;
        await _categoryRepository.UpdateAsync(category);

        return Result.Ok();
    }
}
