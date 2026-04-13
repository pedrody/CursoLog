using Bogus;
using CursoLog.WebApi.Models;
using CursoLog.WebApi.Models.Exceptions;
using FluentAssertions;

namespace CursoLog.UniTests.Models;

[Trait("Category", "Review")]
public class ReviewTests
{
    private readonly Faker _faker = new();

    [Theory]
    [InlineData(0)]
    [InlineData(2.5)]
    [InlineData(5)]
    public void Constructor_GivenValidParams_ThenShouldCreateCorrectly(decimal expectedRating)
    {
        // Arrange
        var expectedCourseId = Guid.NewGuid();
        var expectedComment = _faker.Lorem.Sentence();
        var expectedUserId = Guid.NewGuid();

        // Act
        var review = new Review(
            expectedCourseId,
            expectedRating,
            expectedComment,
            expectedUserId
        );

        // Assert
        review.CourseId.Should().Be(expectedCourseId);
        review.Rating.Should().Be(expectedRating);
        review.Comment.Should().Be(expectedComment);
        review.UserId.Should().Be(expectedUserId);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(5.1)]
    public void Constructor_GivenOutOfRangeRating_ThenShouldThrowDomainException(decimal outOfRangeRating)
    {
        // Arrange
        var anyCourseId = Guid.NewGuid();
        var anyComment = _faker.Lorem.Sentence();
        var anyUserId = Guid.NewGuid();

        // Act
        var act = () => new Review(
            anyCourseId,
            outOfRangeRating,
            anyComment,
            anyUserId
        );

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Rating must be between 0 and 5");
    }

    [Theory]
    [InlineData(2.3)]
    [InlineData(1.7)]
    public void Constructor_GivenNonIncrementalRating_ThenShouldThrowDomainException(decimal nonIncrementalRating)
    {
        // Arrange
        var anyCourseId = Guid.NewGuid();
        var anyComment = _faker.Lorem.Sentence();
        var anyUserId = Guid.NewGuid();

        // Act
        var act = () => new Review(
            anyCourseId,
            nonIncrementalRating,
            anyComment,
            anyUserId
        );

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Rating must be in increments of 0.5");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2.5)]
    [InlineData(5)]
    public void Update_GivenValidParams_ThenShouldUpdatePropertiesCorrectly(decimal expectedRating)
    {
        // Arrange
        var expectedUserId = Guid.NewGuid();
        var expectedCourseId = Guid.NewGuid();
        var expectedComment = _faker.Lorem.Sentence();
        var anyRating = 4.0m;
        var anyComment = "Any initial comment";

        var review = new Review(
            expectedCourseId,
            anyRating,
            anyComment,
            expectedUserId
        );

        // Act
        review.Update(expectedRating, expectedComment);

        // Assert
        review.CourseId.Should().Be(expectedCourseId);
        review.Rating.Should().Be(expectedRating);
        review.Comment.Should().Be(expectedComment);
        review.UserId.Should().Be(expectedUserId);
        review.UpdatedAt.Should().NotBeNull();
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(5.1)]
    public void Update_GivenOutOfRangeRating_ThenShouldThrowDomainException(decimal outOfRangeRating)
    {
        // Arrange
        var anyCourseId = Guid.NewGuid();
        var anyRating = 4.0m;
        var anyComment = _faker.Lorem.Sentence();
        var anyUserId = Guid.NewGuid();
        
        var review = new Review(
            anyCourseId,
            anyRating,
            anyComment,
            anyUserId
        );

        // Act
        var act = () => review.Update(outOfRangeRating, anyComment);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Rating must be between 0 and 5");
    }

    [Theory]
    [InlineData(2.3)]
    [InlineData(1.7)]
    public void Update_GivenNonIncrementalRating_ThenShouldThrowDomainException(decimal nonIncrementalRating)
    {
        // Arrange
        var anyCourseId = Guid.NewGuid();
        var anyRating = 4.0m;
        var anyComment = _faker.Lorem.Sentence();
        var anyUserId = Guid.NewGuid();

        var review = new Review(
            anyCourseId,
            anyRating,
            anyComment,
            anyUserId
        );

        // Act
        var act = () => review.Update(nonIncrementalRating, anyComment);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Rating must be in increments of 0.5");
    }
}
