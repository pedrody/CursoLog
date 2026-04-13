using Microsoft.AspNetCore.Identity;

namespace CursoLog.WebApi.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }
    public ICollection<Review> Reviews { get; set; } = [];
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public DateTime RegisteredAt { get; set; }
}
