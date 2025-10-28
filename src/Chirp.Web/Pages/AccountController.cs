using Chirp.Core.Entities;
using Chirp.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Razor.Pages;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IAccountService accountService, ILogger<AccountController> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    // GET: /Account/Register
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // POST: /Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _accountService.RegisterUserAsync(model);

            if (result.Succeeded)
                return RedirectToAction("RegistrationConfirmation");

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during registration for email: {Email}", model.Email);
            ModelState.AddModelError("", "An unexpected error occured. Please try again later.");
            return View(model);
        }
    }

    // GET: /Account/RegistrationConfirmation
    [HttpGet]
    public IActionResult RegistrationConfirmation()
    {
        return View();
    }

    // GET: /Account/ConfirmEmail
    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(Guid userId, string token)
    {
        try
        {
            if (userId == Guid.Empty || string.IsNullOrEmpty(token))
                return BadRequest("Invalid email confirmation request.");

            var result = await _accountService.ConfirmEmailAsync(userId, token);

            if (result.Succeeded)
                return View("EmailConfirmed");

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View("Error");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error confirming email for UserId: {UserID}", userId);
            ModelState.AddModelError("", "An unexpected error occurred during email confirmation.");
            return View("Error");
        }
    }

    // GET: /Account/Login
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    //POST: /Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _accountService.LoginUserAsync(model);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Profile", "Account"); 
            }

            if (result.IsNotAllowed)
                ModelState.AddModelError("", "Email is not confirmed yet.");
            else
                ModelState.AddModelError("", "Invalid login attampt.");

            return View(model);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during login for email: {Email}", model.Email);
            ModelState.AddModelError("", "An unexpected error occured. Please try again later.");
            return View(model);
        }
    }

    // POST: /Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await _accountService.LogoutUserAsync();
            return RedirectToAction("Index", "Home");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during logout");
            return RedirectToAction("Index", "Home");
        }
    }

    // GET: /Account/ResendEmailConfirmation
    [HttpGet]
    public IActionResult ResendEmailConfirmation()
    {
        return View();
    }

    // POST: /Account/ResendEmailConfirmation
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendEmailConfirmation(ResendConfirmationEmailViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(model);

            await _accountService.SendEmailConfirmationAsync(model.Email);

            ViewBag.Message = "If the emial is registered, a confirmation link has been sent.";
            return View("ResendEmailConfirmationSuccess");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error sending email confirmation to: {Email}", model.Email);
            ModelState.AddModelError("", "An unexpected error occured. Please try again later.");
            return View(model);
        }
    }
}
