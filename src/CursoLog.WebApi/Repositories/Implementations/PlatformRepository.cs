using CursoLog.WebApi.Data;
using CursoLog.WebApi.Models;
using CursoLog.WebApi.Repositories.Interfaces;

namespace CursoLog.WebApi.Repositories.Implementations;

public class PlatformRepository : Repository<Platform>, IPlatformRepository
{
    public PlatformRepository(AppDbContext context) : base(context)
    {
    }
}
