using CursoLog.WebApi.Data;
using CursoLog.WebApi.Models;
using CursoLog.WebApi.Repositories.Interfaces;

namespace CursoLog.WebApi.Repositories.Implementations;

public class TechnologyRepository : Repository<Technology>, ITechnologyRepository
{
    public TechnologyRepository(AppDbContext context) : base(context)
    {
    }
}
