using Bogus;
using CursoLog.WebApi.Models;
using CursoLog.WebApi.Models.Enums;
using CursoLog.WebApi.Repositories.Interfaces;
using CursoLog.WebApi.Services.Implementations;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace CursoLog.UniTests.Services;

[Trait("Category", "CourseService")]
public class CourseServiceTests
{
    private readonly Faker _faker;
    private readonly CourseService _courseService;
    private readonly ICourseRepository _courseRepository;
    private readonly IInstructorRepository _instructorRepository;
    private readonly IPlatformRepository _platformRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITechnologyRepository _technologyRepository;
    private Instructor _instructor;
    private Course _course;
    private Platform _platform;
    private Category _category;
    private Technology _technology;

    public CourseServiceTests()
    {
        _faker = new Faker();
        _courseRepository = Substitute.For<ICourseRepository>();
        _instructorRepository = Substitute.For<IInstructorRepository>();
        _platformRepository = Substitute.For<IPlatformRepository>();
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _technologyRepository = Substitute.For<ITechnologyRepository>();
        var logger = Substitute.For<ILogger<CourseService>>();
        _courseService = new CourseService(
            _courseRepository, 
            _instructorRepository,
            _platformRepository,
            _categoryRepository,
            _technologyRepository,
            logger
        );
        _instructor = new(_faker.Name.FirstName(), _faker.Lorem.Text(), _faker.Internet.Url());
        _platform = new(_faker.Company.CompanyName(), _faker.Internet.Url());
        _category = new(_faker.Name.FirstName());
        _technology = new(_faker.Name.FirstName(), TechnologyType.Language);
        _course = new(
            "course name",
            "course description",
            "www.course.com/csharp",
            CoursePriceRange.AboveOneHundred,
            CourseDurationRange.AboveOneHundredHours,
            CourseLanguage.English,
            "www.course.com/thumbnails/csharp.png",
            _instructor.Id,
            _platform.Id,
            _category.Id
        )
        {
            Instructor = _instructor,
            Platform = _platform,
            Category = _category,
        };
    }

