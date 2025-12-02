using Moq;
using System.Linq.Expressions;

using Chirp.Core.Entities;
using Chirp.Core.Interfaces;
using Chirp.Core.Application.Contracts;
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
            new(1, cheepId, "ALice", "Hello", "2025-10-10T12:30:00Z")
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


    [Fact]
    public async Task PostReply()
    {
        var createDto = new CreateReplyRequest(1, 1, "Hello");
        var mockRepository = new Mock<IReplyRepository>();

        var replyDto = new ReplyDTO(0, 1, "Bob", "Hello", "2025-10-10T12:30:00Z");

        mockRepository
            .Setup(repo => repo.CreateAsync(createDto))
            .ReturnsAsync(AppResult<ReplyDTO>.Created(replyDto, null));

        mockRepository
            .Setup(repo => repo.ReadAsync(1))
            .ReturnsAsync(new List<ReplyDTO>{replyDto});

        var service = new ReplyService(mockRepository.Object);

        await service.PostReplyAsync(createDto);

        var replies = await service.GetReplies(1);
        Assert.Single(replies);
        Assert.Equal(replyDto, replies.First());
    }

}