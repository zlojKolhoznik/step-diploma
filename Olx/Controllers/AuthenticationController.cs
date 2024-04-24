using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Olx.Models;
using Olx.ViewModels;

namespace Olx.Controllers;

public class AuthenticationController : Controller
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<AuthenticationController> _logger;
    private readonly IConfiguration _configuration;

    public AuthenticationController(SignInManager<User> signInManager, ILogger<AuthenticationController> logger, IConfiguration configuration, UserManager<User> userManager)
    {
        _signInManager = signInManager;
        _logger = logger;
        _configuration = configuration;
        _userManager = userManager;
    }
    
    [HttpGet]
    public IActionResult Login(string returnUrl = "/")
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        if (!string.IsNullOrWhiteSpace(vm.ErrorMessage))
        {
            ViewData["Error"] = vm.ErrorMessage;
            return View(vm);
        }

        var returnUrl = vm.ReturnUrl ?? Url.Content("~/");
        
        var result = await _signInManager.PasswordSignInAsync(vm.Username, vm.Password, vm.IsPersistent, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            _logger.LogInformation("User logged in.");
            return LocalRedirect(returnUrl);
        }

        ModelState.AddModelError("", "Невірний логін або пароль.");
        ViewData["Error"] = "Невірний логін або пароль.";
        return View(vm);
    }
    
    [HttpGet]
    public async Task<IActionResult> Register(string returnUrl = "/")
    {
        return View(new RegisterViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        if (!string.IsNullOrWhiteSpace(vm.ErrorMessage))
        {
            ViewData["Error"] = vm.ErrorMessage;
            return View(vm);
        }
        
        var returnUrl = vm.ReturnUrl ?? Url.Content("~/");
        var user = new User { UserName = vm.Email, Email = vm.Email, Name = vm.Name};
        var result = await _userManager.CreateAsync(user, vm.Password);
        if (result.Succeeded)
        {
            _logger.LogInformation("User created a new account with password.");
            await _signInManager.SignInAsync(user, isPersistent: vm.IsPersistent);
            return LocalRedirect(returnUrl);
        }
        
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
        
        ViewData["Error"] = "Помилка при реєстрації.";
        return View(vm);
    }
    
    [HttpPost]
    public async Task<IActionResult> ExternalLogin(string provider, string returnUrl = "/")
    {
        var baseUrl = _configuration["application:url"];
        var redirectUrl = baseUrl + Url.Action("ExternalLoginCallback", "Authentication", new { returnUrl });
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
        
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (email is null)
        {
            ViewData["Error"] = "Ми не змогли отримати вашу електронну пошту від " + info.LoginProvider;
            return LocalRedirect(returnUrl);
        }
        
        // Sign in the user with this external login provider if the user already has a login or register new user
        var user = await _signInManager.UserManager.FindByEmailAsync(email) ?? new User { UserName = email, Email = email };
        await _userManager.AddLoginAsync(user, info);
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

    public async Task<IActionResult> Logout(string returnUrl = "/")
    {
        await _signInManager.SignOutAsync();
        return LocalRedirect(returnUrl);
    }
}