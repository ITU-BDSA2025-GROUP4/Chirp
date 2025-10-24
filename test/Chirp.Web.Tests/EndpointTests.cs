using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Xunit;

namespace Chirp.Infrastructure.Tests
{
    public class EndpointTestsTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private WebApplicationFactory<Program> _factory;

        public EndpointTestsTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        private void ensureNewDB()
        {

            Environment.SetEnvironmentVariable("CHIRPDBPATH", Path.GetTempFileName());

            _factory = _factory.WithWebHostBuilder(builder => {
                    builder.UseEnvironment("test");
            });
        }

        [Fact]
        public async Task PublicTimeline_ReturnsContent()
        {
            ensureNewDB(); 
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/");

            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStringAsync();
            Assert.Contains("Public Timeline", html, System.StringComparison.OrdinalIgnoreCase);
            // Optionally check for known cheep content if seeded
        }

        [Theory]
        [InlineData("bob")]
        [InlineData("tom")]
        [InlineData("carl")]
        public async Task PrivateTimeline_ReturnsContent(string username)
        {
            ensureNewDB(); 
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/author/{username}");

            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStringAsync();
            Assert.Contains($"{username}'s Timeline", html, System.StringComparison.OrdinalIgnoreCase);
            // Optionally check for known cheep content if seeded
        }

        [Fact]
        public async Task InvalidUserTimeline_ReturnsContent()
        {
            ensureNewDB(); 
            var client = _factory.CreateClient();
            var invalidUser = "nonexistentuser";

            var response = await client.GetAsync($"/author/{invalidUser}");

            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStringAsync();
            Assert.Contains($"{invalidUser}'s Timeline", html, System.StringComparison.OrdinalIgnoreCase);
            // Optionally check for empty timeline message
        }

        [Fact]
        public async Task RootEndpoint_ReturnsContent()
        {
            ensureNewDB(); 
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/");
            Assert.NotNull(response.Content.Headers.ContentType);
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }
    }
}