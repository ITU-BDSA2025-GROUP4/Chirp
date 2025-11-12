//todo: is there a cleaner way to do DI?
using Chirp.Core.Interfaces;
using Chirp.Core.Entities;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Repositories;
using Chirp.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string? envPath = Environment.GetEnvironmentVariable("CHIRPDBPATH");
var dbPath = !string.IsNullOrWhiteSpace(envPath)
    ? envPath
    : Path.Combine(Path.GetTempPath(), "chirp.db");

builder.Services.AddRazorPages();

builder.Services.AddDbContext<ChirpDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}")
);

builder
    .Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ChirpDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromMinutes(30));

builder.Services.AddIdentityCore<Author>().AddEntityFrameworkStores<ChirpDbContext>().AddApiEndpoints();

builder.Services.ConfigureApplicationCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10);

        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/";
        options.SlidingExpiration = true;
    });


if(string.IsNullOrWhiteSpace(builder.Configuration["AUTHGITHUBCLIENTID"]) ||
    string.IsNullOrWhiteSpace(builder.Configuration["AUTHGITHUBCLIENTSECRET"])
) {
    Console.WriteLine("OAuth client id or client secret missing, Github OAuth will not function");
    OAuthEnabledStatus.IsOAuthEnabled = false;
} else {
    OAuthEnabledStatus.IsOAuthEnabled = true;
    builder.Services
        .AddAuthentication()
        .AddGitHub(options =>
        {
            options.ClientId = builder.Configuration["AUTHGITHUBCLIENTID"]!;
            options.ClientSecret = builder.Configuration["AUTHGITHUBCLIENTSECRET"]!;
            options.CallbackPath = "/signin-github";
            options.Scope.Add("user:email");
        });
}

builder.Services.AddScoped<ICheepRepository, CheepRepository>();
builder.Services.AddScoped<ICheepService, CheepService>();

builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IAuthorService, AuthorService>();

builder.Services.AddScoped<IEmailService, EmailService>();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapIdentityApi<Author>();

app.Run();

public partial class Program { } // only for endpoint tests