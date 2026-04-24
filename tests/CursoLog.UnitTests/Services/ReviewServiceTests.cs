using CursoLog.WebApi.DTOs;
using CursoLog.WebApi.Models;
using CursoLog.WebApi.Models.Enums;
using CursoLog.WebApi.Repositories.Interfaces;
using CursoLog.WebApi.Services.Implementations;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace CursoLog.UniTests.Services;

[Trait("Category", "ReviewService")]
public class ReviewServiceTests
{
    private readonly ReviewService _reviewService;
    private readonly IReviewRepository _reviewRepositoryMock;
    private readonly ICourseRepository _courseRepositoryMock;
    private readonly Course _course;
    private readonly ApplicationUser _user;
    private readonly Review _review;

    public ReviewServiceTests()
    {
        _reviewRepositoryMock = Substitute.For<IReviewRepository>();
        _courseRepositoryMock = Substitute.For<ICourseRepository>();
        var loggerMock = Substitute.For<ILogger<ReviewService>>();
        _reviewService = new ReviewService(_reviewRepositoryMock, _courseRepositoryMock,
            loggerMock);

        _course = new Course(
            "Course name",
            "https://www.example.com/course",
            CoursePriceRange.Free,
            CourseDurationRange.UnderFiveHours,
            CourseLanguage.English,
            "https://www.example.com/thumbnail.jpeg",
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid()
        );

        _user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "username" };

