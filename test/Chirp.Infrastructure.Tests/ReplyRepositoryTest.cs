using Microsoft.EntityFrameworkCore;
using Moq;

using Chirp.Core.Interfaces;
using Chirp.Infrastructure.Repositories;
using Chirp.Infrastructure.Data;
using Chirp.Core.Utils;
using Chirp.Core.Entities;
using Chirp.Core.Application.Contracts;

namespace Chirp.Infrastructure.Tests;

public class ReplyRepostioryTest
{
    private readonly ReplyRepository _repo;

    ICheepRepository cheepRepo;
    IAuthorRepository authorRepo;

    public ReplyRepostioryTest()
    {
        string DbPath = StringUtils.UniqueFilePath("./", ".db");
        DbContextOptionsBuilder<ChirpDbContext> optionsBuilder = new();
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
        ChirpDbContext context = new(optionsBuilder.Options);
        context.Database.EnsureCreated();
        DbInitializer.SeedDatabase(context);

        authorRepo = new AuthorRepository(context);
        cheepRepo = new CheepRepository(context);

        _repo = new ReplyRepository(context);
    }

    [Fact]
    public async Task ReplyRepostioryIsInitiallyEmpty()
    {
        var result = await _repo.ReadAll();
        // Replies should be empty
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateReplyAndReturnIt()
    {
        int authorId = 1;
        int cheepId = 2;
        string replyText = "Reply";
        CreateReplyRequest dto = new CreateReplyRequest(authorId, cheepId, replyText);

        await _repo.CreateAsync(dto);

        var reply = await _repo.ReadAsync(cheepId);
        Optional<AuthorDTO> authorDTO = await authorRepo.FindByIdAsync(authorId);

        Assert.True(authorDTO.HasValue);
        Assert.Single(reply);
        Assert.Equal(reply.First().Author, authorDTO.Value().Name);
        Assert.Equal(reply.First().CheepId, cheepId);
        Assert.Equal(reply.First().Text, replyText);
    }

    [Fact]
    public async Task CreateMultipleRepliesForSameCheep()
    {
        List<AuthorDTO> allAuthors = await authorRepo.ReadAll();
        var expectedReplies = new List<(AuthorDTO, string)>();

        int cheepId = 2;

        foreach(AuthorDTO author in allAuthors) {
            string replyText = "Reply by " + author.Name;
            expectedReplies.Add((author, replyText));
            var dto = new CreateReplyRequest(author.Id, cheepId, replyText);
            await _repo.CreateAsync(dto);
        }

        IEnumerable<ReplyDTO> replies = await _repo.ReadAsync(cheepId);
        foreach((AuthorDTO, string) expectedReply in expectedReplies) {
            var reply = replies.Where(r => r.Author == expectedReply.Item1.Name).First();
            Assert.NotNull(reply);

            Assert.Equal(reply.Text, expectedReply.Item2);
        }
    }

    [Fact]
    public async Task CreateMultipleRepliesForDifferentCheeps()
    {
        List<AuthorDTO> allAuthors = await authorRepo.ReadAll();
        var expectedReplies = new List<(AuthorDTO, string)>();

        var cheeps = new List<int>{1, 2, 3, 4};

        foreach(AuthorDTO author in allAuthors) {
            string replyText = "Reply by " + author.Name;
            expectedReplies.Add((author, replyText));

            foreach(int cheepId in cheeps) {
                var dto = new CreateReplyRequest(author.Id, cheepId, replyText);
                await _repo.CreateAsync(dto);
            }
        }

        foreach(int cheepId in cheeps) {
            IEnumerable<ReplyDTO> replies = await _repo.ReadAsync(cheepId);
            foreach((AuthorDTO, string) expectedReply in expectedReplies) {
                var reply = replies.Where(r => r.Author == expectedReply.Item1.Name).First();
                Assert.NotNull(reply);

                Assert.Equal(reply.Text, expectedReply.Item2);
            }
        }
    }
}