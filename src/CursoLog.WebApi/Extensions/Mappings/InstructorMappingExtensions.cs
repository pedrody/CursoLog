using CursoLog.WebApi.DTOs;
using CursoLog.WebApi.Models;

namespace CursoLog.WebApi.Extensions.Mappings;

public static class InstructorMappingExtensions
{
    public static InstructorResponse ToInstructorResponse(this Instructor instructor)
    {
        return new InstructorResponse(instructor.Id, instructor.Name, instructor.Bio, instructor.ProfileUrl);
    }
}
