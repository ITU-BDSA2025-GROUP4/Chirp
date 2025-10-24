using Moq;
using System.Linq.Expressions;

using Chirp.Core.Entities;
using Chirp.Core.Interfaces;
using Chirp.Infrastructure.Services;

namespace Chirp.Core.Tests.Unit;

public class CheepServiceTests
{
    [Fact]
    public async Task GetCheeps_ShouldReturnAllCheeps()
    {
        var expectedCheeps = new List<CheepDTO>
        {
            new("Alice", "Vim > Emacs > Everything else", "2025-10-10T12:00:00Z"),
            new("Bob", "Jarvis, rewrite this project in rust", "2025-10-10T12:30:00Z")
        };

        var mockRepository = new Mock<ICheepRepository>();

        mockRepository
            .Setup(repo => repo.Read(1, 10))
            .ReturnsAsync(expectedCheeps);

        var service = new CheepService(mockRepository.Object);

        var result = await service.GetCheeps(1, 10);

        Assert.Equal(expectedCheeps, result);
        mockRepository.Verify(repo => repo.Read(1, 10), Times.Once);
    }

    [Fact]
    public async Task GetCheepsFromAuthor_ShouldReturnCheepsFromSpecificAuthor()
    {
        var expectedCheeps = new List<CheepDTO>
        {
            new("Alice", "Hmm, what should we write here?", "2025-10-10T14:00:00Z"),
            new("Alice", "Hello, World!", "2025-10-10T18:30:00Z")
        };

        var mockRepository = new Mock<ICheepRepository>();

        mockRepository
            .Setup(repo => repo.Query(It.IsAny<Expression<Func<Cheep, bool>>>(), 1, 10))
            .ReturnsAsync(expectedCheeps);

        var service = new CheepService(mockRepository.Object);

        var result = await service.GetCheepsFromAuthor("Alice", 1, 10);

        Assert.Equal(expectedCheeps, result);
        mockRepository.Verify(r => r.Query(It.IsAny<Expression<Func<Cheep, bool>>>(), 1, 10), Times.Once);
    }

    [Fact]
    public async Task GetCheeps_ShouldReturnEmptyList_WhenNoCheepsExist()
    {
        var mockRepository = new Mock<ICheepRepository>();

        mockRepository
            .Setup(r => r.Read(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new List<CheepDTO>());

        var service = new CheepService(mockRepository.Object);

        var result = await service.GetCheeps(1, 10);

        Assert.Empty(result);
        mockRepository.Verify(repo => repo.Read(1, 10), Times.Once);
    }

    [Fact]
    public async Task GetCheepsFromAuthor_ShouldReturnEmptyList_WhenNoCheepsFromAuthorExists()
    {
        var mockRepository = new Mock<ICheepRepository>();

        mockRepository
            .Setup(repo => repo.Query(It.IsAny<Expression<Func<Cheep, bool>>>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new List<CheepDTO>());

        var service = new CheepService(mockRepository.Object);

        var result = await service.GetCheepsFromAuthor("Alice", 1, 10);

        Assert.Empty(result);
        mockRepository.Verify(r => r.Query(It.IsAny<Expression<Func<Cheep, bool>>>(), 1, 10), Times.Once);
    }
}