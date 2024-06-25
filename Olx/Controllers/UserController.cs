using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olx.Data;
using Olx.Models;
using Olx.Services.Abstract;
using Olx.ViewModels;

namespace Olx.Controllers;

[Authorize]
public class UserController : Controller
{
    private readonly ShopDbContext _context;
    private readonly IPhotoManager _photoManager;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public UserController(ShopDbContext context, IPhotoManager photoManager, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _context = context;
        _photoManager = photoManager;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public IActionResult FavoriteProducts()
    {
        var products = _context.Products.Include(p => p.FavoredBy);
        var favoriteProducts = products.Where(p => p.FavoredBy!.Any(u => u.UserName == User.Identity!.Name));
        return View(favoriteProducts);
    }

    public IActionResult Settings()
    {
        var user = _context.Users.FirstOrDefault(u => u.UserName == User.Identity!.Name);
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Settings(User user, IFormFile? profilePicture, string? currentPassword,
        string? newPassword, string? confirmPassword)
    {
        var loggedIdUser = _context.Users.FirstOrDefault(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (loggedIdUser is null || loggedIdUser.Email != user.Email)
        {
            return Unauthorized();
        }

        loggedIdUser.Name = user.Name;
        loggedIdUser.Email = user.Email;
        loggedIdUser.PhoneNumber = user.PhoneNumber;
        if (profilePicture is not null)
        {
            var oldUrl = loggedIdUser.ProfilePictureUrl;
            loggedIdUser.ProfilePictureUrl = await _photoManager.SavePhotoAsync(profilePicture);
            if (oldUrl is not null)
            {
                await _photoManager.DeletePhotoAsync(oldUrl);
            }
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

        await _context.SaveChangesAsync();
        return RedirectToAction("Settings");
    }

    public async Task<IActionResult> Delete()
    {
        var user = _context.Users.FirstOrDefault(u => u.UserName == User.Identity!.Name);
        if (user is null)
        {
            return Unauthorized();
        }

        var ordersToDelete = _context.Orders
            .Include(o => o.Product)
            .Where(o => o.BuyerId == user.Id || o.Product.OwnerId == user.Id);
        var messagesToDelete = _context.Messages.Where(m => m.SenderId == user.Id || m.ReceiverId == user.Id);
        var filterValuesToDelete = _context.FilterValues
            .Include(fv => fv.Product)
            .Where(fv => fv.Product.OwnerId == user.Id);
        var productsToDelete = _context.Products.Where(p => p.OwnerId == user.Id);
        _context.Orders.RemoveRange(ordersToDelete);
        _context.Messages.RemoveRange(messagesToDelete);
        _context.FilterValues.RemoveRange(filterValuesToDelete);
        _context.Products.RemoveRange(productsToDelete);
        await _context.SaveChangesAsync();
        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            await _signInManager.SignOutAsync();
        }

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Profile()
    {
        var user = _context.Users.FirstOrDefault(u => u.UserName == User.Identity!.Name);
        var vm = new ProfileViewModel
        {
            User = user,
            ActivePublications =
                _context.Products.Count(p => p.OwnerId == user!.Id && p.PublicationState == PublicationState.Active),
            HiddenPublications =
                _context.Products.Count(p => p.OwnerId == user.Id && p.PublicationState == PublicationState.Hidden),
            ArchivedPublications =
                _context.Products.Count(p => p.OwnerId == user.Id && p.PublicationState == PublicationState.Archived),
            RejectedPublications = _context.Products.Count(p => p.OwnerId == user.Id && p.RejectedAt != null && p.RejectedAt < p.UpdatedAt),
            Sells = _context.Orders.Count(o => o.Product.OwnerId == user.Id),
            Purchases = _context.Orders.Count(o => o.BuyerId == user.Id)
        };
        return View(vm);
    }
}