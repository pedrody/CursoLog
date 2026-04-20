using CursoLog.WebApi.DTOs;
using FluentResults;

namespace CursoLog.WebApi.Services.Interfaces;

public interface ICategoryService
{
    Task<Result<CategoryResponse>> GetCategoryByIdAsync(Guid id);
    Task<Result<IEnumerable<CategoryResponse>>> GetAllCategoriesAsync();
    Task<Result<CategoryResponse>> AddCategoryAsync(string name);
    Task<Result> DeleteCategoryAsync(Guid id);
    Task<Result> UpdateCategoryAsync(Guid id, string name);
}
