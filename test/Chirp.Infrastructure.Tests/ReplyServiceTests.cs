using Moq;
using System.Linq.Expressions;

using Chirp.Core.Entities;
using Chirp.Core.Interfaces;
using Chirp.Infrastructure.Services;

namespace Chirp.Infrastructure.Tests;

public class ReplyServiceTests
{
    [Fact]
    public async Task GetReplies_ShouldReturnAllReplies()
    {
        int cheepId = 1;
        var expectedReplies = new List<ReplyDTO>
        {
            new(0, cheepId, "Bob", "MSG", "2"),
            new(1, cheepId, "ALice", "rewrite this project in rust", "2025-10-10T12:30:00Z")
        };

        var mockRepository = new Mock<IReplyRepository>();

        mockRepository
            .Setup(repo => repo.ReadAsync(cheepId))
            .ReturnsAsync(expectedReplies);

        var service = new ReplyService(mockRepository.Object);

        var result = await service.GetReplies(cheepId);

        Assert.Equal(expectedReplies, result);
        mockRepository.Verify(repo => repo.ReadAsync(cheepId), Times.Once);
    }

}