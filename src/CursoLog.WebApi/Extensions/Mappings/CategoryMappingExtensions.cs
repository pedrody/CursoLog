using CursoLog.WebApi.DTOs;
using CursoLog.WebApi.Models;

namespace CursoLog.WebApi.Extensions.Mappings;

public static class CategoryMappingExtensions
{
    public static CategoryResponse ToCategoryResponse(this Category category)
    {
        return new CategoryResponse(category.Id, category.Name);
    }
}
