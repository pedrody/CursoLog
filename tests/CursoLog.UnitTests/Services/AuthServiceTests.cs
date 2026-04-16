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
    private readonly ISignInService _signInServiceMock;

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
        _signInServiceMock = Substitute.For<ISignInService>();
        _authService = new AuthService(_tokenServiceMock, _configurationMock, _userServiceMock, _signInServiceMock);
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

    [Fact]
    public async Task GivenValidCredentials_ThenShouldLoginCorrectly()
    {
        // Arrange
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var user = new ApplicationUser();
        var token = Guid.NewGuid().ToString();
        var refreshToken = Guid.NewGuid().ToString();

        _userServiceMock.FindByEmailAsync(email).Returns(user);
        _signInServiceMock.CheckPasswordSignInAsync(Arg.Any<ApplicationUser>(), password, lockoutOnFailure: false).Returns(true);
        _tokenServiceMock.GenerateToken(user, Arg.Any<IList<string>>()).Returns(token);
        _tokenServiceMock.GenerateRefreshToken().Returns(refreshToken);

        // Act
        var result = await _authService.LoginAsync(email, password);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Token.Should().Be(token);
        result.Value.RefreshToken.Should().Be(refreshToken);

        Received.InOrder(() =>
        {
            _userServiceMock.FindByEmailAsync(email);
            _signInServiceMock.CheckPasswordSignInAsync(Arg.Any<ApplicationUser>(), password, lockoutOnFailure: false);
            _userServiceMock.GetRolesAsync(Arg.Any<ApplicationUser>());
            _tokenServiceMock.GenerateToken(Arg.Any<ApplicationUser>(), Arg.Any<IList<string>>());
            _tokenServiceMock.GenerateRefreshToken();
            _userServiceMock.UpdateAsync(Arg.Is<ApplicationUser>(u => u.RefreshToken == refreshToken));
        });
    }

    [Fact]
    public async Task LoginAsync_GivenAnUserNotFoundByEmail_ThenShouldReturnFailureAndNotGenerateTokens()
    {
        // Arrange
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();

        _userServiceMock.FindByEmailAsync(email).Returns(null as ApplicationUser);

        // Act
        var result = await _authService.LoginAsync(email, password);

        // Assert
        result.IsFailed.Should().BeTrue();

        await _signInServiceMock.DidNotReceive().CheckPasswordSignInAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>(), Arg.Any<bool>());
        await _userServiceMock.DidNotReceive().GetRolesAsync(Arg.Any<ApplicationUser>());
        _tokenServiceMock.DidNotReceive().GenerateToken(Arg.Any<ApplicationUser>(), Arg.Any<IList<string>>());
        _tokenServiceMock.DidNotReceive().GenerateRefreshToken();
        await _userServiceMock.DidNotReceive().UpdateAsync(Arg.Any<ApplicationUser>());
    }

    [Fact]
    public async Task LoginAsync_GivenAnInvalidPassword_ThenShouldReturnFailureAndNotGenerateTokens()
    {
        // Arrange
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var user = new ApplicationUser();

        _userServiceMock.FindByEmailAsync(email).Returns(user);
        _signInServiceMock.CheckPasswordSignInAsync(Arg.Any<ApplicationUser>(), password, Arg.Any<bool>()).Returns(false);

        // Act
        var result = await _authService.LoginAsync(email, password);

        // Assert
        result.IsFailed.Should().BeTrue();

        await _userServiceMock.DidNotReceive().GetRolesAsync(Arg.Any<ApplicationUser>());
        _tokenServiceMock.DidNotReceive().GenerateToken(Arg.Any<ApplicationUser>(), Arg.Any<IList<string>>());
        _tokenServiceMock.DidNotReceive().GenerateRefreshToken();
        await _userServiceMock.DidNotReceive().UpdateAsync(Arg.Any<ApplicationUser>());
    }
}

