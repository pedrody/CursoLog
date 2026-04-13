using CursoLog.WebApi.Models;

namespace CursoLog.WebApi.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(ApplicationUser user, IEnumerable<string> roles);
    string GenerateRefreshToken();
}