        _review = new(Guid.NewGuid(), 2.5m, "Comment", _user.Id)
        {
            Course = _course,
            User = _user
        };
    }

    [Fact]
    public async Task GetReviewByIdAsync_GivenReviewFoundById_ThenShouldReturnReviewResponse()
    {
        // Arrange
        _reviewRepositoryMock.GetByIdAsync(_review.Id).Returns(_review);

        // Act
        var result = await _reviewService.GetReviewByIdAsync(_review.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();

        await _reviewRepositoryMock.Received(1).GetByIdAsync(_review.Id);

        result.Value.Id.Should().Be(_review.Id);
        result.Value.CourseId.Should().Be(_review.CourseId);
        result.Value.Rating.Should().Be(_review.Rating);
        result.Value.Comment.Should().Be(_review.Comment);
        result.Value.UserId.Should().Be(_review.UserId);
    }

    [Fact]
    public async Task GetReviewByIdAsync_GivenReviewNotFoundById_ThenShouldReturnFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        _reviewRepositoryMock.GetByIdAsync(id).ReturnsNull();

        // Act
        var result = await _reviewService.GetReviewByIdAsync(id);

        // Assert
        result.IsFailed.Should().BeTrue();
        await _reviewRepositoryMock.Received(1).GetByIdAsync(id);
    }

    [Fact]
    public async Task GetReviewsByCourseIdAsync_GivenCourseFoundById_ThenShouldReturnIEnumerableReviewResponse()
    {
        // Arrange
        var reviews = new List<Review> { _review };
        _courseRepositoryMock.GetByIdAsync(_course.Id).Returns(_course);
        _reviewRepositoryMock.GetByCourseIdAsync(_course.Id).Returns(reviews);

        // Act
        var result = await _reviewService.GetReviewsByCourseIdAsync(_course.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _courseRepositoryMock.Received(1).GetByIdAsync(_course.Id);
        await _reviewRepositoryMock.Received(1).GetByCourseIdAsync(_course.Id);
        result.Value.Should().AllBeOfType<ReviewResponse>();
    }

    [Fact]
    public async Task GetReviewsByCourseIdAsync_GivenCourseNotFoundById_ThenShouldReturnFailure()
    {
        // Arrange
        _courseRepositoryMock.GetByIdAsync(_course.Id).ReturnsNull();

        // Act
        var result = await _reviewService.GetReviewsByCourseIdAsync(_course.Id);

        // Assert
        result.IsFailed.Should().BeTrue();
        await _courseRepositoryMock.Received(1).GetByIdAsync(_course.Id);
        await _reviewRepositoryMock.DidNotReceive().GetByCourseIdAsync(Arg.Any<Guid>());
    }

    [Fact]
    public async Task AddReviewAsync_GivenValidParams_ThenShouldAddReviewSuccessfully()
    {
        // Arrange
        _courseRepositoryMock.GetByIdAsync(_course.Id).Returns(_course);
        _reviewRepositoryMock.UserHasReviewAsync(_user.Id, _course.Id).Returns(false);
        _reviewRepositoryMock.AddAsync(Arg.Any<Review>()).Returns(_review);

        // Act
        var result = await _reviewService.AddReviewAsync(_user.Id, _course.Id, _review.Rating, _review.Comment);

        // Assert
        result.IsSuccess.Should().BeTrue();
        Received.InOrder(async () =>
        {
            await _courseRepositoryMock.GetByIdAsync(_course.Id);
            await _reviewRepositoryMock.UserHasReviewAsync(_user.Id, _course.Id);
            await _reviewRepositoryMock.AddAsync(Arg.Is<Review>(
                r => r.CourseId == _course.Id
                    && r.Rating == _review.Rating
                    && r.Comment == _review.Comment
                    && r.UserId == _user.Id));
        });
    }

    [Fact]
    public async Task AddReviewAsync_GivenCourseNotFound_ThenShouldReturnFailureAndNotCreateReview()
    {
        // Arrange
        _courseRepositoryMock.GetByIdAsync(_course.Id).ReturnsNull();

        // Act
        var result = await _reviewService.AddReviewAsync(_user.Id, _course.Id, _review.Rating, _review.Comment);

        // Assert
        result.IsFailed.Should().BeTrue();
        await _courseRepositoryMock.Received(1).GetByIdAsync(_course.Id);
        await _reviewRepositoryMock.DidNotReceive().UserHasReviewAsync(Arg.Any<Guid>(), Arg.Any<Guid>());
        await _reviewRepositoryMock.DidNotReceive().AddAsync(Arg.Any<Review>());
    }

    [Fact]
    public async Task AddReviewAsync_GivenUserAlreadyReviewed_ThenShouldReturnFailureAndNotCreateReview()
    {
        // Arrange
        _courseRepositoryMock.GetByIdAsync(_course.Id).Returns(_course);
        _reviewRepositoryMock.UserHasReviewAsync(_user.Id, _course.Id).Returns(true);

        // Act
        var result = await _reviewService.AddReviewAsync(_user.Id, _course.Id, _review.Rating, _review.Comment);

        // Assert
        result.IsFailed.Should().BeTrue();
        await _courseRepositoryMock.Received(1).GetByIdAsync(_course.Id);
        await _reviewRepositoryMock.Received(1).UserHasReviewAsync(_user.Id, _course.Id);
        await _reviewRepositoryMock.DidNotReceive().AddAsync(Arg.Any<Review>());
    }

    [Theory]
    [InlineData(2.2)]
    [InlineData(-0.1)]
    [InlineData(5.1)]
    public async Task AddReviewAsync_GivenInvalidRating_ThenShouldReturnFailureAndNotCreateReview(decimal invalidRating)
    {
        // Arrange
        _review.Rating = invalidRating;
        _courseRepositoryMock.GetByIdAsync(_course.Id).Returns(_course);
        _reviewRepositoryMock.UserHasReviewAsync(_user.Id, _course.Id).Returns(false);

        // Act
        var result = await _reviewService.AddReviewAsync(_user.Id, _course.Id, _review.Rating, _review.Comment);

        // Assert
        result.IsFailed.Should().BeTrue();
        await _courseRepositoryMock.Received(1).GetByIdAsync(_course.Id);
        await _reviewRepositoryMock.Received(1).UserHasReviewAsync(_user.Id, _course.Id);
        await _reviewRepositoryMock.DidNotReceive().AddAsync(Arg.Any<Review>());
    }

    [Fact]
    public async Task UpdateReviewAsync_GivenValidParams_ThenShouldUpdateSuccessfully()
    {
        // Arrange
        var newValidRating = 3.5m;
        var newComment = "New comment";
        _reviewRepositoryMock.GetByIdAsync(_review.Id).Returns(_review);

        // Act
        var result = await _reviewService.UpdateReviewAsync(_user.Id, _review.Id, newValidRating, newComment);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _reviewRepositoryMock.Received(1).GetByIdAsync(_review.Id);
        await _reviewRepositoryMock.Received(1).UpdateAsync(Arg.Is<Review>(
            r => r.Rating == newValidRating
            && r.Comment == newComment));
    }

    [Fact]
    public async Task UpdateReviewAsync_GivenReviewNotFound_ThenShouldReturnFailure()
    {
        // Arrange
        _reviewRepositoryMock.GetByIdAsync(_review.Id).ReturnsNull();

        // Act
        var result = await _reviewService.UpdateReviewAsync(_user.Id, _review.Id, _review.Rating, _review.Comment);

        // Assert
        result.IsFailed.Should().BeTrue();
        await _reviewRepositoryMock.Received(1).GetByIdAsync(_review.Id);
        await _reviewRepositoryMock.DidNotReceive().UpdateAsync(Arg.Any<Review>());
    }

    [Fact]
    public async Task UpdateReviewAsync_GivenUserWithoutPermission_ThenShouldReturnFailureAndNotUpdateReview()
    {
        // Arrange
        var userWithoutPermissionId = Guid.NewGuid();
        _reviewRepositoryMock.GetByIdAsync(_review.Id).Returns(_review);

        // Act
        var result = await _reviewService.UpdateReviewAsync(userWithoutPermissionId, _review.Id, _review.Rating, _review.Comment);

        // Assert
        result.IsFailed.Should().BeTrue();
        await _reviewRepositoryMock.Received(1).GetByIdAsync(_review.Id);
        await _reviewRepositoryMock.DidNotReceive().UpdateAsync(Arg.Any<Review>());
    }

    [Theory]
    [InlineData(2.2)]
    [InlineData(-0.1)]
    [InlineData(5.1)]
    public async Task UpdateReviewAsync_GivenInvalidRating_ThenShouldReturnFailure(decimal invalidRating)
    {
        // Arrange
        _review.Rating = invalidRating;
        _reviewRepositoryMock.GetByIdAsync(_review.Id).Returns(_review);

        // Act
        var result = await _reviewService.UpdateReviewAsync(_user.Id, _review.Id, _review.Rating, _review.Comment);

        // Assert
        result.IsFailed.Should().BeTrue();
        await _reviewRepositoryMock.Received(1).GetByIdAsync(_review.Id);
        await _reviewRepositoryMock.DidNotReceive().UpdateAsync(Arg.Any<Review>());
    }

    [Fact]
    public async Task DeleteReviewAsync_GivenValidParams_ThenShouldReturnSuccessAndDeleteReview()
    {
        // Arrange
        _reviewRepositoryMock.GetByIdAsync(_review.Id).Returns(_review);

        // Act
        var result = await _reviewService.DeleteReviewAsync(_user.Id, _review.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _reviewRepositoryMock.Received(1).GetByIdAsync(_review.Id);
        await _reviewRepositoryMock.Received(1).DeleteAsync(Arg.Is<Review>(
            r => r.Id == _review.Id 
            && r.UserId == _user.Id));
    }

    [Fact]
    public async Task DeleteReviewAsync_GivenReviewNotFound_ThenShouldReturnFailure()
    {
        // Arrange
        _reviewRepositoryMock.GetByIdAsync(_review.Id).ReturnsNull();

        // Act
        var result = await _reviewService.DeleteReviewAsync(_user.Id, _review.Id);

        // Assert
        result.IsFailed.Should().BeTrue();
        await _reviewRepositoryMock.Received(1).GetByIdAsync(_review.Id);
        await _reviewRepositoryMock.DidNotReceive().DeleteAsync(Arg.Any<Review>());
    }

    [Fact]
    public async Task DeleteReviewAsync_GivenUserWithoutPermission_ThenShouldReturnFailureAndNotDeleteReview()
    {
        // Arrange
        var userWithoutPermission = Guid.NewGuid();
        _reviewRepositoryMock.GetByIdAsync(_review.Id).Returns(_review);

        // Act
        var result = await _reviewService.DeleteReviewAsync(userWithoutPermission, _review.Id);

        // Assert
        result.IsFailed.Should().BeTrue();
        await _reviewRepositoryMock.Received(1).GetByIdAsync(_review.Id);
        await _reviewRepositoryMock.DidNotReceive().DeleteAsync(Arg.Any<Review>());
    }
}
