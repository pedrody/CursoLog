using CursoLog.WebApi.Data;
using CursoLog.WebApi.Models;
using CursoLog.WebApi.Repositories.Interfaces;

namespace CursoLog.WebApi.Repositories.Implementations;

public class InstructorRepository : Repository<Instructor>, IInstructorRepository
{
    public InstructorRepository(AppDbContext context) : base(context)
    {
    }
}
