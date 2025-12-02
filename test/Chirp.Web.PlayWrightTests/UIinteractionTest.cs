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

    public async Task Login(string email, string password)
    {
        await Page.GotoAsync("http://localhost:5273/Account/Login");

        await Page.Locator("#Email").FillAsync(email);
        await Page.Locator("#Password").FillAsync(password);

        await Page.Locator("#LoginSubmit").ClickAsync();
    }

    public async Task Register(string name, string email, string password)
    {
        await Page.GotoAsync("http://localhost:5273/Account/Register");

        await Page.Locator("#Username").FillAsync(name);
        await Page.Locator("#Email").FillAsync(email);
        await Page.Locator("#Password").FillAsync(password);
        await Page.Locator("#ConfirmPassword").FillAsync(password);

        await Page.Locator("#RegisterSubmit").ClickAsync();
    }

    [Test]
    public async Task ChangePasswordSuccess()
    {
        string username = "exampleUsername2";
        string email = "example2@service.com";
        string password1 = "a_84zJw!i9Hq14kPgR";
        string password2 = "EXTRAa_84zJw!i9Hq14kPgR";

        // Register with password1 and login
        await Register(username, email, password1);
        await Login(email, password1);

        // We should now be able to see the email used to register in the settings page
        // since we're logged in
        await Page.GotoAsync("http://localhost:5273/Account/Settings");
        string html = await Page.ContentAsync();
        Assert.True(html.Contains(email));

        // Change password and logout
        await Page.Locator("#ChangePasswordPageButton").ClickAsync();
        await Page.Locator("#PreviousPassword").FillAsync(password1);
        await Page.Locator("#NewPassword").FillAsync(password2);
        await Page.Locator("#NewPasswordConfirm").FillAsync(password2);
        await Page.Locator("#ChangePasswordSubmit").ClickAsync();
        await Page.Locator("#LogoutButton").ClickAsync();

        // Attempt to login with previous password
        // This should fail
        await Login(email, password1);
        await Page.GotoAsync("http://localhost:5273/Account/Settings");
        html = await Page.ContentAsync();
        Assert.False(html.Contains(email));

        // Attempt to login with new password
        // This should work
        await Login(email, password2);
        await Page.GotoAsync("http://localhost:5273/Account/Settings");
        html = await Page.ContentAsync();
        Assert.True(html.Contains(email));
    } 

    [Test]
    public async Task ChangePasswordFailure()
    {
        string username = "exampleUsername3";
        string email = "example3@service.com";
        string password1 = "a_84zJw!i9Hq14kPgR";
        string password2 = "EXTRAa_84zJw!i9Hq14kPgR";

        await Register(username, email, password1);
        await Login(email, password1);

        // We should now be able to see the email used to register in the settings page
        // since we're logged in
        await Page.GotoAsync("http://localhost:5273/Account/Settings");
        string html = await Page.ContentAsync();
        Assert.True(html.Contains(email));

        // Attempt to change password, but use incorrect previous password and logout
        await Page.Locator("#ChangePasswordPageButton").ClickAsync();
        await Page.Locator("#PreviousPassword").FillAsync("Incorrect53_2!Dfji!9");
        await Page.Locator("#NewPassword").FillAsync(password2);
        await Page.Locator("#NewPasswordConfirm").FillAsync(password2);
        await Page.Locator("#ChangePasswordSubmit").ClickAsync();
        await Page.Locator("#LogoutButton").ClickAsync();

        // Attempt to login with new password
        // This should fail since the previous password set was incorrect
        await Login(email, password2);
        await Page.GotoAsync("http://localhost:5273/Account/Settings");
        html = await Page.ContentAsync();
        Assert.False(html.Contains(email));

        // Attempt to login with old password
        // This should work since the password change should failed
        await Login(email, password1);
        await Page.GotoAsync("http://localhost:5273/Account/Settings");
        html = await Page.ContentAsync();
        Assert.True(html.Contains(email));
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

        Assert.True(html.Contains("What's on your mind"));
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