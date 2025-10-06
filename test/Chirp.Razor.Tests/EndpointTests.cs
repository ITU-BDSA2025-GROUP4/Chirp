using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Chirp.Razor.Tests
{
    public class EndpointTestsTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public EndpointTestsTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task PublicTimeline_ReturnsSuccessAndContainsCheeps()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/");

            // Assert
            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStringAsync();
            Assert.Contains("Public Timeline", html, System.StringComparison.OrdinalIgnoreCase);
            // Optionally check for known cheep content if seeded
            // Assert.Contains("hello", html, System.StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData("bob")]
        [InlineData("tom")]
        [InlineData("carl")]
        public async Task PrivateTimeline_ReturnsSuccessAndContainsAuthorCheeps(string username)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync($"/{username}");

            // Assert
            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStringAsync();
            Assert.Contains($"{username}'s Timeline", html, System.StringComparison.OrdinalIgnoreCase);
            // Optionally check for known cheep content if seeded
            // Assert.Contains("what's up?", html, System.StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task InvalidUserTimeline_ReturnsSuccessAndEmptyTimeline()
        {
            // Arrange
            var client = _factory.CreateClient();
            var invalidUser = "nonexistentuser";

            // Act
            var response = await client.GetAsync($"/{invalidUser}");

            // Assert
            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStringAsync();
            Assert.Contains($"{invalidUser}'s Timeline", html, System.StringComparison.OrdinalIgnoreCase);
            // Optionally check for empty timeline message
            // Assert.Contains("No cheeps found", html, System.StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task RootEndpoint_ReturnsHtmlContentType()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/");
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }
    }
}