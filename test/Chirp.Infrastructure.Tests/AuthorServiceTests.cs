using Moq;

using Chirp.Core.Entities;
using Chirp.Core.Interfaces;
using Chirp.Core.Utils;
using Chirp.Core.Interfaces;
using Chirp.Infrastructure.Services;

namespace Chirp.Core.Tests.Unit;

public class AuthorServiceTests
{
    [Fact]
    public async Task GetAuthors_ShouldReturnAllAuthors()
    {
        var expectedAuthors = new List<AuthorDTO>
        {
            new("Peter", "peter@chirp.dk"),
            new("Mortem", "morten@chirp.dk")
        };

        var mockRepository = new Mock<IAuthorRepository>();

        mockRepository
            .Setup(repo => repo.ReadAll())
            .ReturnsAsync(expectedAuthors);

        var service = new AuthorService(mockRepository.Object);

        var result = await service.GetAuthors();

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

        var service = new AuthorService(mockRepository.Object);

        var result = await service.GetAuthors();

        Assert.Empty(result);
        mockRepository.Verify(repo => repo.ReadAll(), Times.Once);
    }

    [Fact]
    public async Task FindAuthorByName_ShouldReturnAuthor()
    {
        var expectedAuthor = new AuthorDTO("Alice", "alice@chirp.dk");
        var mockRepository = new Mock<IAuthorRepository>();

        mockRepository
            .Setup(repo => repo.FindAuthorByName("Alice"))
            .ReturnsAsync(Optional.Of<AuthorDTO>(expectedAuthor));

        var service = new AuthorService(mockRepository.Object);

        var result = await service.GetAuthorByName("Alice");

        Assert.True(result.HasValue);
        Assert.Equal(result.Value(), expectedAuthor);

        mockRepository.Verify(repo => repo.FindAuthorByName("Alice"), Times.Once);
    }

    [Fact]
    public async Task FindAuthorByName_ShouldReturnEmptyOptional_WhenAuthorIsNonExistant()
    {
        var mockRepository = new Mock<IAuthorRepository>();

        mockRepository
            .Setup(repo => repo.FindAuthorByName(It.IsAny<string>()))
            .ReturnsAsync(Optional.Empty<AuthorDTO>());

        var service = new AuthorService(mockRepository.Object);

        var result = await service.GetAuthorByName("Alice");

        Assert.False(result.HasValue);
        mockRepository.Verify(repo => repo.FindAuthorByName(It.IsAny<string>()), Times.Once);
    }


    [Fact]
    public async Task FindAuthorByEmail_ShouldReturnAuthor()
    {
        var expectedAuthor = new AuthorDTO("Alice", "alice@chirp.dk");
        var mockRepository = new Mock<IAuthorRepository>();

        mockRepository
            .Setup(repo => repo.FindAuthorByEmail("alice@chirp.dk"))
            .ReturnsAsync(Optional.Of<AuthorDTO>(expectedAuthor));

        var service = new AuthorService(mockRepository.Object);

        var result = await service.GetAuthorByEmail("alice@chirp.dk");

        Assert.True(result.HasValue);
        Assert.Equal(result.Value(), expectedAuthor);

        mockRepository.Verify(repo => repo.FindAuthorByEmail("alice@chirp.dk"), Times.Once);
    }

    [Fact]
    public async Task FindAuthorByEmail_ShouldReturnEmptyOptional_WhenAuthorIsNonExistant()
    {
        var mockRepository = new Mock<IAuthorRepository>();

        mockRepository
            .Setup(repo => repo.FindAuthorByEmail(It.IsAny<string>()))
            .ReturnsAsync(Optional.Empty<AuthorDTO>());

        var service = new AuthorService(mockRepository.Object);

        var result = await service.GetAuthorByEmail("alice@chirp.dk");

        Assert.False(result.HasValue);
        mockRepository.Verify(repo => repo.FindAuthorByEmail(It.IsAny<string>()), Times.Once);
    }
}