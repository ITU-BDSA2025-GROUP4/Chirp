using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Xunit;

namespace Chirp.Infrastructure.Tests
{
    public class CheepPostTest : IClassFixture<WebApplicationFactory<Program>>
    {

        private static readonly string APItoken = "eD[oiaj24_wda=/232)=_1EEdhue]3";
        private WebApplicationFactory<Program> _factory;

        public CheepPostTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            ensureNewDB(); 
        }

        private void ensureNewDB()
        {
            Environment.SetEnvironmentVariable("CHIRPDBPATH", Path.GetTempFileName());

            _factory = _factory.WithWebHostBuilder(builder => {
                    builder.UseEnvironment("test");
            });
        }

        [Fact]
        public async Task AddSingleCheepFromValidAuthor()
        {
            ensureNewDB();

            var client = _factory.CreateClient();

            var authorName = "Jacqualine Gilcoine";
            var cheepContent = "THIS IS A TEST 3";

            var content = new StringContent(
                    $"Cheep={cheepContent}&APItoken={APItoken}",
                    System.Text.Encoding.UTF8,
                    "application/x-www-form-urlencoded"
                );

            // Cheep should not be there before it has been posted
            var authorTimeline = await client.GetAsync($"/author/{authorName}");
            authorTimeline.EnsureSuccessStatusCode();

            var html = await authorTimeline.Content.ReadAsStringAsync();

            // Should still contain author name, just not the cheep
            Assert.Contains(authorName, html, System.StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(cheepContent, html, System.StringComparison.OrdinalIgnoreCase);

            // Post the cheep
            var response = await client.PostAsync("/?handler=Submit", content);

            var responseText = await response.Content.ReadAsStringAsync();

            Console.WriteLine("\n\nRESP TEXT: " + responseText + "\n\n");

            var expectedResponseCode = 200;

            Assert.Equal((int)response.StatusCode, expectedResponseCode);

            // Cheep should now be on the author's timeline
            authorTimeline = await client.GetAsync($"/author/{authorName}");
            authorTimeline.EnsureSuccessStatusCode();

            html = await authorTimeline.Content.ReadAsStringAsync();

            Assert.Contains(authorName, html, System.StringComparison.OrdinalIgnoreCase);
            Assert.Contains(cheepContent, html, System.StringComparison.OrdinalIgnoreCase);

            // Should also be on the first page of the public timeline
            var publicTimeline = await client.GetAsync($"/");
            publicTimeline.EnsureSuccessStatusCode();

            html = await publicTimeline.Content.ReadAsStringAsync();

            Assert.Contains(authorName, html, System.StringComparison.OrdinalIgnoreCase);
            Assert.Contains(cheepContent, html, System.StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task AddSingleCheepFromInvalidAuthor()
        {
            ensureNewDB();
            var client = _factory.CreateClient();

            var invalidToken = "ijdoifjd";
            var cheepContent = "THIS IS A TEST 2";

            var content = new StringContent(
                    $"Cheep={cheepContent.Replace(" ", "%20")}&APItoken={invalidToken}",
                    System.Text.Encoding.UTF8,
                    "application/x-www-form-urlencoded"
                );


            var response = await client.PostAsync("/?handler=Submit", content);

            var timeline = await client.GetAsync("/");
            timeline.EnsureSuccessStatusCode();

            var html = await timeline.Content.ReadAsStringAsync();

            Assert.DoesNotContain(cheepContent, html, System.StringComparison.OrdinalIgnoreCase);
        }

        // Cheep page has been deprecated
        // Cheep bar has been moved to the public timelines page
//        [Fact]
//        public async Task EnsureCheepPageExists()
//        {
//            ensureNewDB();
//
//            var client = _factory.CreateClient();
//
//            var response = await client.GetAsync("/cheep");
//
//            response.EnsureSuccessStatusCode();
//            var html = await response.Content.ReadAsStringAsync();
//            Assert.Contains("Cheep", html, System.StringComparison.OrdinalIgnoreCase);
//        }
    }
}