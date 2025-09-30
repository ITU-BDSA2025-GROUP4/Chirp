using SimpleDB;
using Chirp.Types;

var builder = WebApplication.CreateBuilder(args);

string? envPath = Environment.GetEnvironmentVariable("CHIRPDBPATH");
var dbPath = !string.IsNullOrWhiteSpace(envPath)
    ? envPath
    : Path.Combine(Path.GetTempPath(), "chirp.db");

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<ICheepService>(cheepService => new CheepService(dbPath);


var app = builder.Build();

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