using Microsoft.EntityFrameworkCore;

using Chirp.Infrastructure.Repositories;
using Chirp.Infrastructure.Data;
using Chirp.Core.Interfaces;
using Chirp.Core.Utils;
using Chirp.Core.Entities;

namespace Chirp.Infrastructure.Tests;

public class AuthorRepostioryTest
{
    private const int expectedNumberOfAuthors = 12;
    private static IAuthorRepository repo;

    public AuthorRepostioryTest()
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

        // Tests in run in parallel the AddAuthorTest might run before this and
        // cause the number to be greater than the expected 12
        Assert.True(result.Count() >= expectedNumberOfAuthors); }

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
        var author = await repo.FindAuthorByName(name);

        Assert.True(author.HasValue);

        Assert.Equal(author.Value().Name, name);
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
        var author = await repo.FindAuthorByEmail(email);

        Assert.True(author.HasValue);

        Assert.Equal(author.Value().Email, email);
    }

    [Theory]
    [InlineData("wasd@invalid.com")]
    [InlineData("does@not.exist")]
    async Task FindAuthorsByEmailInvalid(string email)
    {
        var author = await repo.FindAuthorByEmail(email);

        Assert.False(author.HasValue);
    }

    [Theory]
    [InlineData("Invalid name 1")]
    [InlineData("RANDOM LETTERS")]
    [InlineData("")]
    [InlineData("a")]
    async Task FindAuthorsByNameInvalid(string name)
    {
        var author = await repo.FindAuthorByName(name);

        Assert.False(author.HasValue);
    }

    [Theory]
    [InlineData("name1", "name1@email.com")]
    [InlineData("first last", "example@last.org")]
    [InlineData("first middle last", "first@middle.dk")]
    async Task AddAuthorTest(string name, string email)
    {
        // Should not exist before inserting
        var authorByName = await repo.FindAuthorByName(name);
        var authorByEmail = await repo.FindAuthorByEmail(email);

        Assert.False(authorByName.HasValue);
        Assert.False(authorByEmail.HasValue);

        // Should exist after adding
        await repo.AddAuthor(new AuthorDTO(name, email));

        var authorByName2 = await repo.FindAuthorByName(name);
        var authorByEmail2 = await repo.FindAuthorByEmail(email);

        Assert.True(authorByName2.HasValue);
        Assert.True(authorByEmail2.HasValue);

        Assert.Equal(authorByName2.Value().Name, name);
        Assert.Equal(authorByName2.Value().Email, email);

        Assert.Equal(authorByEmail2.Value().Name, name);
        Assert.Equal(authorByEmail2.Value().Email, email);

    }


}