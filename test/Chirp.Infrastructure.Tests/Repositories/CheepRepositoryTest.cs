using System.Linq;
using System.Threading.Tasks;
using Chirp.Core.Application;
using Chirp.Core.Application.Contracts;
using Chirp.Core.Entities;
using Chirp.Core.Utils;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Chirp.Tests.Infrastructure.Repositories
{
    public class CheepRepositoryTests
    {
        private static CheepRepository NewRepo(out ChirpDbContext ctx, out Author author)
        {
            var dbPath = StringUtils.UniqueFilePath("./", ".db");
            var options = new DbContextOptionsBuilder<ChirpDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;

            ctx = new ChirpDbContext(options);
            ctx.Database.EnsureCreated();
            DbInitializer.SeedDatabase(ctx);
            author = ctx.Authors.First();
            return new CheepRepository(ctx);
        }

        [Fact]
        public async Task CreateAsync_returns_created_and_persists()
        {
            var repo = NewRepo(out var ctx, out var author);
            var res = await repo.CreateAsync(new CreateCheepRequest(author.Id, "hello"));

            Assert.Equal(AppStatus.Created, res.Status);
            Assert.NotNull(res.Value);
            Assert.Equal("hello", res.Value.Text);
            Assert.False(string.IsNullOrWhiteSpace(res.ETag));

            var inDb = await ctx.Cheeps
                .Include(c => c.Author)
                .SingleAsync(c => c.Id == ctx.Cheeps.Max(x => x.Id));
            Assert.Equal("hello", inDb.Text);
            Assert.Equal(author.Id, inDb.AuthorId);
        }

        [Fact]
        public async Task UpdateAsync_with_correct_etag_updates()
        {
            var repo = NewRepo(out var ctx, out var author);
            var created = await repo.CreateAsync(new CreateCheepRequest(author.Id, "old"));
            var cheepId = ctx.Cheeps.OrderBy(c => c.Id).Select(c => c.Id).Last();
            var etag = created.ETag;

           foreach (var entry in ctx.ChangeTracker.Entries<Cheep>().ToList())
                entry.State = EntityState.Detached;

            var res = await repo.UpdateAsync(new UpdateCheepRequest(cheepId, author.Id, "new", etag));

            Assert.Equal(AppStatus.Ok, res.Status);
            Assert.NotNull(res.Value);
            Assert.Equal("new", res.Value.Text);
            Assert.False(string.IsNullOrWhiteSpace(res.ETag));
            Assert.NotEqual(etag, res.ETag);

            var inDb = await ctx.Cheeps.FindAsync(cheepId);
            Assert.Equal("new", inDb!.Text);
        }

        [Fact]
        public async Task UpdateAsync_with_wrong_etag_returns_conflict()
        {
            var repo = NewRepo(out var ctx, out var author);
            await repo.CreateAsync(new CreateCheepRequest(author.Id, "text"));
            var cheepId = ctx.Cheeps.OrderBy(c => c.Id).Select(c => c.Id).Last();

            foreach (var entry in ctx.ChangeTracker.Entries<Cheep>().ToList())
                entry.State = EntityState.Detached;

            var res = await repo.UpdateAsync(new UpdateCheepRequest(cheepId, author.Id, "other", "ZmFrZUVUQUc="));

            Assert.Equal(AppStatus.Conflict, res.Status);
            Assert.Null(res.Value);

            var inDb = await ctx.Cheeps.FindAsync(cheepId);
            await ctx.Entry(inDb!).ReloadAsync();   
            Assert.Equal("text", inDb!.Text);
        }

        [Fact]
        public async Task DeleteAsync_unknown_id_returns_not_found()
        {
            var repo = NewRepo(out var ctx, out var author);
            var res = await repo.DeleteAsync(new DeleteCheepRequest(999999, author.Id, null));
            Assert.Equal(AppStatus.NotFound, res.Status);
        }

        [Fact]
        public async Task DeleteAsync_with_wrong_etag_returns_conflict()
        {
            var repo = NewRepo(out var ctx, out var author);
            await repo.CreateAsync(new CreateCheepRequest(author.Id, "to-del"));
            var cheepId = ctx.Cheeps.OrderBy(c => c.Id).Select(c => c.Id).Last();

            var res = await repo.DeleteAsync(new DeleteCheepRequest(cheepId, author.Id, "ZmFrZUVUQUc="));

            Assert.Equal(AppStatus.Conflict, res.Status);
            Assert.NotNull(await ctx.Cheeps.FindAsync(cheepId));
        }

        [Fact]
        public async Task DeleteAsync_with_correct_etag_deletes()
        {
            var repo = NewRepo(out var ctx, out var author);
            var created = await repo.CreateAsync(new CreateCheepRequest(author.Id, "to-del"));
            var cheepId = ctx.Cheeps.OrderBy(c => c.Id).Select(c => c.Id).Last();
            var etag = created.ETag;

            var res = await repo.DeleteAsync(new DeleteCheepRequest(cheepId, author.Id, etag));

            Assert.Equal(AppStatus.NoContent, res.Status);
            Assert.Null(await ctx.Cheeps.FindAsync(cheepId));
        }
    }
}
