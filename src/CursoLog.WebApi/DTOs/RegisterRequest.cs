using System.ComponentModel.DataAnnotations;

namespace CursoLog.WebApi.DTOs;

public record RegisterRequest(
    [StringLength(30, MinimumLength = 3)] string username, 
    [StringLength(256)] [EmailAddress] string email, 
    [StringLength(100)] string password
);
