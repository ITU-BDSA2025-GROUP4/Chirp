using Moq;

using Chirp.Core.Entities;
using Chirp.Core.Interfaces;
using Chirp.Core.Utils;
using Chirp.Infrastructure.Services;

namespace Chirp.Infrastructure.Tests;

public class AuthorServiceTests
{
    [Fact]
    public async Task GetAuthors_ShouldReturnAllAuthors()
    {
        var expectedAuthors = new List<AuthorDTO>
        {
            new("Peter", "peter@chirp.dk", 1),
            new("Mortem", "morten@chirp.dk", 2)
        };

        var mockRepository = new Mock<IAuthorRepository>();

        mockRepository
            .Setup(repo => repo.ReadAll())
            .ReturnsAsync(expectedAuthors);

        var service = new AuthorService(null!, null!, mockRepository.Object, null!, null!);

        var result = await service.GetAuthorsAsync();

        Assert.Equal(expectedAuthors, result);
        mockRepository.Verify(repo => repo.ReadAll(), Times.Once);
    }

    [Fact]
    public async Task GetAuthors_ShouldReturnEmptyList_WhenNoAuthorsExist()
    {
        var mockRepository = new Mock<IAuthorRepository>();

        mockRepository
            .Setup(repo => repo.ReadAll())
            .ReturnsAsync(new List<AuthorDTO>());

        var service = new AuthorService(null!, null!, mockRepository.Object, null!, null!);

        var result = await service.GetAuthorsAsync();

        Assert.Empty(result);
        mockRepository.Verify(repo => repo.ReadAll(), Times.Once);
    }

    [Fact]
    public async Task FindAuthorByName_ShouldReturnAuthor()
    {
        var expectedAuthor = new AuthorDTO("Alice", "alice@chirp.dk", 3);
        var mockRepository = new Mock<IAuthorRepository>();

        mockRepository
            .Setup(repo => repo.FindByNameAsync("Alice"))
            .ReturnsAsync(Optional.Of(expectedAuthor));

        var service = new AuthorService(null!, null!, mockRepository.Object, null!, null!);

        var result = await service.FindByNameAsync("Alice");

        Assert.True(result.HasValue);
        Assert.Equal(result.Value(), expectedAuthor);
        Assert.True(result.Value().Id > 0);

        mockRepository.Verify(repo => repo.FindByNameAsync("Alice"), Times.Once);
    }

    [Fact]
    public async Task FindAuthorByName_ShouldReturnEmptyOptional_WhenAuthorIsNonExistant()
    {
        var mockRepository = new Mock<IAuthorRepository>();

        mockRepository
            .Setup(repo => repo.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(Optional.Empty<AuthorDTO>());

        var service = new AuthorService(null!, null!, mockRepository.Object, null!, null!);

        var result = await service.FindByNameAsync("Alice");

        Assert.False(result.HasValue);
        mockRepository.Verify(repo => repo.FindByNameAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task FindAuthorByEmail_ShouldReturnAuthor()
    {
        var expectedAuthor = new AuthorDTO("Alice", "alice@chirp.dk", 3);
        var mockRepository = new Mock<IAuthorRepository>();

        mockRepository
            .Setup(repo => repo.FindByEmailAsync("alice@chirp.dk"))
            .ReturnsAsync(Optional.Of(expectedAuthor));

        var service = new AuthorService(null!, null!, mockRepository.Object, null!, null!);

        var result = await service.FindByEmailAsync("alice@chirp.dk");

        Assert.True(result.HasValue);
        Assert.Equal(result.Value(), expectedAuthor);
        Assert.True(result.Value().Id > 0);

        mockRepository.Verify(repo => repo.FindByEmailAsync("alice@chirp.dk"), Times.Once);
    }

    [Fact]
    public async Task FindAuthorByEmail_ShouldReturnEmptyOptional_WhenAuthorIsNonExistant()
    {
        var mockRepository = new Mock<IAuthorRepository>();

        mockRepository
            .Setup(repo => repo.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(Optional.Empty<AuthorDTO>());

        var service = new AuthorService(null!, null!, mockRepository.Object, null!, null!);

        var result = await service.FindByEmailAsync("alice@chirp.dk");

        Assert.False(result.HasValue);
        mockRepository.Verify(repo => repo.FindByEmailAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetAuthorById_ShouldReturnAuthor()
    {
        var expected = new AuthorDTO("Alice", "alice@chirp.dk", 3);
        var repo = new Moq.Mock<IAuthorRepository>();
        repo.Setup(r => r.FindByIdAsync(3))
            .ReturnsAsync(Optional.Of(expected));

        var svc = new AuthorService(null!, null!, repo.Object, null!, null!);

        var result = await svc.FindByIdAsync(3);

        Assert.True(result.HasValue);
        Assert.Equal(expected, result.Value());
        Assert.True(result.Value().Id > 0);
        repo.Verify(r => r.FindByIdAsync(3), Moq.Times.Once);
    }

    [Fact]
    public async Task GetAuthorById_ShouldReturnEmpty_WhenMissing()
    {
        var repo = new Moq.Mock<IAuthorRepository>();
        repo.Setup(r => r.FindByIdAsync(Moq.It.IsAny<int>()))
            .ReturnsAsync(Optional.Empty<AuthorDTO>());

        var svc = new AuthorService(null!, null!, repo.Object, null!, null!);

        var result = await svc.FindByIdAsync(42);

        Assert.False(result.HasValue);
        repo.Verify(r => r.FindByIdAsync(42), Moq.Times.Once);
    }

    [Fact]
    public async Task FindByName_And_FindByEmail_return_same_id()
    {
        var expected = new AuthorDTO("Alice", "alice@chirp.dk", 7);
        var repo = new Moq.Mock<IAuthorRepository>();
        repo.Setup(r => r.FindByNameAsync("Alice"))
            .ReturnsAsync(Optional.Of(expected));
        repo.Setup(r => r.FindByEmailAsync("alice@chirp.dk"))
            .ReturnsAsync(Optional.Of(expected));

        var svc = new AuthorService(null!, null!, repo.Object, null!, null!);

        var byName = await svc.FindByNameAsync("Alice");
        var byEmail = await svc.FindByEmailAsync("alice@chirp.dk");

        Assert.True(byName.HasValue && byEmail.HasValue);
        Assert.Equal(byName.Value().Id, byEmail.Value().Id);
    }
}