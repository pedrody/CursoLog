using Bogus;
using CursoLog.WebApi.Models;
using CursoLog.WebApi.Repositories.Interfaces;
using CursoLog.WebApi.Services.Implementations;
using CursoLog.WebApi.Services.Interfaces;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace CursoLog.UniTests.Services;

[Trait("Category", "CategoryService")]
public class CategoryServiceTests
{
    private readonly ICategoryService _categoryService;
    private readonly ICategoryRepository _categoryRepository;

    public CategoryServiceTests()
    {
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _categoryService = new CategoryService(_categoryRepository);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_GivenCategoryFoundById_ThenShouldReturnSuccessAndCategoryResponse()
    {
        // Arrange
        var category = new Category("category name");
        _categoryRepository.GetByIdAsync(category.Id).Returns(category);

        // Act
        var result = await _categoryService.GetCategoryByIdAsync(category.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(category.Id);
        result.Value.Name.Should().Be(category.Name);

        await _categoryRepository.Received(1).GetByIdAsync(category.Id);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_GivenCategoryNotFoundById_ThenShouldReturnFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        _categoryRepository.GetByIdAsync(id).ReturnsNull();

        // Act
        var result = await _categoryService.GetCategoryByIdAsync(id);

        // Assert
        result.IsFailed.Should().BeTrue();
        await _categoryRepository.Received(1).GetByIdAsync(id);
    }

    [Fact]
    public async Task GetAllCategoriesAsync_GivenCategoriesExist_ThenShouldReturnSuccessWithMappedResponses()
    {
        // Arrange
        List<Category> categories = [
            new("category name")
        ];
        _categoryRepository.GetAllAsync().Returns(categories);

        // Act
        var result = await _categoryService.GetAllCategoriesAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Any(c => c.Name == "category name").Should().BeTrue();
        await _categoryRepository.Received(1).GetAllAsync();
    }

    [Fact]
    public async Task GetAllCategoriesAsync_GivenNoCategoriesExist_ThenShouldReturnEmptyList()
    {
        // Arrange
        _categoryRepository.GetAllAsync().Returns(Enumerable.Empty<Category>());

        // Act
        var result = await _categoryService.GetAllCategoriesAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
        await _categoryRepository.Received(1).GetAllAsync();
    }

    [Fact]
    public async Task AddCategoryAsync_GivenName_ThenShouldAddAndReturnSuccessWithMappedResponse()
    {
        // Arrange
        var categoryName = "category name";

        // Act
        var result = await _categoryService.AddCategoryAsync(categoryName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _categoryRepository.Received(1).AddAsync(Arg.Is<Category>(c => c.Name == categoryName));
    }

    [Fact]
    public async Task DeleteCategoryAsync_GivenCategoryFoundById_ThenShouldDeleteAndReturnSuccess()
    {
        // Arrange
        var category = new Category("category name");
        _categoryRepository.GetByIdAsync(category.Id).Returns(category);

        // Act
        var result = await _categoryService.DeleteCategoryAsync(category.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _categoryRepository.Received(1).DeleteAsync(category);
    }

    [Fact]
    public async Task DeleteCategoryAsync_GivenCategoryNotFoundById_ThenShouldReturnFailure()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var result = await _categoryService.DeleteCategoryAsync(id);

        // Assert
        result.IsFailed.Should().BeTrue();
        await _categoryRepository.Received(1).GetByIdAsync(id);
        await _categoryRepository.DidNotReceive().DeleteAsync(Arg.Any<Category>());
    }

    [Fact]
    public async Task UpdateCategoryAsync_GivenCategoryFoundById_ThenShouldUpdateAndReturnSuccess()
    {
        // Arrange
        var category = new Category("category name");
        _categoryRepository.GetByIdAsync(category.Id).Returns(category);

        // Act
        var result = await _categoryService.UpdateCategoryAsync(category.Id, category.Name);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _categoryRepository.Received(1).GetByIdAsync(category.Id);
        await _categoryRepository.Received(1).UpdateAsync(Arg.Is<Category>(c => c.Id == category.Id && c.Name == category.Name));
    }

    [Fact]
    public async Task UpdateCategoryAsync_GivenCategoryNotFoundById_ThenShouldReturnFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "category name";

        // Act
        var result = await _categoryService.UpdateCategoryAsync(id, name);

        // Assert
        result.IsFailed.Should().BeTrue();
        await _categoryRepository.Received(1).GetByIdAsync(id);
        await _categoryRepository.DidNotReceive().UpdateAsync(Arg.Any<Category>());
    }
}
