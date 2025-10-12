using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Chirp.Infrastructure.Tests
{
    public class EndpointTestsTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public EndpointTestsTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PublicTimeline_ReturnsContent()
        {
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
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/{username}");

            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStringAsync();
            Assert.Contains($"{username}'s Timeline", html, System.StringComparison.OrdinalIgnoreCase);
            // Optionally check for known cheep content if seeded
        }

        [Fact]
        public async Task InvalidUserTimeline_ReturnsContent()
        {
            var client = _factory.CreateClient();
            var invalidUser = "nonexistentuser";

            var response = await client.GetAsync($"/{invalidUser}");

            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStringAsync();
            Assert.Contains($"{invalidUser}'s Timeline", html, System.StringComparison.OrdinalIgnoreCase);
            // Optionally check for empty timeline message
        }

        [Fact]
        public async Task RootEndpoint_ReturnsContent()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/");
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }
    }
}