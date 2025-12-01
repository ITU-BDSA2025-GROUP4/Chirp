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
        Assert.Equal(0, result.Count);
    }

    [Fact]
    public async Task CreateReplyAndReturnIt(string name)
    {
        CreateReplyRequest dto = new CreateReplyRequest(1, 2, "Reply");

        await _repo.CreateAsync(dto);

        var reply = await _repo.ReadAsync(2);

        Assert.Equal(1, reply.Count());
        Assert.Equal(reply.First().Author, "Alice");
        Assert.Equal(reply.First().CheepId, 2);
        Assert.Equal(reply.First().Text, "Reply");
    }

    [Fact]
    public async Task CreateMultipleReplies()
    {
        var allAuthors = await authorRepo.ReadAll();
    }
}