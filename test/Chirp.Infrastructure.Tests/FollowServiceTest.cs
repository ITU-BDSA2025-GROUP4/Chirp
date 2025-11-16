using Chirp.Core.Application.Contracts;
using Chirp.Core.Interfaces;
using Chirp.Infrastructure.Services;

using Moq;

namespace Chirp.Infrastructure.Tests;

public class FollowServiceTests
{
    private readonly Mock<IFollowRepository> _repository;
    private readonly FollowService _service;

    public FollowServiceTests()
    {
        _repository = new Mock<IFollowRepository>();
        _service = new FollowService(_repository.Object);
    }

    [Fact]
    public async Task FollowAuthorAsync_ShouldReturnTrue_ForSuccess()
    {
        var request = new FollowRequest(1, 2);
        _repository.Setup(r => r.FollowAsync(request))
            .ReturnsAsync(FollowResult.Success);

        var result = await _service.FollowAuthorAsync(request);
        Assert.True(result);
        _repository.Verify(repo => repo.FollowAsync(request), Times.Once);
    }

    [Fact]
    public async Task FollowAuthorAsync_ShouldReturnFalse_ForFollowerNotFound()
    {
        var request = new FollowRequest(1, 2);
        _repository.Setup(r => r.FollowAsync(request))
            .ReturnsAsync(FollowResult.FollowerNotFound);

        var result = await _service.FollowAuthorAsync(request);

        Assert.False(result);
        _repository.Verify(repo => repo.FollowAsync(request), Times.Once);
    }

    [Fact]
    public async Task UnfollowAuthorAsync_ShouldReturnTrue_ForSuccess()
    {
        var request = new FollowRequest(1, 2);
        _repository.Setup(r => r.UnfollowAsync(request))
            .ReturnsAsync(FollowResult.Success);

        var result = await _service.UnfollowAuthorAsync(request);

        Assert.True(result);
        _repository.Verify(repo => repo.UnfollowAsync(request), Times.Once);
    }

    [Fact]
    public async Task UnfollowAuthorAsync_ShouldReturnFalse_ForFolloweeNotFound()
    {
        var request = new FollowRequest(1, 2);
        _repository.Setup(r => r.UnfollowAsync(request))
            .ReturnsAsync(FollowResult.FolloweeNotFound);

        var result = await _service.UnfollowAuthorAsync(request);

        Assert.False(result);
        _repository.Verify(repo => repo.UnfollowAsync(request), Times.Once);
    }
}