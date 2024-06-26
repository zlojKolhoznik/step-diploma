﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olx.Models;

namespace Olx.ViewComponents;

public class ProductCard : ViewComponent
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public ProductCard(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<IViewComponentResult> InvokeAsync(Product product)
    {
        var userId = _userManager.GetUserId(HttpContext.User);
        var claimsPrincipal = HttpContext.User;
        var isFavorite = false;
        if (_signInManager.IsSignedIn(claimsPrincipal))
        {
            isFavorite = _userManager.Users
                .Include(u => u.Favorites)
                .SingleOrDefault(u => u.Id == userId)
                ?.Favorites?.Any(p => p.Id == product.Id) ?? false;
        }
        ViewData["IsFavorite"] = isFavorite;
        return View(product);
    }
}