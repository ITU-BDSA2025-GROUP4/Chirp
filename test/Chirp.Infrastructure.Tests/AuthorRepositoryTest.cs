using Microsoft.EntityFrameworkCore;
using Xunit;

using Chirp.Infrastructure.Repositories;
using Chirp.Infrastructure.Data;
using Chirp.Core.Interfaces;
using Chirp.Core.Utils;
using Chirp.Core.Entities;

namespace Chirp.Infrastructure.Tests;

public class AuthorRepositoryTest
{
    private const int expectedNumberOfAuthors = 12;
    private static IAuthorRepository repo;

    public AuthorRepositoryTest()
    {
        string DbPath = StringUtils.UniqueFilePath("./", ".db");
        DbContextOptionsBuilder<ChirpDbContext> optionsBuilder = new();
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
        ChirpDbContext context = new(optionsBuilder.Options);
        context.Database.EnsureCreated();
        DbInitializer.SeedDatabase(context);

        repo = new AuthorRepository(context);
    }

    [Fact]
    async Task ReadsAllInitializerData()
    {
        var result = await repo.ReadAll();

        // Tests run in parallel; AddAuthorTest might run before this and
        // cause the number to be greater than the expected 12
        Assert.True(result.Count() >= expectedNumberOfAuthors);
    }

    [Theory]
    [InlineData("Roger Histand")]
    [InlineData("Luanna Muro")]
    [InlineData("Wendell Ballan")]
    [InlineData("Nathan Sirmon")]
    [InlineData("Quintin Sitts")]
    [InlineData("Mellie Yost")]
    [InlineData("Malcolm Janski")]
    [InlineData("Octavio Wagganer")]
    [InlineData("Johnnie Calixto")]
    [InlineData("Jacqualine Gilcoine")]
    [InlineData("Helge")]
    [InlineData("Adrian")]
    async Task FindsAuthorByName(string name)
    {
        var author = await repo.FindByNameAsync(name);

        Assert.True(author.HasValue);
        Assert.Equal(name, author.Value().Name);
        Assert.True(author.Value().Id > 0);
    }

    [Theory]
    [InlineData("Roger+Histand@hotmail.com")]
    [InlineData("Luanna-Muro@ku.dk")]
    [InlineData("Wendell-Ballan@gmail.com")]
    [InlineData("Nathan+Sirmon@dtu.dk")]
    [InlineData("Quintin+Sitts@itu.dk")]
    [InlineData("Mellie+Yost@ku.dk")]
    [InlineData("Malcolm-Janski@gmail.com")]
    [InlineData("Octavio.Wagganer@dtu.dk")]
    [InlineData("Johnnie+Calixto@itu.dk")]
    [InlineData("Jacqualine.Gilcoine@gmail.com")]
    [InlineData("ropf@itu.dk")]
    [InlineData("adho@itu.dk")]
    async Task FindAuthorsByEmail(string email)
    {
        var author = await repo.FindByEmailAsync(email);

        Assert.True(author.HasValue);
        Assert.Equal(email, author.Value().Email);
        Assert.True(author.Value().Id > 0);
    }

    [Theory]
    [InlineData("wasd@invalid.com")]
    [InlineData("does@not.exist")]
    async Task FindAuthorsByEmailInvalid(string email)
    {
        var author = await repo.FindByEmailAsync(email);

        Assert.False(author.HasValue);
    }

    [Theory]
    [InlineData("Invalid name 1")]
    [InlineData("RANDOM LETTERS")]
    [InlineData("")]
    [InlineData("a")]
    async Task FindAuthorsByNameInvalid(string name)
    {
        var author = await repo.FindByNameAsync(name);

        Assert.False(author.HasValue);
    }

    [Theory]
    [InlineData("name1", "name1@email.com")]
    [InlineData("first last", "example@last.org")]
    [InlineData("first middle last", "first@middle.dk")]
    async Task AddAuthorTest(string name, string email)
    {
        // Should not exist before inserting
        var authorByName = await repo.FindByNameAsync(name);
        var authorByEmail = await repo.FindByEmailAsync(email);

        Assert.False(authorByName.HasValue);
        Assert.False(authorByEmail.HasValue);

        // Provide a placeholder id (0) — repository/DB should assign a real one.
        await repo.AddAuthorAsync(new AuthorDTO(name, email, 0));

        var authorByName2 = await repo.FindByNameAsync(name);
        var authorByEmail2 = await repo.FindByEmailAsync(email);

        Assert.True(authorByName2.HasValue);
        Assert.True(authorByEmail2.HasValue);

        Assert.Equal(name, authorByName2.Value().Name);
        Assert.Equal(email, authorByName2.Value().Email);
        Assert.True(authorByName2.Value().Id > 0);

        Assert.Equal(name, authorByEmail2.Value().Name);
        Assert.Equal(email, authorByEmail2.Value().Email);
        Assert.True(authorByEmail2.Value().Id > 0);

        // Ids from both lookups should match
        Assert.Equal(authorByName2.Value().Id, authorByEmail2.Value().Id);
    }
    
    [Fact]
    public async Task FindAuthorById_returns_author_when_exists()
    {
        var byName = await repo.FindByNameAsync("Roger Histand");
        Assert.True(byName.HasValue);

        var byId = await repo.FindByIdAsync(byName.Value().Id);

        Assert.True(byId.HasValue);
        Assert.Equal(byName.Value().Id, byId.Value().Id);
        Assert.Equal(byName.Value().Name, byId.Value().Name);
        Assert.Equal(byName.Value().Email, byId.Value().Email);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    public async Task FindAuthorById_returns_empty_when_missing(int id)
    {
        var byId = await repo.FindByIdAsync(id);
        Assert.False(byId.HasValue);
    }
    
    [Fact]
    public async Task AddAuthor_then_fetch_by_id_roundtrip()
    {
        var name = "roundtrip";
        var email = "roundtrip@chirp.dk";

        var before = await repo.FindByEmailAsync(email);
        Assert.False(before.HasValue);

        await repo.AddAuthorAsync(new AuthorDTO(name, email, 0));

        var after = await repo.FindByEmailAsync(email);
        Assert.True(after.HasValue);
        Assert.True(after.Value().Id > 0);

        var roundtrip = await repo.FindByIdAsync(after.Value().Id);
        Assert.True(roundtrip.HasValue);
        Assert.Equal(name, roundtrip.Value().Name);
        Assert.Equal(email, roundtrip.Value().Email);
    }
    
    [Fact]
    public async Task ReadAll_produces_unique_positive_ids()
    {
        var all = await repo.ReadAll();
        Assert.All(all, a => Assert.True(a.Id > 0));

        var distinct = all.Select(a => a.Id).Distinct().Count();
        Assert.Equal(all.Count(), distinct);
    }

}
