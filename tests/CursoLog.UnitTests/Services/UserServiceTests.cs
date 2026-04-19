using Bogus;
using CursoLog.WebApi.Models;
using CursoLog.WebApi.Services.Implementations;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace CursoLog.UniTests.Services;

[Trait("Category", "UserService")]
public class UserServiceTests
{
    private readonly UserService _userService;
    private readonly UserManager<ApplicationUser> _userManagerMock;
    private readonly Faker _faker;

    public UserServiceTests()
    {
        _faker = new Faker();
        var store = Substitute.For<IUserStore<ApplicationUser>>();
        _userManagerMock = Substitute.For<UserManager<ApplicationUser>>(
            store, null, null, null, null, null, null, null, null);
        _userService = new UserService(_userManagerMock);
    }

    [Fact]
    public async Task GetUserProfileByIdAsync_GivenValidUserId_ThenShouldReturnUserProfile()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new ApplicationUser
        {
            Id = userId,
            UserName = _faker.Internet.UserName(),
            Name = _faker.Name.FirstName(),
            Bio = _faker.Lorem.Word(),
            ProfilePictureUrl = _faker.Internet.Url(),
            RegisteredAt = DateTime.UtcNow,
        };
        _userManagerMock.FindByIdAsync(userId.ToString()).Returns(user);

        // Act
        var result = await _userService.GetUserProfileByIdAsync(userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(user.Id);
        result.Value.Username.Should().Be(user.UserName);
        result.Value.Name.Should().Be(user.Name);
        result.Value.Bio.Should().Be(user.Bio);
        result.Value.ProfilePictureUrl.Should().Be(user.ProfilePictureUrl);
        result.Value.RegisteredAt.Should().Be(user.RegisteredAt);

        await _userManagerMock.Received(1).FindByIdAsync(userId.ToString());
    }

    [Fact]
    public async Task GetUserProfileByIdAsync_GivenUserNotFound_ThenShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userManagerMock.FindByIdAsync(userId.ToString()).ReturnsNull();

        // Act
        var result = await _userService.GetUserProfileByIdAsync(userId);

        // Assert
        result.IsFailed.Should().BeTrue();
        await _userManagerMock.Received(1).FindByIdAsync(userId.ToString());
    }

    [Fact]
    public async Task UpdateUserProfileAsync_GivenValidParameters_ThenShouldReturnSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = _faker.Name.FirstName();
        var bio = _faker.Lorem.Text();
        var profilePictureUrl = _faker.Internet.Url();
        _userManagerMock.FindByIdAsync(userId.ToString()).Returns(new ApplicationUser { Id = userId });
        _userManagerMock.UpdateAsync(Arg.Any<ApplicationUser>()).Returns(IdentityResult.Success);

        // Act
        var result = await _userService.UpdateUserProfileAsync(userId, name, bio, profilePictureUrl);

        // Assert
        result.IsSuccess.Should().BeTrue();

        Received.InOrder(() =>
        {
            _userManagerMock.FindByIdAsync(userId.ToString());
            _userManagerMock.UpdateAsync(Arg.Is<ApplicationUser>(u =>
                u.Id == userId &&
                u.Name == name &&
                u.Bio == bio &&
                u.ProfilePictureUrl == profilePictureUrl));
        });
    }

    [Fact]
    public async Task UpdateUserProfileAsync_GivenUserNotFound_ThenShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = _faker.Name.FirstName();
        var bio = _faker.Lorem.Text();
        var profilePictureUrl = _faker.Internet.Url();
        _userManagerMock.FindByIdAsync(userId.ToString()).ReturnsNull();

        // Act
        var result = await _userService.UpdateUserProfileAsync(userId, name, bio, profilePictureUrl);

        // Assert
        result.IsFailed.Should().BeTrue();
        await _userManagerMock.Received(1).FindByIdAsync(userId.ToString());
    }

    [Fact]
    public async Task UpdateUserProfieAsync_GivenAnyErrorWhenUpdating_ThenShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = _faker.Name.FirstName();
        var bio = _faker.Lorem.Text();
        var profilePictureUrl = _faker.Internet.Url();
        var errors = new IdentityError { Code = "1", Description = "Error" };
        _userManagerMock.FindByIdAsync(userId.ToString()).Returns(new ApplicationUser());
        _userManagerMock.UpdateAsync(Arg.Any<ApplicationUser>()).Returns(IdentityResult.Failed(errors));

        // Act
        var result = await _userService.UpdateUserProfileAsync(userId, name, bio, profilePictureUrl);

        // Assert
        result.IsFailed.Should().BeTrue();
        Received.InOrder(() =>
        {
            _userManagerMock.FindByIdAsync(userId.ToString());
            _userManagerMock.UpdateAsync(Arg.Any<ApplicationUser>());
        });
    }
}
