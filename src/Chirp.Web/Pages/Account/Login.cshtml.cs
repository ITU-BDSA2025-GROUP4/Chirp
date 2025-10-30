using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.AspNetCore.Identity;

using Chirp.Core.Interfaces;
using Chirp.Infrastructure.Services;
using Chirp.Core.Entities;

namespace Chirp.Razor.Pages;

public class LoginPageModel : PageModel
{
    private readonly IAuthorService _authorService;

    public LoginPageModel(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    [HttpPost]
    public IActionResult OnPostLogin(LoginViewModel login)
    {
        if (!ModelState.IsValid)
        {
            TempData["message"] = "Invalid model state";
            return Redirect("/Account/Login");
        }

        var result = _authorService.LoginUserAsync(login);
        result.Wait();

        if (result.Result.Succeeded)
        {
            Console.WriteLine("Login success");
            return Redirect("/");
        }

        if (result.Result.IsNotAllowed)
            TempData["message"] = "Email is not confirmed yet.";
        else
            TempData["message"] = "Invalid login attampt.";

        //_logger.LogError(e, "Error during login for email: {Email}", model.Email);
        TempData["message"] = $"Error during login for email: {login.Email}";
        return Redirect("/Account/Login");
    }

//    [HttpGet]
//    [Route("/Account/Logout")]
//    public IActionResult OnGet()
//    {
//        if(Request.Path.ToString().EndsWith("Logout")) {
//            Console.WriteLine("LOGOUT FIRED");
//            _authorService.LogoutAuthorAsync();
//            return Redirect("/");
//        }
//        return new OkResult();
//    }

}