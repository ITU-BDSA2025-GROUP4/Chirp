using Chirp.Core.Interfaces;
using Chirp.Infrastructure.Repositories;
using Chirp.Core.Application.Contracts;

using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Data;

namespace Chirp.Infrastructure.Tests;

public class FollowRepositoryTest
{
    private static ChirpDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ChirpDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        var context = new ChirpDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();
        DbInitializer.SeedDatabase(context);
        return context;
    }

    [Fact]
    public async Task GetFollowedAuthorNames_ShouldReturnEmptySet_WhenAuthorDoesNotExist()
    {
        var repo = new FollowRepository(CreateInMemoryContext());
        var result = await repo.GetFollowedAuthorNames(42069);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetFollowedAuthorNames_ShouldReturnEmptySet_WhenAuthorFollowsNone()
    {
        var repo = new FollowRepository(CreateInMemoryContext());
        var result = await repo.GetFollowedAuthorNames(2);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetFollowedAuthorNames_ShouldWork_WhenAuthorExistAndFollows()
    {
        var repo = new FollowRepository(CreateInMemoryContext());
        var result = await repo.GetFollowedAuthorNames(1);

        Assert.Equal(2, result.Count);
        Assert.Contains("Wendell Ballan", result);
        Assert.Contains("Luanna Muro", result);
    }

    [Fact]
    public async Task FollowAsync_ShouldReturnError_WhenFollowerDoesNotExist()
    {
        var repo = new FollowRepository(CreateInMemoryContext());
        var followRequest = new FollowRequest(42069, 1);
        var result = await repo.FollowAsync(followRequest);

        Assert.Equal(FollowResult.FollowerNotFound, result);
    }

    [Fact]
    public async Task FollowAsync_ShouldReturnError_WhenFolloweeDoesNotExist()
    {
        var repo = new FollowRepository(CreateInMemoryContext());
        var followRequest = new FollowRequest(1, 42069);
        var result = await repo.FollowAsync(followRequest);

        Assert.Equal(FollowResult.FolloweeNotFound, result);
    }

    [Fact]
    public async Task FollowAsync_ShouldFunctionAndReturnSuccess_WhenFollowerAndFolloweeExist()
    {
        var repo = new FollowRepository(CreateInMemoryContext());
        var followRequest = new FollowRequest(5, 6);
        var result = await repo.FollowAsync(followRequest);

        Assert.Equal(FollowResult.Success, result);

        var follows = await repo.ReadAll();

        Assert.Contains(follows, f => f.FollowerID == 5 && f.FolloweeID == 6);
    }

    [Fact]
    public async Task FollowAsync_ShouldReturnAlreadyFollowing_WhenFollowerDoesAlreadyFollowsFollowee()
    {
        var repo = new FollowRepository(CreateInMemoryContext());
        var followRequest = new FollowRequest(1, 2);

        var result = await repo.FollowAsync(followRequest);
        Assert.Equal(FollowResult.AlreadyFollowing, result);
    }

    [Fact]
    public async Task UnfollowAsync_ShouldReturnError_WhenFollowerDoesNotExist()
    {
        var repo = new FollowRepository(CreateInMemoryContext());
        var followRequest = new FollowRequest(42069, 1);
        var result = await repo.UnfollowAsync(followRequest);

        Assert.Equal(FollowResult.FollowerNotFound, result);
    }

    [Fact]
    public async Task UnfollowAsync_ShouldReturnError_WhenFolloweeDoesNotExist()
    {
        var repo = new FollowRepository(CreateInMemoryContext());
        var followRequest = new FollowRequest(1, 42069);
        var result = await repo.UnfollowAsync(followRequest);

        Assert.Equal(FollowResult.FolloweeNotFound, result);
    }

    [Fact]
    public async Task UnffollowAsync_ShouldFunctionAndReturnSuccess_WhenFollowerAndFolloweeExist()
    {
        var repo = new FollowRepository(CreateInMemoryContext());
        var followRequest = new FollowRequest(1, 2);

        var result = await repo.UnfollowAsync(followRequest);

        Assert.Equal(FollowResult.Success, result);

        var follows = await repo.ReadAll();
        Assert.DoesNotContain(follows, f => f.FollowerID == 1 && f.FolloweeID == 2);
    }

    [Fact]
    public async Task UnfollowAsync_ShouldReturnNotFollowing_WhenFollowerDoesNotFollowFollowee()
    {
        var repo = new FollowRepository(CreateInMemoryContext());
        var followRequest = new FollowRequest(2, 1);

        var result = await repo.UnfollowAsync(followRequest);
        Assert.Equal(FollowResult.NotFollowing, result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllFollows()
    {
        var repo = new FollowRepository(CreateInMemoryContext());

        var allFollows = await repo.ReadAll();

        Assert.Equal(3, allFollows.Count);
        Assert.Contains(allFollows, f => f.FollowerID == 1 && f.FolloweeID == 2);
        Assert.Contains(allFollows, f => f.FollowerID == 1 && f.FolloweeID == 3);
        Assert.Contains(allFollows, f => f.FollowerID == 3 && f.FolloweeID == 1);
    }
}