using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ExampleTest : PageTest
{
    [Test]
    public async Task HasTitle()
    {
        await Page.GotoAsync("http://localhost:5273");
        await Expect(Page).ToHaveTitleAsync(new Regex("Chirp!"));
    }

    [Test]
    public async Task IncorrectLogin()
    {
        await Page.GotoAsync("http://localhost:5273");

        await Page.GetByText("Login").ClickAsync();

        await Page.Locator("#Email").FillAsync("INVALID USER");
        await Page.Locator("#Password").FillAsync("INVALID PASSWORD");

        await Page.Locator("#LoginSubmit").ClickAsync();

        await Expect(Page.Locator("#WarningMessage")).ToHaveTextAsync("Incorrect password or email");
    } 

    [Test]
    public async Task RegisterAndLoginAndCheep()
    {
        string username = "exampleUsername";
        string email = "example@service.com";
        string password = "a_84zJw!i9Hq14kPgR";
        string cheep = "This cheep doesn't exist yet!";

        await Page.GotoAsync("http://localhost:5273");

        // REGISTER ACCOUNT
        await Page.GetByText("Register").ClickAsync();

        await Page.Locator("#Username").FillAsync(username);
        await Page.Locator("#Email").FillAsync(email);
        await Page.Locator("#Password").FillAsync(password);
        await Page.Locator("#ConfirmPassword").FillAsync(password);

        await Page.Locator("#RegisterSubmit").ClickAsync();

        // LOGIN INTO ACCOUNT
        await Page.Locator("#LoginPageButton").ClickAsync();

        await Page.Locator("#Email").FillAsync(email);
        await Page.Locator("#Password").FillAsync(password);

        await Page.Locator("#LoginSubmit").ClickAsync();

        // CHEEP
        string html = await Page.ContentAsync();

        Assert.True(html.Contains("What's on your mind?"));
        Assert.False(html.Contains(cheep));
        
        await Page.Locator("#cheep").FillAsync(cheep);
        await Page.Locator("#CheepSubmit").ClickAsync();

        string htmlAfterSubmit = await Page.ContentAsync();

        Assert.True(htmlAfterSubmit.Contains(cheep));

        // DELETE ACCOUNT
        await Page.GotoAsync("http://localhost:5273/Account/Settings");
        await Page.Locator("#DeleteAccountButton").ClickAsync();
    } 

    // KEEP THIS HERE FOR DEBUGGGING
    // If a test failed and you aren't sure why, uncomment this code and PlayWright will record a video showing it interacting with the UI,
    // feel free to add sleep calls between calls as PlayWright will perform these tests at inhuman speeds.
//    public override BrowserNewContextOptions ContextOptions()
//    {
//        return new BrowserNewContextOptions()
//        {
//            RecordVideoDir = "videos/",
//            ViewportSize = new ViewportSize
//            {
//                Height = 720, 
//                Width = 1280
//            },
//            RecordVideoSize = new RecordVideoSize
//            {
//                Height = 720, 
//                Width = 1280
//            }
//        };
//    }
}