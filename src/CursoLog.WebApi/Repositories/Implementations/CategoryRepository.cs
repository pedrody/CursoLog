using CursoLog.WebApi.Data;
using CursoLog.WebApi.Models;
using CursoLog.WebApi.Repositories.Interfaces;

namespace CursoLog.WebApi.Repositories.Implementations;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context)
    {
    }
}
