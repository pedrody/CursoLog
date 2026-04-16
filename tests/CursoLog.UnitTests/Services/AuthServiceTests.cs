using Bogus;
using CursoLog.WebApi.Models;
using CursoLog.WebApi.Services.Implementations;
using CursoLog.WebApi.Services.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using FluentResults;
using CursoLog.WebApi.Constants;

namespace CursoLog.UniTests.Services;

[Trait("Category", "AuthService")]
public class AuthServiceTests
{    
    private readonly AuthService _authService;
    private readonly Faker _faker = new();
    private readonly ITokenService _tokenServiceMock;
    private readonly IUserService _userServiceMock;
    private readonly IConfiguration _configurationMock;

    public AuthServiceTests()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            { "Jwt:RefreshTokenExpirationInDays", "7" }
        };

        _configurationMock = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _tokenServiceMock = Substitute.For<ITokenService>();
        _userServiceMock = Substitute.For<IUserService>();
        var signInServiceMock = Substitute.For<ISignInService>();
        _authService = new AuthService(_tokenServiceMock, _configurationMock, _userServiceMock, signInServiceMock);
    }

    [Fact]
    public async Task RegisterAsync_GivenUserParams_ThenShouldCreateUserCorrectly()
    {
        // Arrange
        var username = _faker.Name.FirstName();
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var roles = new List<string> { Roles.User };
        var token = Guid.NewGuid().ToString();
        var refreshToken = Guid.NewGuid().ToString();

        _userServiceMock.CreateAsync(Arg.Any<ApplicationUser>(), password)
            .Returns(Result.Ok());
        _userServiceMock.AddToRoleAsync(Arg.Any<ApplicationUser>(), Roles.User)
            .Returns(Result.Ok());
        _userServiceMock.GetRolesAsync(Arg.Any<ApplicationUser>()).Returns(roles);
        _tokenServiceMock.GenerateToken(Arg.Any<ApplicationUser>(), roles).Returns(token);
        _tokenServiceMock.GenerateRefreshToken().Returns(refreshToken);

        // Act
        var result = await _authService.RegisterAsync(username, email, password);

        // Assert
        Received.InOrder(() =>
        {
            _userServiceMock.CreateAsync(Arg.Any<ApplicationUser>(), password);
            _userServiceMock.AddToRoleAsync(Arg.Any<ApplicationUser>(), Roles.User);
            _userServiceMock.GetRolesAsync(Arg.Any<ApplicationUser>());
            _tokenServiceMock.GenerateToken(Arg.Any<ApplicationUser>(), roles);
            _tokenServiceMock.GenerateRefreshToken();
        });

        result.IsSuccess.Should().BeTrue();
        result.Value.Token.Should().Be(token);
        result.Value.RefreshToken.Should().Be(refreshToken);
    }

    [Fact]
    public async Task RegisterAsync_GivenErrorWhenCreatingUser_ThenShouldReturnFailureAndNotCreateTokens()
    {
        // Arrange
        var username = _faker.Name.FirstName();
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();

        _userServiceMock.CreateAsync(Arg.Any<ApplicationUser>(), password).Returns(Result.Fail("Error when creating user"));

        // Act
        var result = await _authService.RegisterAsync(username, email, password);

        // Assert
        result.IsFailed.Should().BeTrue();

        await _userServiceMock.DidNotReceive().AddToRoleAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>());
        
        await _userServiceMock.DidNotReceive().GetRolesAsync(Arg.Any<ApplicationUser>());
        
        _tokenServiceMock.DidNotReceive().GenerateToken(Arg.Any<ApplicationUser>(), Arg.Any<IList<string>>());
        
        _tokenServiceMock.DidNotReceive().GenerateRefreshToken();
    }

    [Fact]
    public async Task RegisterAsync_GivenRoleAssignmentFails_ThenShouldReturnFailureAndNotCreateTokens()
    {
        // Arrange
        var username = _faker.Name.FirstName();
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();

        _userServiceMock.CreateAsync(Arg.Any<ApplicationUser>(), password).Returns(Result.Ok());
        _userServiceMock.AddToRoleAsync(Arg.Any<ApplicationUser>(), Roles.User).Returns(Result.Fail("Error when assigning role"));

        // Act
        var result = await _authService.RegisterAsync(username, email, password);

        // Assert
        result.IsFailed.Should().BeTrue();

        await _userServiceMock.DidNotReceive().GetRolesAsync(Arg.Any<ApplicationUser>());
        _tokenServiceMock.DidNotReceive().GenerateToken(Arg.Any<ApplicationUser>(), Arg.Any<IList<string>>());
        _tokenServiceMock.DidNotReceive().GenerateRefreshToken();
    }
}