    [Fact]
    public async Task GetCourseByIdAsync_GivenCourseFoundById_ThenShouldReturnSuccessWithCourseResponse()
    {
        // Arrange
        _courseRepository.GetByIdAsync(_course.Id).Returns(_course);

        // Act
        var result = await _courseService.GetCourseByIdAsync(_course.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(_course.Id);
        await _courseRepository.Received(1).GetByIdAsync(_course.Id);
    }

    [Fact]
    public async Task GetCourseByIdAsync_GivenCourseNotFoundById_ThenShouldReturnFailure()
    {
        // Arrange
        _courseRepository.GetByIdAsync(_course.Id).ReturnsNull();

        // Act
        var result = await _courseService.GetCourseByIdAsync(_course.Id);

        // Assert
        result.IsFailed.Should().BeTrue();
        await _courseRepository.Received(1).GetByIdAsync(_course.Id);
    }

    [Fact]
    public async Task GetAllCoursesAsync_GivenCoursesExist_ThenShouldReturnSuccessWithMappedResponses()
    {
        // Arrange
        var courses = new List<Course> { _course };
        _courseRepository.GetAllAsync().Returns(courses);

        // Act
        var result = await _courseService.GetAllCoursesAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Any(c => c.Id == _course.Id);
        await _courseRepository.Received(1).GetAllAsync();
    }

    [Fact]
    public async Task GetAllCoursesAsync_GivenNoCoursesExist_ThenShouldReturnSuccessWithEmptyList()
    {
        // Arrange
        var courses = Enumerable.Empty<Course>();
        _courseRepository.GetAllAsync().Returns(courses);

        // Act
        var result = await _courseService.GetAllCoursesAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
        await _courseRepository.Received(1).GetAllAsync();
    }

    [Fact]
    public async Task AddCourseAsync_GivenValidParams_ThenShouldCreateCourseSuccessfully()
    {
        // Arrange
        _instructorRepository.GetByIdAsync(_instructor.Id).Returns(_instructor);
        _platformRepository.GetByIdAsync(_platform.Id).Returns(_platform);
        _categoryRepository.GetByIdAsync(_category.Id).Returns(_category);
        _technologyRepository.GetByIdAsync(_technology.Id).Returns(_technology);
        _course.Technologies = new List<Technology> { _technology };
        _courseRepository.AddAsync(Arg.Any<Course>()).Returns(_course);

        // Act
        var result = await _courseService.AddCourseAsync(
            _course.Name, 
            _course.Description,
            _course.Url,
            _course.PriceRange,
            _course.DurationRange,
            _course.Language,
            _course.ThumbnailUrl,
            _course.InstructorId,
            _course.PlatformId,
            _course.CategoryId,
            _course.Technologies.Select(t => t.Id)
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(_course.Name);
        result.Value.Description.Should().Be(_course.Description);
        result.Value.Url.Should().Be(_course.Url);
        result.Value.PriceRange.Should().Be(_course.PriceRange);
        result.Value.DurationRange.Should().Be(_course.DurationRange);
        result.Value.Language.Should().Be(_course.Language);
        result.Value.ThumbnailUrl.Should().Be(_course.ThumbnailUrl);
        result.Value.InstructorId.Should().Be(_course.InstructorId);
        result.Value.PlatformId.Should().Be(_course.PlatformId);
        result.Value.CategoryId.Should().Be(_course.CategoryId);
        result.Value.Technologies.Any(t => t.Id == _technology.Id);
        
        await _instructorRepository.Received(1).GetByIdAsync(_instructor.Id);
        await _platformRepository.Received(1).GetByIdAsync(_platform.Id);
        await _categoryRepository.Received(1).GetByIdAsync(_category.Id);
        await _technologyRepository.Received(1).GetByIdAsync(_technology.Id);
        await _courseRepository.Received(1).AddAsync(Arg.Any<Course>());
    }

    [Fact]
    public async Task AddCourseAsync_GivenTechnologiesNotFound_ThenShouldCreateCourseSuccessfullyWithTechnologiesEmpty()
    {
        // Arrange
        _instructorRepository.GetByIdAsync(_instructor.Id).Returns(_instructor);
        _platformRepository.GetByIdAsync(_platform.Id).Returns(_platform);
        _categoryRepository.GetByIdAsync(_category.Id).Returns(_category);
        _technologyRepository.GetByIdAsync(Arg.Any<Guid>()).ReturnsNull();
        _courseRepository.AddAsync(Arg.Any<Course>()).Returns(_course);

        // Act
        var result = await _courseService.AddCourseAsync(
            _course.Name,
            _course.Description,
            _course.Url,
            _course.PriceRange,
            _course.DurationRange,
            _course.Language,
            _course.ThumbnailUrl,
            _course.InstructorId,
            _course.PlatformId,
            _course.CategoryId,
            new List<Guid> { _technology.Id }
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(_course.Name);
        result.Value.Description.Should().Be(_course.Description);
        result.Value.Url.Should().Be(_course.Url);
        result.Value.PriceRange.Should().Be(_course.PriceRange);
        result.Value.DurationRange.Should().Be(_course.DurationRange);
        result.Value.Language.Should().Be(_course.Language);
        result.Value.ThumbnailUrl.Should().Be(_course.ThumbnailUrl);
        result.Value.InstructorId.Should().Be(_course.InstructorId);
        result.Value.PlatformId.Should().Be(_course.PlatformId);
        result.Value.CategoryId.Should().Be(_course.CategoryId);
        result.Value.Technologies.Should().BeEmpty();

        await _instructorRepository.Received(1).GetByIdAsync(_instructor.Id);
        await _platformRepository.Received(1).GetByIdAsync(_platform.Id);
        await _categoryRepository.Received(1).GetByIdAsync(_category.Id);
        await _technologyRepository.Received(1).GetByIdAsync(_technology.Id);
        await _courseRepository.Received(1).AddAsync(Arg.Any<Course>());
    }

    [Fact]
    public async Task AddCourseAsync_GivenInstructorNotFound_ThenShouldReturnFailure()
    {
        // Arrange
        _instructorRepository.GetByIdAsync(_instructor.Id).ReturnsNull();

        // Act
        var result = await _courseService.AddCourseAsync(
            _course.Name,
            _course.Description,
            _course.Url,
            _course.PriceRange,
            _course.DurationRange,
            _course.Language,
            _course.ThumbnailUrl,
            _course.InstructorId,
            _course.PlatformId,
            _course.CategoryId,
            new List<Guid> { _technology.Id }
        );

        // Assert
        result.IsFailed.Should().BeTrue();
        await _instructorRepository.Received(1).GetByIdAsync(_instructor.Id);
        await _courseRepository.DidNotReceive().AddAsync(Arg.Any<Course>());
    }

    [Fact]
    public async Task AddCourseAsync_GivenPlatformNotFound_ThenShouldReturnFailure()
    {
        // Arrange
        _instructorRepository.GetByIdAsync(_instructor.Id).Returns(_instructor);
        _platformRepository.GetByIdAsync(_platform.Id).ReturnsNull();

        // Act
        var result = await _courseService.AddCourseAsync(
            _course.Name,
            _course.Description,
            _course.Url,
            _course.PriceRange,
            _course.DurationRange,
            _course.Language,
            _course.ThumbnailUrl,
            _course.InstructorId,
            _course.PlatformId,
            _course.CategoryId,
            new List<Guid> { _technology.Id }
        );

        // Assert
        result.IsFailed.Should().BeTrue();
        await _platformRepository.Received(1).GetByIdAsync(_platform.Id);
        await _courseRepository.DidNotReceive().AddAsync(Arg.Any<Course>());
    }

    [Fact]
    public async Task AddCourseAsync_GivenCategoryNotFound_ThenShouldReturnFailure()
    {
        // Arrange
        _instructorRepository.GetByIdAsync(_instructor.Id).Returns(_instructor);
        _platformRepository.GetByIdAsync(_platform.Id).Returns(_platform);
        _categoryRepository.GetByIdAsync(_category.Id).ReturnsNull();

        // Act
        var result = await _courseService.AddCourseAsync(
            _course.Name,
            _course.Description,
            _course.Url,
            _course.PriceRange,
            _course.DurationRange,
            _course.Language,
            _course.ThumbnailUrl,
            _course.InstructorId,
            _course.PlatformId,
            _course.CategoryId,
            new List<Guid> { _technology.Id }
        );

        // Assert
        result.IsFailed.Should().BeTrue();
        await _categoryRepository.Received(1).GetByIdAsync(_category.Id);
        await _courseRepository.DidNotReceive().AddAsync(Arg.Any<Course>());
    }

    [Fact]
    public async Task UpdateCourseAsync_GivenCourseFound_ThenShouldUpdateSuccessfully()
    {
        // Arrange
        var newCourseName = "course name updated";
        var newCourseDescription = "course description updated";
        _courseRepository.GetByIdAsync(_course.Id).Returns(_course);
        _technologyRepository.GetByIdAsync(_technology.Id).Returns(_technology);

        // Act
        var result = await _courseService.UpdateCourseAsync(
            _course.Id, 
            newCourseName,
            newCourseDescription,
            _course.PriceRange,
            _course.DurationRange,
            new List<Guid> { _technology.Id }
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _courseRepository.Received(1).GetByIdAsync(_course.Id);
        await _courseRepository.Received(1).UpdateAsync(Arg.Is<Course>(
            c => c.Id == _course.Id 
            && c.Name == newCourseName 
            && c.Description == newCourseDescription
            && c.PriceRange == _course.PriceRange
            && c.DurationRange == _course.DurationRange
            && c.Technologies.Any(t => t.Id == _technology.Id)));
    }

    [Fact]
    public async Task UpdateCourseAsync_GivenCourseNotFound_ThenShouldReturnFailure()
    {
        // Arrange
        _courseRepository.GetByIdAsync(_course.Id).ReturnsNull();

        // Act
        var result = await _courseService.UpdateCourseAsync(
            _course.Id,
            _course.Name,
            _course.Description,
            _course.PriceRange,
            _course.DurationRange,
            _course.Technologies.Select(t => t.Id)
        );

        // Assert
        result.IsFailed.Should().BeTrue();
        await _courseRepository.Received(1).GetByIdAsync(_course.Id);
        await _courseRepository.DidNotReceive().UpdateAsync(Arg.Any<Course>());
    }

    [Fact]
    public async Task DeleteCourseAsync_GivenCourseFound_ThenReturnSuccessAndDeleteSuccessfully()
    {
        // Arrange
        _courseRepository.GetByIdAsync(_course.Id).Returns(_course);

        // Act
        var result = await _courseService.DeleteCourseAsync(_course.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _courseRepository.Received(1).GetByIdAsync(_course.Id);
        await _courseRepository.Received(1).DeleteAsync(Arg.Any<Course>());
    }

    [Fact]
    public async Task DeleteCourseAsync_GivenCourseNotFound_ThenShouldReturnFailure()
    {
        // Arrange
        _courseRepository.GetByIdAsync(_course.Id).ReturnsNull();

        // Act
        var result = await _courseService.DeleteCourseAsync(_course.Id);

        // Assert
        result.IsFailed.Should().BeTrue();
        await _courseRepository.Received(1).GetByIdAsync(_course.Id);
        await _courseRepository.DidNotReceive().DeleteAsync(Arg.Any<Course>());
    }

    [Fact]
    public async Task GetCoursesByFilterAsync_GivenCourseFound_ThenShouldReturnCoursesFiltered()
    {
        // Arrange
        var course1 = new Course(
            "course filtered 1", 
            _course.Description, 
            _course.Url, 
            _course.PriceRange, 
            _course.DurationRange,
            _course.Language,
            _course.ThumbnailUrl, 
            _course.InstructorId, 
            _course.PlatformId, 
            _course.CategoryId)
        {
            Instructor = _instructor,
            Platform = _platform,
            Category = _category,
        };
        var course2 = new Course(
            "course filtered 2",
            _course.Description,
            _course.Url,
            _course.PriceRange,
            _course.DurationRange,
           _course.Language,
            _course.ThumbnailUrl,
            _course.InstructorId,
            _course.PlatformId,
            _course.CategoryId)
        {
            Instructor = _instructor,
            Platform = _platform,
            Category = _category,
        };
        var courses = new List<Course> { course1, course2 };
        _courseRepository.GetByFilterAsync("filtered", null, null, null, null, null, null, null, null).Returns(courses);

        // Act
        var result = await _courseService.GetCoursesByFilterAsync("filtered", null, null, null, null, null, null, null, null);

        // Assert
        result.All(c => c.Name.Contains("filtered"));
        await _courseRepository.Received(1).GetByFilterAsync("filtered", null, null, null, null, null, null, null, null);
    }

    [Fact]
    public async Task GetCoursesByFilterAsync_GivenNoCourseFound_ThenShouldReturnEmptyList()
    {
        // Arrange
        _courseRepository.GetByFilterAsync(null, null, null, null, null, null, null, null, null)
            .Returns(new List<Course>());

        // Act
        var result = await _courseService.GetCoursesByFilterAsync(null, null, null, null, null, null, null, null, null);

        // Assert
        result.Should().BeEmpty();
        await _courseRepository.Received(1).GetByFilterAsync(null, null, null, null, null, null, null, null, null);
    }
}

