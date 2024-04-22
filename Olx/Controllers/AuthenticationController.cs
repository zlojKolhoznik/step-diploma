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
}