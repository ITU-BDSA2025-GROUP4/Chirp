using Microsoft.EntityFrameworkCore;
using Chirp.Razor.Data;
using Chirp.Razor.Repositories;

var builder = WebApplication.CreateBuilder(args);

string? envPath = Environment.GetEnvironmentVariable("CHIRPDBPATH");
var dbPath = !string.IsNullOrWhiteSpace(envPath)
    ? envPath
    : Path.Combine(Path.GetTempPath(), "chirp.db");

builder.Services.AddRazorPages();
builder.Services.AddDbContext<ChirpDbContext>(options => options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddScoped<ICheepRepository, CheepRepository>();
builder.Services.AddScoped<ICheepService, CheepService>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ChirpDbContext>();

    context.Database.Migrate();

    DbInitializer.SeedDatabase(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.MapRazorPages();

app.Run();

public partial class Program { } // only for endpoint tests