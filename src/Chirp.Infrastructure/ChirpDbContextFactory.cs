using Chirp.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

public sealed class ChirpDbContextFactory : IDesignTimeDbContextFactory<ChirpDbContext>
{
    public ChirpDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var cs = config.GetConnectionString("chirp_db")
                 ?? "Data Source=chirp.db"; // safe fallback

        var builder = new DbContextOptionsBuilder<ChirpDbContext>()
            .UseSqlite(cs); 

        return new ChirpDbContext(builder.Options);
    }
}
