using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olx.Data;
using Olx.Models;
using Olx.Services.Abstract;

namespace Olx.Controllers;

[Authorize]
public class UserController : Controller
{
    private readonly ShopDbContext _dbContext;
    private readonly IPhotoManager _photoManager;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public UserController(ShopDbContext dbContext, IPhotoManager photoManager, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _dbContext = dbContext;
        _photoManager = photoManager;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public IActionResult FavoriteProducts()
    {
        var products = _dbContext.Products.Include(p => p.FavoredBy);
        var favoriteProducts = products.Where(p => p.FavoredBy!.Any(u => u.UserName == User.Identity!.Name));
        return View(favoriteProducts);
    }

    public IActionResult Settings()
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.UserName == User.Identity!.Name);
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Settings(User user, IFormFile? profilePicture, string? currentPassword,
        string? newPassword, string? confirmPassword)
    {
        var loggedIdUser = _dbContext.Users.FirstOrDefault(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (loggedIdUser is null || loggedIdUser.Email != user.Email)
        {
            return Unauthorized();
        }

        loggedIdUser.Name = user.Name;
        loggedIdUser.Email = user.Email;
        loggedIdUser.PhoneNumber = user.PhoneNumber;
        if (profilePicture is not null)
        {
            loggedIdUser.ProfilePictureUrl = await _photoManager.SavePhotoAsync(profilePicture);
        }

        if (newPassword is not null && confirmPassword is not null)
        {
            var passwordHasher = new PasswordHasher<User>();
            var userHasPassword = loggedIdUser.PasswordHash is not null;
            if (!userHasPassword && confirmPassword == newPassword)
            {
                loggedIdUser.PasswordHash = passwordHasher.HashPassword(loggedIdUser, newPassword);
            }
            else
            {
                var isCorrectPassword = passwordHasher.VerifyHashedPassword(loggedIdUser, loggedIdUser.PasswordHash!,
                    currentPassword ?? "") == PasswordVerificationResult.Success;
                if (isCorrectPassword && newPassword == confirmPassword)
                {
                    loggedIdUser.PasswordHash = passwordHasher.HashPassword(loggedIdUser, newPassword);
                }
            }
        }

        await _dbContext.SaveChangesAsync();
        return RedirectToAction("Settings");
    }

    public async Task<IActionResult> Delete()
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.UserName == User.Identity!.Name);
        if (user is null)
        {
            return Unauthorized();
        }

        var ordersToDelete = _dbContext.Orders
            .Include(o => o.Product)
            .Where(o => o.BuyerId == user.Id || o.Product.OwnerId == user.Id);
        var messagesToDelete = _dbContext.Messages.Where(m => m.SenderId == user.Id || m.ReceiverId == user.Id);
        var filterValuesToDelete = _dbContext.FilterValues
            .Include(fv => fv.Product)
            .Where(fv => fv.Product.OwnerId == user.Id);
        var productsToDelete = _dbContext.Products.Where(p => p.OwnerId == user.Id);
        _dbContext.Orders.RemoveRange(ordersToDelete);
        _dbContext.Messages.RemoveRange(messagesToDelete);
        _dbContext.FilterValues.RemoveRange(filterValuesToDelete);
        _dbContext.Products.RemoveRange(productsToDelete);
        await _dbContext.SaveChangesAsync();
        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            await _signInManager.SignOutAsync();
        }

        return RedirectToAction("Index", "Home");
    }
}