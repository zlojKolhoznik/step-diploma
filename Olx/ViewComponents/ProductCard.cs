using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olx.Models;

namespace Olx.ViewComponents;

public class ProductCard : ViewComponent
{
    private readonly UserManager<User> _userManager;

    public ProductCard(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync(Product product)
    {
        var userId = _userManager.GetUserId(HttpContext.User);
        var isFavorite = _userManager.Users
            .Include(u => u.Favorites)
            .Single(u => u.Id == userId)
            .Favorites!.Any(p => p.Id == product.Id);
        ViewData["IsFavorite"] = isFavorite;
        return View(product);
    }
}