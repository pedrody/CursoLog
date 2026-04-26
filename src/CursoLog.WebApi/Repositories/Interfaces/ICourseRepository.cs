using CursoLog.WebApi.Models;

namespace CursoLog.WebApi.Repositories.Interfaces;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(Guid id);
}
