namespace CursoLog.WebApi.Models;

public class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
}
