using CursoLog.WebApi.Models;
using Microsoft.AspNetCore.Identity;

namespace CursoLog.WebApi.Services.Interfaces;

public interface ISignInService
{
    Task<bool> CheckPasswordSignInAsync(ApplicationUser user, string password, bool lockoutOnFailure);
}
