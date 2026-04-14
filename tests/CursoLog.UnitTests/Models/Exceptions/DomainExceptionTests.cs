using Bogus;
using CursoLog.WebApi.Models.Exceptions;
using FluentAssertions;
using Xunit.Abstractions;

namespace CursoLog.UniTests.Models.Exceptions;

[Trait("Category", "DomainException")]
public class DomainExceptionTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void Constructor_GivenNoParams_ThenShouldHaveDefaultMessage()
    {
        // Arrange and Act
        var exception = new DomainException();

        // Assert
        exception.Message.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Constructor_GivenMessage_ThenShouldSetMessageCorrectly()
    {
        // Arrange
        var expectedMessage = _faker.Lorem.Sentences();
       
        // Act
        var exception = new DomainException(expectedMessage);

        // Assert
        exception.Message.Should().Be(expectedMessage);
    }

    [Fact]
    public void Constructor_GivenMessageAndInnerException_ThenShouldSetPropertiesCorrectly()
    {
        // Arrange
        var expectedMessage = _faker.Lorem.Sentences();
        var expectedInnerException = new Exception(_faker.Lorem.Sentence());

        // Act
        var exception = new DomainException(expectedMessage, expectedInnerException);

        // Assert
        exception.Message.Should().Be(expectedMessage);
        exception.InnerException.Should().Be(expectedInnerException);
    }
}
