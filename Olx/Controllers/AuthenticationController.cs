using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Olx.Models;
using Olx.ViewModels;

namespace Olx.Controllers;

public class AuthenticationController : Controller
{
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(SignInManager<User> signInManager, ILogger<AuthenticationController> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }
    
    [HttpGet]
    public IActionResult Login(string returnUrl = "/")
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    public async Task<IActionResult> Login(string login, string password, bool isPersistent, string returnUrl = "/")
    {
        var result = await _signInManager.PasswordSignInAsync(login, password, isPersistent, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            _logger.LogInformation("User logged in.");
            return LocalRedirect(returnUrl);
        }

        return LocalRedirect($"{returnUrl}?error=Invalid login attempt&login"); // If login fails, redirect back to the login page with an error message
    }
    
    [HttpPost]
    public async Task<IActionResult> ExternalLogin(string provider, string returnUrl = "/")
    {
        var redirectUrl = Url.Action("ExternalLoginCallback", "Authentication", new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return new ChallengeResult(provider, properties);
    }
    
    [HttpGet]
    public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
    {
        returnUrl = returnUrl ?? Url.Content("~/");
        if (remoteError is not null)
        {
            return LocalRedirect($"{returnUrl}?error=Error from external provider: {remoteError}");
        }
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info is null)
        {
            return LocalRedirect($"{returnUrl}?error=Error loading external login information.");
        }

        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        if (result.Succeeded)
        {
            _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
            return LocalRedirect(returnUrl);
        }
        if (result.IsLockedOut)
        {
            return RedirectToPage("./Lockout");
        }
        else
        {
            return LocalRedirect($"{returnUrl}?error=If the user does not have an account, then ask the user to create an account.");
        }
    }
}