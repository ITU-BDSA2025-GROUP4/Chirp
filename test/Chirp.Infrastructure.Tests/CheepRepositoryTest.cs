using Microsoft.EntityFrameworkCore;

using Chirp.Infrastructure.Repositories;
using Chirp.Infrastructure.Data;
using Chirp.Core.Utils;
using Chirp.Core.Entities;

namespace Chirp.Infrastructure.Tests;

public class CheepRepostioryTest
{
    private const int ExpectedNumberOfCheeps = 657;
    private readonly CheepRepository _repo;

    public CheepRepostioryTest()
    {
        string DbPath = "tmp.db";
        DbContextOptionsBuilder<ChirpDbContext> optionsBuilder = new();
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
        ChirpDbContext context = new(optionsBuilder.Options);
        context.Database.EnsureCreated();
        DbInitializer.SeedDatabase(context);

        _repo = new CheepRepository(context);
    }

    [Fact]
    public async Task ReadsAllInitializerData()
    {
        var result = await _repo.ReadAll();

        Assert.Equal(ExpectedNumberOfCheeps, result.Count);
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
    public async Task FiltersByAuthor(string name)
    {
        var result = await _repo.QueryAsync(x => x.Author.Name.Equals(name), 1, ExpectedNumberOfCheeps);

        foreach (CheepDTO c in result)
        {
            Assert.Equal(c.Author, name);
        }
    }

    [Theory]
    [InlineData("example", 3)]
    [InlineData("against", 4)]
    [InlineData("small", 9)]
    [InlineData("broke", 1)]
    public async Task FiltersByMessage(string word, int count)
    {
        var result = await _repo.QueryAsync(x => x.Text.ToLower().Contains(word), 1, ExpectedNumberOfCheeps);

        Assert.Equal(result.Count, count);
        foreach (CheepDTO x in result)
        {
            Assert.True(x.Text.Contains(word, StringComparison.CurrentCultureIgnoreCase));
        }
    }

    [Theory]
    [InlineData("2023-08-01 13:13:19")]
    [InlineData("2023-08-01 13:14:34")]
    [InlineData("2023-08-01 13:16:13")]
    public async Task FilterAfterTimestamp(string date)
    {
        DateTime dt = DateTime.Parse(date);
        var result = await _repo.QueryAsync(x => x.Timestamp.CompareTo(dt) >= 0, 1, ExpectedNumberOfCheeps);
      
        foreach (CheepDTO cheep in result)
        {
            var cheepDt = TimestampUtils.DateTimeStringToDateTimeTimeStamp(cheep.Timestamp);
            Assert.True(cheepDt.CompareTo(dt) >= 0);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(8)]
    [InlineData(16)]
    [InlineData(32)]
    [InlineData(64)]
    [InlineData(128)]
    [InlineData(256)]
    [InlineData(512)]
    public async Task ReadsCorrectNumberOfCheepsPerPage(int pageSize)
    {
        var result = await _repo.ReadAsync(1, pageSize);

        Assert.Equal(result.Count, pageSize);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    async Task ReadsCorrectNumberOfCheepsOnDifferentPage(int page)
    {
        var pageSize = 32;
        var result = await _repo.ReadAsync(page, pageSize);

        Assert.Equal(result.Count(), pageSize);
    }

    [Fact]
    public async Task TwoPagesGiveDifferentCheeps()
    {
        var cheeps1 = await _repo.ReadAsync(1, 32);
        var cheeps2 = await _repo.ReadAsync(2, 32);

        foreach (CheepDTO ch in cheeps1)
        {
            Assert.DoesNotContain(ch, cheeps2);
        }
    }
}