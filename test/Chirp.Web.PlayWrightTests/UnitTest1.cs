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
//        await Page.GotoAsync("https://bdsagroup4chirprazor.azurewebsites.net");
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
}